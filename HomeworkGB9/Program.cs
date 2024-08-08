using HomeworkGB9.Abstractions;

namespace HomeworkGB9
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Соединение...");
                int port = Chat.GetServerEndPoint().Port;
                var source = new MessageSourceServer(port);
                var server = new ChatServer(source);
                await server.StartServerAsync();
            }
            else
            {
                string name = args[0];
                int port = int.Parse(args[1]);
                var source = new MessageSourceClient(port);
                var client = new ChatClient(name, source);
                await client.StartClientAsync(); 
            }

            Console.WriteLine("До свидания!");
        }
    }
}
