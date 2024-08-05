using System.Net;
using System.Net.Sockets;
using System.Text;

namespace HomeworkGB9
{
    internal class ChatClient
    {
        public string Nickname { get; }
        private readonly IPEndPoint localEndPoint;
        private readonly IPEndPoint remoteEndPoint;
        private readonly UdpClient udpClient;
        public ChatClient(string name, int port)
        {
            Nickname = name;
            localEndPoint = Chat.GetClientEndPoint(port);
            remoteEndPoint = Chat.GetServerEndPoint();
            udpClient = new UdpClient(localEndPoint);
            udpClient.Connect(remoteEndPoint);
        }
        public async Task StartClientAsync()
        {
            using var cts = new CancellationTokenSource();
            try
            {
                Task[] tasks = [AcceptMessagesAsync(cts.Token), SendMessagesAsync(cts)];
                await Task.WhenAll(tasks);
            }
            catch (OperationCanceledException exception)
            {
                Console.WriteLine(exception.Message);
            }
        }
        public async Task SendMessagesAsync(CancellationTokenSource cts)
        {
            while (true)
            {
                string text = Chat.EnterText("Вы можете ввести своё сообщение.");
                string toName = Chat.EnterText("Укажите адресата.");

                if (string.IsNullOrEmpty(text)) continue;
                if (Chat.IsEquals(text, "exit")) cts.Cancel();

                try
                {
                    //проверка прерывания
                    cts.Token.ThrowIfCancellationRequested();

                    //отправка
                    Message message = new(Nickname, toName, text);
                    byte[] buffer = Encoding.UTF8.GetBytes(message.GetJson());
                    await udpClient.SendAsync(buffer);
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
        public async Task AcceptMessagesAsync(CancellationToken token)
        {
            while (true)
            {
                try
                {
                    //проверка прерывания
                    token.ThrowIfCancellationRequested();

                    //прием
                    var data = await udpClient.ReceiveAsync(token);
                    string json = Encoding.UTF8.GetString(data.Buffer);
                    var message = Message.GetMessage(json);
                    Console.WriteLine(message);

                    //подтверждение доставки
                    var confirmMessage = new Message(Nickname, "confirm") { Id = message?.Id };
                    byte[] buffer = Encoding.UTF8.GetBytes(confirmMessage.GetJson());
                    await udpClient.SendAsync(buffer, token);
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
    }
}
