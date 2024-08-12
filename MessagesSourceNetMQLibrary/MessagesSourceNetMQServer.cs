using ChatObjectsLibrary;
using MessagesSourceLibrary;
using NetMQ;
using NetMQ.Sockets;
using System.Linq;
using System.Text;

namespace MessagesSourceNetMQLibrary
{
    public class MessagesSourceNetMQServer : IMessagesSource<string>
    {
        private readonly int _port;
        private readonly RouterSocket _socket;
        private NetMQFrame? _frame;
        public MessagesSourceNetMQServer(int port)
        {
            _port = port;
            _socket = new RouterSocket();
            _socket.Bind($"tcp://*:{_port}");
        }
        public async Task<Message?> ReceiveAsync(CancellationToken token, MemberBuilder<string>? builder)
        {
            var a = await _socket.ReceiveMultipartMessageAsync();
            _frame = a.First;
            string json = a.Last.ConvertToString(Encoding.UTF8);
            return Message.ToMessage(json);
        }
        public async Task SendAsync(Message message, CancellationToken token, string? endPoint)
        {
            await Task.CompletedTask;
            var request = new NetMQMessage();
            request.Append(_frame!);
            request.Append(message.ToJson());
            _socket.SendMultipartMessage(request);
        }
    }
}
