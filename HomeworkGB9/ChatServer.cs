using System.Net.Sockets;
using System.Net;
using System.Text;
using HomeworkGB9.Model;

namespace HomeworkGB9
{
    internal class ChatServer
    {
        public string Nickname { get; } = "Server";
        public Dictionary<string, IPEndPoint> ChatMembers = [];
        private static readonly Lazy<ChatServer> instance = new(() => new ChatServer());
        private readonly UdpClient udpClient = new(12345);
        private ChatServer() { }
        public static ChatServer Instance() => instance.Value;
        public async Task StartServerAsync() //запуск сервера
        {
            using var cts = new CancellationTokenSource();
            try
            {
                new Task(() =>
                {
                    Console.ReadKey(true);
                    cts.Cancel();
                }).Start();

                await AcceptMessagesAsync(cts.Token);
            }
            catch (OperationCanceledException exception)
            {
                Console.WriteLine(exception.Message);
            }
        }
        public async Task AcceptMessagesAsync(CancellationToken token) //прием сообщений и ответы на них
        {
            MemberBuilder builder = new();

            while (true)
            {
                token.ThrowIfCancellationRequested();
                try
                {
                    Message? message = await AcceptAsync(builder, token);
                    if (message == null) continue;

                    Console.WriteLine(message);

                    builder.BuildName(message.FromName);
                    Member sender = builder.GetMember();

                    ChatMembers[sender.Name] = sender.EndPoint!;
                    Message reply = GetReplyMessage(message, sender);
                    if (message.Text == "confirm") continue;

                    await SendAsync(reply, sender.EndPoint!, token); //подтверждение отправки клиенту
                    await SendUnrecievedAsync(sender, token); //проверка, нет ли непрочитанных для клиента
                    await SendToAllAsync(message, sender, token); //отправка сообщения клиенту (или клиентам)
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        private Message GetReplyMessage(Message message, Member sender)
        {
            Message reply = new(Nickname, "Сообщение отправлено");

            if (Chat.IsEquals(message.Text, "register"))
            {
                if (Register(sender))
                {
                    reply.Text = $"Пользователь {sender.Name} зарегестрирован";
                }
            }
            else if (Chat.IsEquals(message.Text, "delete"))
            {
                if (ChatMembers.Remove(sender.Name))
                {
                    reply.Text = $"Пользователь {sender.Name} вышел из чата";
                }
            }
            else if (Chat.IsEquals(message.Text, "list"))
            {
                reply.Text = $"Сейчас онлайн: {string.Join(", ", ChatMembers.Keys)}";
            }
            else if (Chat.IsEquals(message.Text, "confirm"))
            {
                Confirm(message.Id);
            }

            return reply;
        }
        private async Task SendUnrecievedAsync(Member sender, CancellationToken token)
        {
            using var context = new ChatDbContext();

            var senderFromDb = context.Users.FirstOrDefault(x => x.Name == sender.Name);
            if (senderFromDb == null) return;
            var undeliveredList = senderFromDb.ToMessages.Where(x => x.IsRecieved == false).ToList();

            foreach (var undeliveredMessage in undeliveredList)
            {
                var message = Message.ConvertFromDatabase(undeliveredMessage);
                message.Text = "[непрочитанное сообщение]" + message.Text;
                await SendAsync(message, sender.EndPoint!, token);
                undeliveredMessage.IsRecieved = true;
                context.SaveChanges();
            }
        }
        private async Task SendToAllAsync(Message message, Member sender, CancellationToken token)
        {
            if (ChatMembers.TryGetValue(message.ToName, out var memberEndPoint))
            {
                AddMessageToDatabase(message);
                await SendAsync(message, memberEndPoint, token);
            }
            else
            {
                if (!ChatMembers.ContainsKey(sender.Name)) return;

                List<Task> tasks = [];

                foreach (var chatMember in ChatMembers)
                {
                    if (chatMember.Key != sender.Name)
                    {
                        message.ToName = chatMember.Key;
                        AddMessageToDatabase(message);
                        tasks.Add(SendAsync(message, chatMember.Value, token));
                    }
                }
                await Task.WhenAll(tasks);
            }
        }
        private async Task<Message?> AcceptAsync(MemberBuilder builder, CancellationToken token)
        {
            var data = await udpClient.ReceiveAsync(token);
            builder.BuildEndPoint(data.RemoteEndPoint);
            string json = Encoding.UTF8.GetString(data.Buffer);
            return Message.GetMessage(json);
        }
        private async Task SendAsync(Message message, IPEndPoint endPoint, CancellationToken token)
        {
            string json = message.GetJson();
            byte[] buffer = Encoding.UTF8.GetBytes(json);
            await udpClient.SendAsync(buffer, endPoint, token);
        }
        private static bool Register(Member sender)
        {
            using var context = new ChatDbContext();

            if (context.Users.FirstOrDefault(x => x.Name == sender.Name) == null)
            {
                context.Users.Add(new User() { Name = sender.Name });
                context.SaveChanges();
                return true;
            }
            return false;
        }
        private static void Confirm(int? id)
        {
            using var context = new ChatDbContext();
            var message = context.Messages.FirstOrDefault(x => x.Id == id);

            if (message != null)
            {
                message.IsRecieved = true;
                context.SaveChanges();
            }
        }
        private static void AddMessageToDatabase(Message message)
        {
            using var context = new ChatDbContext();

            var sender = context.Users.FirstOrDefault(x => x.Name == message.FromName);
            var recipient = context.Users.FirstOrDefault(x => x.Name == message.ToName);

            Console.WriteLine("sender: " + sender);
            Console.WriteLine("recipient: " + recipient);

            var dbMessage = new Model.Message()
            {
                Text = message.Text,
                CreationTime = message.Time,
                IsRecieved = false,
                SenderId = sender?.Id,
                RecipientId = recipient?.Id
            };

            context.Messages.Add(dbMessage);
            message.Id = dbMessage.Id;
            context.SaveChanges();
        }
    }
}
