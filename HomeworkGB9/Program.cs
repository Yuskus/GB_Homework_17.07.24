namespace HomeworkGB9
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Соединение...");
                var server = ChatServer.Instance();
                await server.TryAcceptAsync();
            }
            else
            {
                var client = new ChatClient(args[0], int.Parse(args[1]));
                await client.StartChat(); 
            }

            Console.WriteLine("До свидания!");
        }
    }
}
