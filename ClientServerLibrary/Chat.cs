using System.Net;

namespace ClientServerLibrary
{
    public static class Chat
    {
        private static readonly string _serverIP = "127.0.0.1";
        private static readonly string _clientIP = "127.0.0.1";
        private static readonly int _serverPortFirst = 12345;
        private static readonly int _serverPortLast = 23456;
        public static IPEndPoint GetServerEndPoint()
        {
            return new IPEndPoint(IPAddress.Parse(_serverIP), _serverPortFirst);
        }
        public static string GetServerEndPointAsString()
        {
            return $"tcp://{_serverIP}:{_serverPortFirst}";
        }
        public static IPEndPoint GetClientEndPoint(int port)
        {
            return new IPEndPoint(IPAddress.Parse(_clientIP), port);
        }
        public static string GetClientEndPointAsString(int port)
        {
            return $"tcp://{_clientIP}:{port}";
        }
        public static int GetServerPortFirst()
        {
            return _serverPortFirst;
        }
        public static int GetServerPortLast()
        {
            return _serverPortLast;
        }
        public static bool IsEquals(string str1, string str2)
        {
            return string.Equals(str1, str2, StringComparison.InvariantCultureIgnoreCase);
        }
        public static async ValueTask<string?> EnterTextAsync(string aboutInput, CancellationToken token)
        {
            await Console.Out.WriteLineAsync(aboutInput);
            return await Console.In.ReadLineAsync(token);
        }
        public static async Task ExitAsync()
        {
            Console.WriteLine("Запрошен выход. Приложение закроется через секунду.");
            await Task.Delay(1000);
            Environment.Exit(0);
        }
    }
}
