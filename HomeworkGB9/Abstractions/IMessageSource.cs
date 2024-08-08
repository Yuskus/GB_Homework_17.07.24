using System.Net;

namespace HomeworkGB9.Abstractions
{
    public interface IMessageSource
    {
        Task<Message?> ReceiveAsync(CancellationToken token, MemberBuilder? builder = null);
        Task SendAsync(Message message, CancellationToken token, IPEndPoint? endPoint = null);
    }
}
