using System.Net;
using System.Net.Sockets;
using System.Text;

namespace HomeworkGB9.Abstractions
{
    public class MessageSourceServer : IMessageSource
    {
        private readonly UdpClient _udpClient;
        public MessageSourceServer() 
        {
            _udpClient = new();
        }
        public MessageSourceServer(int port)
        {
            _udpClient = new(port);
        }

        public async Task<Message?> ReceiveAsync(CancellationToken token, MemberBuilder? builder)
        {
            var data = await _udpClient.ReceiveAsync(token);
            builder?.BuildEndPoint(data.RemoteEndPoint);
            string json = Encoding.UTF8.GetString(data.Buffer);
            return Message.ToMessage(json);
        }

        public async Task SendAsync(Message message, CancellationToken token, IPEndPoint? endPoint)
        {
            string json = message.ToJson();
            byte[] buffer = Encoding.UTF8.GetBytes(json);
            await _udpClient.SendAsync(buffer, endPoint!, token);
        }
    }
}
