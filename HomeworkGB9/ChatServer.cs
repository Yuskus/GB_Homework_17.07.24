using System.Net.Sockets;
using System.Net;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace HomeworkGB9
{
    internal class ChatServer
    {
        public static async Task AcceptMessageAsync(int port, CancellationToken token)
        {
            IPEndPoint endPoint;
            UdpClient udp = new(port);
            Console.WriteLine("Connecting...");

            while (true)
            {
                token.ThrowIfCancellationRequested();
                try
                {
                    UdpReceiveResult data = await udp.ReceiveAsync(token);
                    endPoint = data.RemoteEndPoint;
                    byte[] buffer = data.Buffer;
                    string json = Encoding.UTF8.GetString(buffer);
                    Message? msg = Message.GetMessage(json);
                    if (msg == null)
                    {
                        Console.WriteLine("Error.");
                        continue;
                    }
                    else if (string.Equals(msg.Text, "exit", StringComparison.InvariantCultureIgnoreCase))
                    {
                        throw new OperationCanceledException("Отмена операции со стороны клиента.");
                    }
                    Console.WriteLine(msg);
                    Message message = new("Server", "Message sent.");
                    string backJson = message.GetJson();
                    byte[] backBuffer = Encoding.UTF8.GetBytes(backJson);
                    await udp.SendAsync(backBuffer, endPoint, token);
                    Console.WriteLine(message);
                }
                catch (OperationCanceledException) //если исключение вызвано запросом отмены
                {
                    throw; //перебрасываем его дальше, в вызывающий код
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public static async Task TryAcceptAsync(int port)
        {
            using (var cts =  new CancellationTokenSource())
            {
                new Task(() =>
                {
                    Console.ReadKey(true);
                    cts.Cancel();
                }).Start();

                try
                {
                    await AcceptMessageAsync(port, cts.Token);
                }
                catch (OperationCanceledException exception)
                {
                    await Console.Out.WriteLineAsync(exception.Message);
                }
            }
        }
    }
}
