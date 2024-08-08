using HomeworkGB9.Abstractions;

namespace HomeworkGB9
{
    public class ChatClient(string name, IMessageSource source)
    {
        public string Nickname { get; } = name;
        private readonly IMessageSource messageSource = source;

        //запуск клиента асинхронно
        public async Task StartClientAsync()
        {
            using var cts = new CancellationTokenSource();
            try
            {
                //запуск асинхронно приема и отправки
                await Task.WhenAll(ReceiveMessagesAsync(cts.Token), SendMessagesAsync(cts));
            }
            catch (OperationCanceledException exception)
            {
                //обработка исключения прерывания и завершение работы программы
                Console.WriteLine(exception.Message);
            }

            //запрос на удаления из списка участников чата
            await Delete();
        }

        //отправка сообщений асинхронно
        public async Task SendMessagesAsync(CancellationTokenSource cts)
        {
            CancellationToken token = cts.Token;

            //автоматическая регистрация пользователя при входе
            Console.WriteLine("enter0");
            await Register(token);
            Console.WriteLine("enter");

            while (true)
            {
                //ввод
                string text = Chat.EnterText("Вы можете ввести своё сообщение.");
                Console.WriteLine(text);
                string toName = Chat.EnterText("Укажите адресата.");
                Console.WriteLine(toName);

                //проверки
                if (string.IsNullOrEmpty(text)) continue;
                if (Chat.IsEquals(text, "exit")) cts.Cancel();

                try
                {
                    //проверка прерывания
                    token.ThrowIfCancellationRequested();

                    //отправка
                    var message = new Message(Nickname, toName, text);
                    await messageSource.SendAsync(message, token);
                }
                catch (OperationCanceledException)
                {
                    //пересыл исключения прерывания в вызывающий метод
                    throw;
                }
                catch (Exception ex)
                {
                    //ошибки при отправке сообщений
                    Console.WriteLine(ex.Message);
                }
            }
        }

        //прием сообщений асинхронно
        public async Task ReceiveMessagesAsync(CancellationToken token)
        {
            while (true)
            {
                try
                {
                    //проверка прерывания
                    token.ThrowIfCancellationRequested();

                    //прием
                    var message = await messageSource.ReceiveAsync(token);
                    Console.WriteLine(message);

                    //подтверждение доставки сообщения
                    await Confirm(message, token);
                }
                catch (OperationCanceledException)
                {
                    //пересыл исключения прерывания в вызывающий метод
                    throw;
                }
                catch (Exception ex)
                {
                    //ошибки при приеме или отправке сообщений
                    Console.WriteLine(ex.Message);
                }
            }
        }

        //запрос на регистрацию в чате
        private async Task Register(CancellationToken token)
        {
            Message registerMessage = new(Nickname) { Command = Command.Register };
            await messageSource.SendAsync(registerMessage, token);
        }

        //подтверждение доставки сообщения
        private async Task Confirm(Message? message, CancellationToken token)
        {
            Message confirmMessage = new(Nickname) { Id = message?.Id, Command = Command.Confirm };
            await messageSource.SendAsync(confirmMessage, token);
        }

        //запрос на удаление
        private async Task Delete()
        {
            Message deleteMessage = new(Nickname) { Command = Command.Delete };
            await messageSource.SendAsync(deleteMessage, default);
        }
    }
}
