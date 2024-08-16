using ChatObjectsLibrary;
using MessagesSourceLibrary;

namespace HomeworkGB9Tests
{
    internal class MockMessageSourceServer<T> : IMessagesSource<T> where T : class
    {
        private readonly Queue<Message> fakeMessages = new();
        public MockMessageSourceServer()
        {
            fakeMessages.Enqueue(new Message("Федор") { Command = Command.Register });
            fakeMessages.Enqueue(new Message("Евгения") { Command = Command.Register });
            fakeMessages.Enqueue(new Message("Федор", "Привет! Я Федор."));
            fakeMessages.Enqueue(new Message("Евгения", "Привет, Федор! Я Евгения."));
            fakeMessages.Enqueue(new Message("Федор", "Приятно познакомиться, Евгения."));
            fakeMessages.Enqueue(new Message("Евгения", "Взаимно, Федор."));
        }
        public async Task<Message?> ReceiveAsync(CancellationToken token, MemberBuilder<T>? builder)
        {
            await Task.CompletedTask;
            if (!fakeMessages.TryDequeue(out var message))
            {
                return null;
            }
            return message;
        }
        public async Task SendAsync(Message message, CancellationToken token, T? endPoint)
        {
            await Task.CompletedTask;
            return;
        }
    }
}
