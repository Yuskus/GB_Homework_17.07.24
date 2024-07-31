using System.Net;

namespace HomeworkGB9
{
    public static class Chat
    {
        private static readonly IPEndPoint serverEndPoint = new(IPAddress.Parse("127.0.0.1"), 12345);
        private static readonly IPAddress clientIP = IPAddress.Parse("127.0.0.1");
        public static IPEndPoint GetServerEndPoint()
        {
            return serverEndPoint;
        }
        public static IPEndPoint GetClientEndPoint(int port)
        {
            return new IPEndPoint(clientIP, port);
        }
        public static bool IsEquals(string str1, string str2)
        {
            return string.Equals(str1, str2, StringComparison.InvariantCultureIgnoreCase);
        }
        public static string EnterText(string aboutInput)
        {
            Console.WriteLine(aboutInput);
            return Console.ReadLine() ?? "";
        }
    }
}
