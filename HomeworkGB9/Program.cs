namespace HomeworkGB9
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            const int PORT = 12345;
            
            if (args.Length == 0)
            {
                await ChatServer.TryAcceptAsync(PORT);
            }
            else
            {
                await ChatClient.SendMessageAsync(PORT, args[0]); 
            }

            Console.WriteLine("Good bye!");
        }
    }
}
