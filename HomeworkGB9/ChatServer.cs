using System.Net.Sockets;
using System.Net;
using System.Text;

namespace HomeworkGB9
{
    internal class ChatServer
    {
        public string Nickname { get; } = "Server";
        public Dictionary<string, IPEndPoint> ChatMembers = [];
        private static readonly Lazy<ChatServer> instance = new(() => new ChatServer());
        private UdpClient udpClient = new(12345);
        private ChatServer() { }
        public static ChatServer Instance() => instance.Value;
        public async Task StartServerAsync()
        {
            using var cts = new CancellationTokenSource();
            try
            {
                new Task(() =>
                {
                    Console.ReadKey(true);
                    cts.Cancel();
                }).Start();

                await AcceptMessagesAsync(cts.Token);
            }
            catch (OperationCanceledException exception)
            {
                Console.WriteLine(exception.Message);
            }
        }
        public async Task AcceptMessagesAsync(CancellationToken token)
        {
            MemberBuilder builder = new();

            while (true)
            {
                token.ThrowIfCancellationRequested();
                try
                {
                    Message? message = await AcceptAsync(builder, token);
                    Console.WriteLine(message);

                    if (message == null) continue;

                    builder.BuildName(message.FromName);
                    Member sender = builder.GetMember();

                    Message reply = GetReplyMessage(message, sender);
                    await SendAsync(reply, sender.EndPoint!, token);
                    await SendToAllAsync(message, sender, token);
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        private async Task<Message?> AcceptAsync(MemberBuilder builder, CancellationToken token)
        {
            var data = await udpClient.ReceiveAsync(token);
            builder.BuildEndPoint(data.RemoteEndPoint);
            string json = Encoding.UTF8.GetString(data.Buffer);
            return Message.GetMessage(json);
        }
        private async Task SendAsync(Message message, IPEndPoint endPoint, CancellationToken token)
        {
            string json = message.GetJson();
            byte[] buffer = Encoding.UTF8.GetBytes(json);
            await udpClient.SendAsync(buffer, endPoint, token);
        }
        private async Task SendToAllAsync(Message message, Member sender, CancellationToken token)
        {
            if (Chat.IsEquals(message.ToName, ""))
            {
                if (!ChatMembers.ContainsKey(sender.Name)) return;

                List<Task> tasks = [];

                foreach (var chatMember in ChatMembers)
                {
                    if (chatMember.Key != sender.Name)
                    {
                        tasks.Add(SendAsync(message, chatMember.Value, token));
                    }
                }
                await Task.WhenAll(tasks);
            }
            else if (ChatMembers.TryGetValue(message.ToName, out var memberEndPoint))
            {
                await SendAsync(message, memberEndPoint, token);
            }
        }
        private Message GetReplyMessage(Message message, Member sender)
        {
            Message reply = new(Nickname, "Ошибка");

            if (Chat.IsEquals(message.Text, "register"))
            {
                if (ChatMembers.TryAdd(sender.Name!, sender.EndPoint!))
                {
                    reply.Text = $"Пользователь {sender.Name} успешно добавлен";
                }
            }
            else if (Chat.IsEquals(message.Text, "delete"))
            {
                if (ChatMembers.Remove(sender.Name!))
                {
                    reply.Text = $"Пользователь {sender.Name} успешно удален";
                }
            }
            else if (Chat.IsEquals(message.Text, "list"))
            {
                reply.Text = $"Список участников чата: {string.Join(", ", ChatMembers.Keys)}";
            }
            else
            {
                reply.Text = "Сообщение отправлено";
            }

            return reply;
        }
    }
}
