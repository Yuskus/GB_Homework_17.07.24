using ChatObjectsLibrary;
using MessagesSourceLibrary;

namespace HomeworkGB9Tests
{
    internal class MockMessageSourceClient<T> : IMessagesSource<T> where T : class
    {
        private readonly Queue<Message> fakeMessages = new();
        public readonly Queue<Message> confirmMessages = new();
        public readonly Queue<Message> deliveredMessages = new();
        public MockMessageSourceClient()
        {
            fakeMessages.Enqueue(new Message("Server", "Федор вошел(ла) в чат."));
            fakeMessages.Enqueue(new Message("Server", "Евгения вошел(ла) в чат."));
            fakeMessages.Enqueue(new Message("Федор", "Привет! Я Федор."));
            fakeMessages.Enqueue(new Message("Евгения", "Привет, Федор! Я Евгения."));
            fakeMessages.Enqueue(new Message("Федор", "Приятно познакомиться, Евгения."));
            fakeMessages.Enqueue(new Message("Евгения", "Взаимно, Федор."));
        }
        public async Task<Message?> ReceiveAsync(CancellationToken token, MemberBuilder<T>? builder = null)
        {
            await Task.CompletedTask;
            if (fakeMessages.TryDequeue(out var message))
            {
                deliveredMessages.Enqueue(message);
                return message;
            }
            return null;
        }

        public async Task SendAsync(Message message, CancellationToken token, T? endPoint = null)
        {
            await Task.CompletedTask;
            confirmMessages.Enqueue(message);
            return;
        }
    }
}
