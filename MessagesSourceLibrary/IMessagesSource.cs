using ChatObjectsLibrary;

namespace MessagesSourceLibrary
{
    public interface IMessagesSource<T> where T : class
    {
        Task<Message?> ReceiveAsync(CancellationToken token, MemberBuilder<T>? builder = null);
        Task SendAsync(Message message, CancellationToken token, T? endPoint = null);
    }
}
