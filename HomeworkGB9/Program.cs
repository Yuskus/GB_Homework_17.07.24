using ClientServerLibrary;
using MessagesSourceNetMQLibrary;
using MessagesSourceUDPLibrary;
using System.Net;

namespace HomeworkGB9
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length == 0)
            {
                await ServerUsesNetMQ();
            }
            else
            {
                await ClientUsesNetMQ(args[0]);
            }

            // или если бы мы хотели запустить клиент-сервер на udp:
            //
            // if (args.Length < 2)
            // {
            //     ServerUsesUDP();
            // }
            // else
            // {
            //     ClientUsesUDP(args[0], args[1]);
            // }

            await Console.Out.WriteLineAsync("До свидания!");
        }
        public static async Task ServerUsesNetMQ()
        {
            int serverPort1 = Chat.GetServerPortFirst();
            int serverPort2 = Chat.GetServerPortLast();
            var source = new MessagesSourceNetMQServer(serverPort1, serverPort2);
            var server = new ChatServer<string>(source);
            await server.StartServerAsync();
        }
        public static async Task ClientUsesNetMQ(string name)
        {
            int serverPort1 = Chat.GetServerPortFirst();
            int serverPort2 = Chat.GetServerPortLast();
            var source = new MessagesSourceNetMQClient(name, serverPort1, serverPort2);
            var client = new ChatClient<string>(name, source);
            await client.StartClientAsync();
        }
        public static async Task ServerUsesUDP()
        {
            int port = Chat.GetServerPortFirst();
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
