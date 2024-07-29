using System.Net;
using System.Net.Sockets;
using System.Text;

namespace HomeworkGB9
{
    internal class ChatClient
    {
        public string Nickname { get; }
        private IPEndPoint remoteEndPoint = new(IPAddress.Parse("127.0.0.1"), 12345);
        private UdpClient udpClient;
        public ChatClient(string name, int port)
        {
            Nickname = name;
            var localEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
            udpClient = new UdpClient(localEndPoint);
            udpClient.Connect(remoteEndPoint);
        }
        public async Task StartChat()
        {
            using (var cts = new CancellationTokenSource())
            {
                try
                {
                    Task[] tasks = [AcceptMessageAsync(cts.Token), SendMessageAsync(cts) ];
                    await Task.WhenAll(tasks);
                }
                catch (OperationCanceledException exception)
                {
                    Console.WriteLine(exception.Message);
                }
            }
        }
        public async Task SendMessageAsync(CancellationTokenSource cts)
        {
            while (true)
            {
                string text = EnterText("Вы можете ввести своё сообщение.");
                string toName = EnterText("Укажите адресата.");

                if (string.IsNullOrEmpty(text)) continue;
                if (text.ToLower().Equals("exit")) cts.Cancel();

                try
                {
                    cts.Token.ThrowIfCancellationRequested();
                    var message = new Message(Nickname, text);
                    if (toName != "") message.ToName = toName;
                    string json = message.GetJson();
                    byte[] buffer = Encoding.UTF8.GetBytes(json);
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
        public async Task AcceptMessageAsync(CancellationToken token)
        {
            while (true)
            {
                try
                {
                    token.ThrowIfCancellationRequested();
                    var data = await udpClient.ReceiveAsync(token);
                    string json = Encoding.UTF8.GetString(data.Buffer);
                    var message = Message.GetMessage(json);
                    Console.WriteLine(message);
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
        private static string EnterText(string aboutInput)
        {
            Console.WriteLine(aboutInput);
            return Console.ReadLine() ?? "";
        }
    }
}
