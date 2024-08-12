using ChatObjectsLibrary;
using ClientServerLibrary;
using MessagesSourceLibrary;
using System.Net;

namespace HomeworkGB9Tests
{
    internal class MockMessageSourceServer : IMessagesSource<IPEndPoint>
    {
        private readonly Queue<Message> fakeMessages = new();
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
        public async Task<Message?> ReceiveAsync(CancellationToken token, MemberBuilder<IPEndPoint>? builder)
        {
            await Task.CompletedTask;
            builder?.BuildEndPoint(endPoint);
            return fakeMessages.TryDequeue(out var message) ? message : null;
        }
        public async Task SendAsync(Message message, CancellationToken token, IPEndPoint? endPoint)
        {
            await Task.CompletedTask;
            return;
        }
    }
}
