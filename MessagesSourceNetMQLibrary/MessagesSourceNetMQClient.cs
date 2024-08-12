using ChatObjectsLibrary;
using MessagesSourceLibrary;
using NetMQ;
using NetMQ.Sockets;

namespace MessagesSourceNetMQLibrary
{
    public class MessagesSourceNetMQClient : IMessagesSource<string>
    {
        private readonly string _remoteAdress;
        private readonly DealerSocket _socket;
        public MessagesSourceNetMQClient(string server)
        {
            _remoteAdress = server;
            _socket = new DealerSocket();
            _socket.Connect(_remoteAdress);
        }
        public async Task<Message?> ReceiveAsync(CancellationToken token, MemberBuilder<string>? builder = null)
        {
            var request = await _socket.ReceiveFrameStringAsync();
            return Message.ToMessage(request.Item1);
        }

        public async Task SendAsync(Message message, CancellationToken token, string? endPoint = null)
        {
            await Task.CompletedTask;
            _socket.SendFrame(message.ToJson());
        }
    }
}
