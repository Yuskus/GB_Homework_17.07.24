using System.Net.Sockets;
using System.Net;
using System.Text;

namespace HomeworkGB9
{
    internal class ChatServer
    {
        private static readonly Lazy<ChatServer> instance = new(() => new ChatServer());
        public string Nickname { get; } = "Server";
        public Dictionary<string, IPEndPoint> ChatMembers = new();
        private UdpClient udpClient = new(12345);
        private ChatServer() { }
        public static ChatServer Instance() => instance.Value;
        public async Task TryAcceptAsync()
        {
            using (var cts = new CancellationTokenSource())
            {
                try
                {
                    new Task(() =>
                    {
                        Console.ReadKey(true);
                        cts.Cancel();
                    }).Start();

                    await AcceptMessageAsync(cts.Token);
                }
                catch (OperationCanceledException exception)
                {
                    Console.WriteLine(exception.Message);
                }
            }
        }
        public async Task AcceptMessageAsync(CancellationToken token)
        {
            MemberBuilder builder = new();

            while (true)
            {
                token.ThrowIfCancellationRequested();
                try
                {
                    var message = await Accept(builder, token);
                    Console.WriteLine(message);

                    if (message == null) continue;

                    builder.BuildName(message.FromName);
                    var member = builder.GetMember();

                    await SendBack(member, message, token);
                    await SendToAll(member, message, token);
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
        private async Task<Message?> Accept(MemberBuilder builder, CancellationToken token)
        {
            var data = await udpClient.ReceiveAsync(token);
            builder.BuildEndPoint(data.RemoteEndPoint);
            string json = Encoding.UTF8.GetString(data.Buffer);
            return Message.GetMessage(json);
        }
        private async Task SendToOne(Message message, IPEndPoint endPoint, CancellationToken token)
        {
            string json = message.GetJson();
            byte[] buffer = Encoding.UTF8.GetBytes(json);
            await udpClient.SendAsync(buffer, endPoint, token);
        }
        private async Task SendBack(Member member, Message message, CancellationToken token)
        {
            Message reply = new(Nickname, "Ошибка"); //дефолтное значение 

            if (IsStringEquals(message.Text, "register"))
            {
                if (ChatMembers.TryAdd(member.Name!, member.EndPoint!))
                {
                    reply.Text = $"Пользователь {member.Name} успешно добавлен";
                }
            }
            else if (IsStringEquals(message.Text, "delete"))
            {
                if (ChatMembers.Remove(member.Name!))
                {
                    reply.Text = $"Пользователь {member.Name} успешно удален";
                }
            }
            else if (IsStringEquals(message.Text, "list"))
            {
                reply.Text = $"Список участников чата: {string.Join(", ", ChatMembers.Keys)}";
            }
            else
            {
                reply.Text = "Сообщение отправлено";
            }

            await SendToOne(reply, member.EndPoint!, token);
        }
        private async Task SendToAll(Member member, Message message, CancellationToken token)
        {
            if (IsStringEquals(message.ToName, ""))
            {
                if (!ChatMembers.ContainsKey(member.Name)) return;

                List<Task> tasks = new();

                foreach (var chatMember in ChatMembers)
                {
                    if (chatMember.Key != member.Name)
                    {
                        tasks.Add(SendToOne(message, chatMember.Value, token));
                    }
                }
                await Task.WhenAll(tasks);
            }
            else if (ChatMembers.ContainsKey(message.ToName))
            {
                await SendToOne(message, ChatMembers[message.ToName], token);
            }
        }
        private static bool IsStringEquals(string str1, string str2)
        {
            return string.Equals(str1, str2, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
