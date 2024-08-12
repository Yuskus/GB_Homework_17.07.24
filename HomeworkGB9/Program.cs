using ClientServerLibrary;
using MessagesSourceNetMQLibrary;
using MessagesSourceUDPLibrary;
using NetMQ;
using System.Net;

namespace HomeworkGB9
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length == 0)
            {
                await ServerUsesNetMQ(); //или ServerUsesUDP()
            }
            else
            {
                await ClientUsesNetMQ(args[0]); //или ClientUsesUDP(args[0], args[1])
            }

            Console.WriteLine("До свидания!");
        }
        public static async Task ServerUsesNetMQ()
        {
            await Task.CompletedTask;
            int port = Chat.GetServerPort();
            var source = new MessagesSourceNetMQServer(port);
            var server = new ChatServer<string>(source);
            using var runtime = new NetMQRuntime();
            runtime.Run(server.StartServerAsync());
        }
        public static async Task ClientUsesNetMQ(string name)
        {
            await Task.CompletedTask;
            string serverEndPoint = Chat.GetServerEndPointAsString();
            var source = new MessagesSourceNetMQClient(serverEndPoint);
            var client = new ChatClient<string>(name, source);
            using var runtime = new NetMQRuntime();
            runtime.Run(client.StartClientAsync());
        }
        public static async Task ServerUsesUDP()
        {
            int port = Chat.GetServerPort();
            var source = new MessagesSourceUDPServer(port);
            var server = new ChatServer<IPEndPoint>(source);
            await server.StartServerAsync();
        }
        public static async Task ClientUsesUDP(string arg1, string arg2)
        {
            string name = arg1;
            int port = int.Parse(arg2);
            IPEndPoint serverEndPoint = Chat.GetServerEndPoint();
            IPEndPoint clientEndPoint = Chat.GetClientEndPoint(port);
            var source = new MessagesSourceUDPClient(serverEndPoint, clientEndPoint);
            var client = new ChatClient<IPEndPoint>(name, source);
            await client.StartClientAsync();
        }
    }
}
