using System.Net;
using System.Net.Sockets;
using System.Text;

namespace HomeworkGB9.Abstractions
{
    public class MessageSourceClient : IMessageSource
    {
        private readonly IPEndPoint _localEndPoint;
        private readonly IPEndPoint _remoteEndPoint;
        private readonly UdpClient _udpClient;
        public MessageSourceClient(int port)
        {
            _localEndPoint = Chat.GetClientEndPoint(port);
            _remoteEndPoint = Chat.GetServerEndPoint();
            _udpClient = new UdpClient(_localEndPoint);
            _udpClient.Connect(_remoteEndPoint);
        }
        public async Task<Message?> ReceiveAsync(CancellationToken token, MemberBuilder? member = null)
        {
            var data = await _udpClient.ReceiveAsync(token);
            string json = Encoding.UTF8.GetString(data.Buffer);
            return Message.ToMessage(json);
        }

        public async Task SendAsync(Message message, CancellationToken token, IPEndPoint? endPoint = null)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(message.ToJson());
            await _udpClient.SendAsync(buffer, token);
        }
    }
}
