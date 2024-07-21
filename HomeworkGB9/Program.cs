namespace HomeworkGB9
{
    internal class Program
    {
        static void Main(string[] args)
        {
            const int PORT = 12345;
            
            if (args.Length == 0)
            {
                Thread thread = new(() => { ChatServer.AcceptMessage(PORT); }) { IsBackground = true };
                thread.Start();
                Console.ReadKey(true);
            }
            else
            {
                ChatClient.SendMessage(PORT, args[0]); 
            }

            Console.WriteLine("Good bye!");
        }
    }
}
