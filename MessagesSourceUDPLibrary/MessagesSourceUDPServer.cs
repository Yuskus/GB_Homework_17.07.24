using MessagesSourceLibrary;
using ChatObjectsLibrary;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace MessagesSourceUDPLibrary
{
    public class MessagesSourceUDPServer(int port) : IMessagesSource<IPEndPoint>
    {
        private readonly UdpClient _udpClient = new(port);
        public async Task<Message?> ReceiveAsync(CancellationToken token, MemberBuilder<IPEndPoint>? builder)
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
