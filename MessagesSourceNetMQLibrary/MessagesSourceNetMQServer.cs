using ChatObjectsLibrary;
using MessagesSourceLibrary;
using NetMQ;
using NetMQ.Sockets;
using System.Text;

namespace MessagesSourceNetMQLibrary
{
    public class MessagesSourceNetMQServer : IMessagesSource<string>
    {
        private readonly int _routerPort;
        private readonly int _publisherPort;
        private readonly RouterSocket _routerSocket;
        private readonly PublisherSocket _publisherSocket;
        public MessagesSourceNetMQServer(int routerPort, int publisherPort)
        {
            _routerPort = routerPort;
            _publisherPort = publisherPort;
            _routerSocket = new RouterSocket();
            _routerSocket.Bind($"tcp://*:{_routerPort}");
            _publisherSocket = new PublisherSocket();
            _publisherSocket.Bind($"tcp://*:{_publisherPort}");
        }
        public async Task<Message?> ReceiveAsync(CancellationToken token, MemberBuilder<string>? builder)
        {
            var multyMessage = await _routerSocket.ReceiveMultipartMessageAsync();
            string json = multyMessage.Last.ConvertToString(Encoding.UTF8);
            var message = Message.ToMessage(json);
            if (message == null) { return null; }
            builder?.BuildEndPoint(message.FromName);
            return message;
        }
        public async Task SendAsync(Message message, CancellationToken token, string? endPoint)
        {
            await Task.CompletedTask;
            _publisherSocket.SendMoreFrame(endPoint!).SendFrame(message.ToJson());
        }
    }
}
