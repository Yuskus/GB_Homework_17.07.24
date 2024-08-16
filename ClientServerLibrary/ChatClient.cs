using ChatObjectsLibrary;
using MessagesSourceLibrary;
using System;

namespace ClientServerLibrary
{
    public class ChatClient<T>(string name, IMessagesSource<T> source) where T : class
    {
        public string Nickname { get; } = name;
        private readonly IMessagesSource<T> messageSource = source;

        //запуск клиента асинхронно
        public async Task StartClientAsync()
        {
            using var cts = new CancellationTokenSource();
            Task[] tasks = [ ReceiveMessagesAsync(cts.Token), SendMessagesAsync(cts) ];

            await Task.WhenAll(tasks);
        }

        //отправка сообщений асинхронно
        public async Task SendMessagesAsync(CancellationTokenSource cts)
        {
            CancellationToken token = cts.Token;

            //автоматическая регистрация пользователя при входе
            await Register(token);

            while (true)
            {
                //ввод
                string text = await Chat.EnterTextAsync("Вы можете ввести своё сообщение.", token) ?? "";
                string toName = await Chat.EnterTextAsync("Укажите адресата.", token) ?? "";

                //проверки
                if (string.IsNullOrEmpty(text)) continue;
                if (Chat.IsEquals(text, "exit")) cts.Cancel();

                try
                {
                    //проверка прерывания
                    token.ThrowIfCancellationRequested();

                    //отправка
                    await messageSource.SendAsync(new Message(Nickname, toName, text), token);
                }
                catch (OperationCanceledException ex)
                {
                    //прерывание операции
                    Console.WriteLine(ex.Message);
                    break;
                }
                catch (Exception ex)
                {
                    //ошибки при отправке сообщений
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
            }

            //запрос на удаления из списка участников чата
            await Delete();

            await Chat.ExitAsync();
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

                    if (message == null) { continue; }
                    Console.WriteLine(message);

                    //подтверждение доставки сообщения
                    await Confirm(message, token);
                }
                catch (OperationCanceledException ex)
                {
                    //прерывание операции
                    Console.WriteLine(ex.Message);
                    break;
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
