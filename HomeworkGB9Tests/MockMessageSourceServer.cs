using HomeworkGB9;
using HomeworkGB9.Abstractions;
using System.Net;

namespace HomeworkGB9Tests
{
    internal class MockMessageSourceServer : IMessageSource
    {
        private readonly Queue<Message> fakeMessages = new();
        private ChatServer? server;
        private readonly IPEndPoint endPoint = new(IPAddress.Any, 0);
        public MockMessageSourceServer()
        {
            fakeMessages.Enqueue(new Message("Федор") { Command = Command.Register });
            fakeMessages.Enqueue(new Message("Евгения") { Command = Command.Register });
            fakeMessages.Enqueue(new Message("Федор", "Привет! Я Федор."));
            fakeMessages.Enqueue(new Message("Евгения", "Привет, Федор! Я Евгения."));
            fakeMessages.Enqueue(new Message("Федор", "Приятно познакомиться, Евгения."));
            fakeMessages.Enqueue(new Message("Евгения", "Взаимно, Федор."));
        }
        public void AddServer(ChatServer server)
        {
            this.server = server;
        }
        public async Task<Message?> ReceiveAsync(CancellationToken token, MemberBuilder? builder)
        {
            await Task.Delay(1, token);
            builder?.BuildEndPoint(endPoint);
            return fakeMessages.TryDequeue(out var message) ? message : null;
        }
        public async Task SendAsync(Message message, CancellationToken token, IPEndPoint? endPoint)
        {
            await Task.Delay(1, token);
            return;
        }
    }
}
