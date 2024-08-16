using ChatObjectsLibrary;
using MessagesSourceLibrary;
using NetMQ;
using NetMQ.Sockets;

namespace MessagesSourceNetMQLibrary
{
    public class MessagesSourceNetMQClient : IMessagesSource<string>
    {
        private readonly string _remoteRouterAdress;
        private readonly string _remotePublisherAdress;
        private readonly DealerSocket _dealerSocket;
        private readonly SubscriberSocket _subscriberSocket;
        public MessagesSourceNetMQClient(string name, int routerPort, int publisherPort)
        {
            _remoteRouterAdress = $"tcp://127.0.0.1:{routerPort}";
            _remotePublisherAdress = $"tcp://127.0.0.1:{publisherPort}";
            _dealerSocket = new DealerSocket();
            _dealerSocket.Connect(_remoteRouterAdress);
            _subscriberSocket = new SubscriberSocket();
            _subscriberSocket.Connect(_remotePublisherAdress);
            _subscriberSocket.Subscribe(name);
        }
        public async Task<Message?> ReceiveAsync(CancellationToken token, MemberBuilder<string>? builder = null)
        {
            var _ = await Task.Run(_subscriberSocket.ReceiveFrameString, token);
            var json = await Task.Run(_subscriberSocket.ReceiveFrameString, token);
            return Message.ToMessage(json);
        }

        public async Task SendAsync(Message message, CancellationToken token, string? endPoint = null)
        {
            await Task.Run(() => _dealerSocket.SendFrame(message.ToJson()), token);
        }
    }
}
