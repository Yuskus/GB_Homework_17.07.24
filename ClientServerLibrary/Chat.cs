using System.Net;

namespace ClientServerLibrary
{
    public static class Chat
    {
        private static readonly string _serverIP = "127.0.0.1";
        private static readonly string _clientIP = "127.0.0.1";
        private static readonly int _serverPort = 12345;
        public static IPEndPoint GetServerEndPoint()
        {
            return new IPEndPoint(IPAddress.Parse(_serverIP), _serverPort);
        }
        public static string GetServerEndPointAsString()
        {
            return $"tcp://{_serverIP}:{_serverPort}";
        }
        public static IPEndPoint GetClientEndPoint(int port)
        {
            return new IPEndPoint(IPAddress.Parse(_clientIP), port);
        }
        public static string GetClientEndPointAsString(int port)
        {
            return $"tcp://{_clientIP}:{port}";
        }
        public static int GetServerPort()
        {
            return _serverPort;
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
