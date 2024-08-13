using ChatObjectsLibrary;
using MessagesSourceLibrary;
using ModelEFCoreLibrary;

namespace ClientServerLibrary
{
    public class ChatServer<T>(IMessagesSource<T> source) where T : class
    {
        public string Nickname { get; } = "Server";
        private readonly IMessagesSource<T> messageSource = source;
        private readonly Dictionary<string, T> _chatMembers = [];

        //запуск сервера асинхронно
        public async Task StartServerAsync()
        {
            using var cts = new CancellationTokenSource();
            Task task = ReceiveSendAsync(cts.Token);
            Console.ReadKey(true);
            cts.Cancel();
            await task;
        }

        //прием сообщений асинхронно
        public async Task ReceiveSendAsync(CancellationToken token) //прием сообщений и ответы на них
        {
            Console.WriteLine("Соединение...");

            MemberBuilder<T> builder = new();

            while (true)
            {
                try
                {
                    //проверка прерывания
                    token.ThrowIfCancellationRequested();

                    //прием сообщения
                    Message? message = await messageSource.ReceiveAsync(token, builder);

                    //прерывание отправки ответа при ошибке, иначе - вывод сообщения в консоль
                    if (message == null) continue;
                    Console.WriteLine(message);

                    //создание объекта отправителя для извлечения ключ-значения
                    builder.BuildName(message.FromName);
                    Member<T> sender = builder.GetMember();

                    //проверка и исполнение команды (или отправка сообщений)
                    switch (message.Command)
                    {
                        case Command.Confirm: Confirm(message.Id); break;
                        case Command.Register: await RegisterAsync(sender, token); break;
                        case Command.Delete: await DeleteAsync(sender, token); break;
                        default: await MainUdpSendingAsync(message, sender, token); break;
                    }
                }
                catch (OperationCanceledException ex)
                {
                    //отмена операции
                    Console.WriteLine(ex.Message);
                    break;
                }
                catch (Exception ex)
                {
                    //ошибки при приеме или отправке сообщений
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
            }
        }

        //отправка непрочитанных пользователю
        private async Task SendUnreceivedAsync(Member<T> sender, CancellationToken token)
        {
            using var context = new ChatDbContext();

            var senderFromDb = context.Users.FirstOrDefault(x => x.Name == sender.Name);
            if (senderFromDb == null) return;
            var undeliveredList = senderFromDb.ToMessages.Where(x => x.IsReceived == false).ToList();

            foreach (var undeliveredMessage in undeliveredList)
            {
                var message = Message.ConvertFromDatabase(undeliveredMessage);
                message.Text = "[непрочитанное сообщение] " + message.Text;
                await messageSource.SendAsync(message, token, sender.EndPoint!);
                undeliveredMessage.IsReceived = true;
                context.SaveChanges();
            }
        }

        //отправка адресату (или адресатам)
        private async Task MainUdpSendingAsync(Message message, Member<T> sender, CancellationToken token)
        {
            //подтверждение отправки клиенту
            await messageSource.SendAsync(new Message(Nickname, "Сообщение отправлено"), token, sender.EndPoint!);

            //отправка сообщения клиенту (или клиентам)
            if (_chatMembers.TryGetValue(message.ToName, out var memberEndPoint))
            {
                AddMessageToDatabase(message);
                await messageSource.SendAsync(message, token, memberEndPoint);
            }
            else
            {
                if (!_chatMembers.ContainsKey(sender.Name)) return;

                List<Task> tasks = [];

                foreach (var chatMember in _chatMembers)
                {
                    if (chatMember.Key != sender.Name)
                    {
                        message.ToName = chatMember.Key;
                        AddMessageToDatabase(message);
                        tasks.Add(messageSource.SendAsync(message, token, chatMember.Value));
                    }
                }
                await Task.WhenAll(tasks);
            }
        }

        private async Task SendNotifyAsync(Message message, Member<T> sender, CancellationToken token)
        {
            List<Task> tasks = [];

            foreach (var chatMember in _chatMembers)
            {
                if (chatMember.Key != sender.Name)
                {
                    tasks.Add(messageSource.SendAsync(message, token, chatMember.Value));
                }
            }
            await Task.WhenAll(tasks);
        }

        //регистрация пользователя, оповещение участников
        private async Task RegisterAsync(Member<T> sender, CancellationToken token)
        {
            //создание ключ-значения (клиент и энд поинт)
            //или переназначение энд поинт на актуальный
            _chatMembers[sender.Name] = sender.EndPoint!;

            using (var context = new ChatDbContext())
            {
                if (context.Users.FirstOrDefault(x => x.Name == sender.Name) == null)
                {
                    context.Users.Add(new UserEntity() { Name = sender.Name });
                    context.SaveChanges();
                }
            }

            //проверка, нет ли непрочитанных для клиента
            await SendUnreceivedAsync(sender, token);

            var notifyMessage = new Message(Nickname, $"{sender.Name} вошел(ла) в чат. " +
                $"Список участников чата: {string.Join(", ", _chatMembers.Keys)}");

            await SendNotifyAsync(notifyMessage, sender, token);
        }

        //подтверждение о том, что сообщение прочитано
        private static void Confirm(int? id)
        {
            using var context = new ChatDbContext();
            var message = context.Messages.FirstOrDefault(x => x.Id == id);

            if (message != null)
            {
                message.IsReceived = true;
                context.SaveChanges();
            }
        }

        //удаление из списка, оповещение участников
        private async Task DeleteAsync(Member<T> sender, CancellationToken token)
        {
            _chatMembers.Remove(sender.Name);

            var notifyMessage = new Message(Nickname, $"{sender.Name} покинул(ла) чат. " +
                $"Список участников чата: {string.Join(", ", _chatMembers.Keys)}");

            await SendNotifyAsync(notifyMessage, sender, token);
        }

        //добавить сообщение в базу данных
        private static void AddMessageToDatabase(Message message)
        {
            using var context = new ChatDbContext();

            var sender = context.Users.FirstOrDefault(x => x.Name == message.FromName);
            var recipient = context.Users.FirstOrDefault(x => x.Name == message.ToName);

            var dbMessage = Message.ConvertToDatabase(message, sender, recipient);

            context.Messages.Add(dbMessage);
            message.Id = dbMessage.Id;
            context.SaveChanges();
        }
    }
}
