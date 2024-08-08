using HomeworkGB9.Model;
using HomeworkGB9;

namespace HomeworkGB9Tests
{
    [TestClass]
    public class ClientTest
    {
        [TestInitialize]
        public void SetUp()
        {
            using var ctx = new ChatDbContext();
            ctx.Messages.RemoveRange(ctx.Messages);
            ctx.Users.RemoveRange(ctx.Users);
            ctx.SaveChanges();
        }

        [TestMethod]
        public async Task Hello()
        {
            await Task.Delay(1);
            var a = 2;
            Assert.AreEqual(2, a);
        }

        [TestMethod]
        [ExpectedException(typeof(TaskCanceledException))]
        public async Task TestReceive()
        {
            await Task.Delay(1);
            var mock = new MockMessageSourceClient();
            var client = new ChatClient("Test", mock);
            using var cts = new CancellationTokenSource();
            Task task = client.ReceiveMessagesAsync(cts.Token);
            //Assert will be here
            await Task.Delay(1000);
            cts.Cancel();
            //it throws an exception here
            await task;
        }

        [TestCleanup]
        public void TearDown()
        {
            using var ctx = new ChatDbContext();
            ctx.Messages.RemoveRange(ctx.Messages);
            ctx.Users.RemoveRange(ctx.Users);
            ctx.SaveChanges();
        }
    }
}
