using ChatObjectsLibrary;
using ClientServerLibrary;
using System.Net;

namespace HomeworkGB9Tests
{
    [TestClass]
    public class ClientTest
    {
        [TestMethod]
        [Ignore]
        [ExpectedException(typeof(TaskCanceledException))]
        public async Task TestReceive()
        {
            await Task.Delay(1);
            var mock = new MockMessageSourceClient();
            var client = new ChatClient<IPEndPoint>("Test", mock);
            using var cts = new CancellationTokenSource();
            Task task = client.ReceiveMessagesAsync(cts.Token);
            
            await Task.Delay(1000);

            Assert.IsNotNull(client);
            Assert.IsNotNull(task);
            Assert.IsNotNull(cts.Token);

            Assert.AreEqual(6, mock.deliveredMessages.Count);

            Assert.AreEqual(2, mock.deliveredMessages.Count(x => x.FromName == "Server"));
            Assert.AreEqual(2, mock.deliveredMessages.Count(x => x.FromName == "Федор"));
            Assert.AreEqual(2, mock.deliveredMessages.Count(x => x.FromName == "Евгения"));

            Assert.AreEqual("Федор вошел(ла) в чат.", mock.deliveredMessages.Dequeue()?.Text);
            Assert.AreEqual("Евгения вошел(ла) в чат.", mock.deliveredMessages.Dequeue()?.Text);
            Assert.AreEqual("Привет! Я Федор.", mock.deliveredMessages.Dequeue()?.Text);
            Assert.AreEqual("Привет, Федор! Я Евгения.", mock.deliveredMessages.Dequeue()?.Text);
            Assert.AreEqual("Приятно познакомиться, Евгения.", mock.deliveredMessages.Dequeue()?.Text);
            Assert.AreEqual("Взаимно, Федор.", mock.deliveredMessages.Dequeue()?.Text);

            Assert.AreEqual(6, mock.confirmMessages.Count);
            Assert.IsTrue(mock.confirmMessages.All(x => x.FromName == "Test"));
            Assert.IsTrue(mock.confirmMessages.All(x => x.Command == Command.Confirm));

            cts.Cancel(); //it throws an exception here

            await task;
        }
    }
}
