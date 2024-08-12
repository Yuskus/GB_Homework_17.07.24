using ChatObjectsLibrary;
using MessagesSourceLibrary;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace MessagesSourceUDPLibrary
{
    /* Доработайте чат, заменив UDP-сокеты на NetMQ. 
     * Для этого напишите новую библиотеку, в которой 
     * имплементируется IMessageSource и IMessageSourceClient 
     * с применением указанной библиотеки. */

    public class MessagesSourceUDPClient : IMessagesSource<IPEndPoint>
    {
        private readonly UdpClient _udpClient;
        public MessagesSourceUDPClient(IPEndPoint server, IPEndPoint client)
        {
            _udpClient = new UdpClient(client);
            _udpClient.Connect(server);
        }
        public async Task<Message?> ReceiveAsync(CancellationToken token, MemberBuilder<IPEndPoint>? member = null)
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
