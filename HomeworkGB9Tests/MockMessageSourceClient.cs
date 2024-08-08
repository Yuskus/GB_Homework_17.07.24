using HomeworkGB9;
using HomeworkGB9.Abstractions;
using System.Net;
using System.Net.Sockets;

namespace HomeworkGB9Tests
{
    internal class MockMessageSourceClient : IMessageSource
    {
        private readonly UdpClient _udpClientServer = new(12345);
        private readonly Queue<Message> fakeMessages = new();
        public MockMessageSourceClient()
        {
            fakeMessages.Enqueue(new Message("Server", "Федор вошел(ла) в чат."));
            fakeMessages.Enqueue(new Message("Server", "Евгения вошел(ла) в чат."));
            fakeMessages.Enqueue(new Message("Федор", "Привет! Я Федор."));
            fakeMessages.Enqueue(new Message("Евгения", "Привет, Федор! Я Евгения."));
            fakeMessages.Enqueue(new Message("Федор", "Приятно познакомиться, Евгения."));
            fakeMessages.Enqueue(new Message("Евгения", "Взаимно, Федор."));
        }
        public async Task<Message?> ReceiveAsync(CancellationToken token, MemberBuilder? builder = null)
        {
            await Task.Delay(1, token);
            return fakeMessages.TryDequeue(out var message) ? message : null;
        }

        public async Task SendAsync(Message message, CancellationToken token, IPEndPoint? endPoint = null)
        {
            await Task.Delay(1, token);
            return;
        }
    }
}
