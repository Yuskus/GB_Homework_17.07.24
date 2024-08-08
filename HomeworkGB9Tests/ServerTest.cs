using HomeworkGB9;
using HomeworkGB9.Model;

namespace HomeworkGB9Tests
{
    [TestClass]
    public class ServerTest
    {
        [TestInitialize]
        [Ignore]
        public void SetUp()
        {
            using var ctx = new ChatDbContext();
            ctx.Messages.RemoveRange(ctx.Messages);
            ctx.Users.RemoveRange(ctx.Users);
            ctx.SaveChanges();
        }

        [TestMethod]
        [Ignore]
        public async Task TestReceive()
        {
            var mock = new MockMessageSourceServer(); // ������� ������ ���� ��� ������������
            var server = new ChatServer(mock); // ������� ������ � �������� ��� ���
            mock.AddServer(server);
            using (var cts = new CancellationTokenSource())
            {
                try
                {
                    var test = server.ReceiveMessagesAsync(cts.Token);
                    await Task.Delay(1000);
                    cts.Cancel();
                    await test;
                }
                catch (OperationCanceledException) { }
            }
            
            using var ctx = new ChatDbContext();

            Assert.IsTrue(ctx.Users.Count() == 2, "������������ �� �������.");

            var user1 = ctx.Users.FirstOrDefault(x => x.Name == "�����");
            var user2 = ctx.Users.FirstOrDefault(x => x.Name == "�������");

            Assert.IsNotNull(user1, "������������ �� ������.");
            Assert.IsNotNull(user2, "������������ �� ������.");

            Assert.IsTrue(user1.FromMessages.Count == 2, "���������� ���������: " + user1.FromMessages.Count);
            Assert.IsTrue(user2.FromMessages.Count == 2, "���������� ���������: " + user1.FromMessages.Count);

            Assert.IsTrue(user1.ToMessages.Count == 2, "���������� ���������: " + user1.ToMessages.Count);
            Assert.IsTrue(user2.ToMessages.Count == 2, "���������� ���������: " + user1.ToMessages.Count);

            var fromFedor = ctx.Messages.Where(x => x.Sender == user1 && x.Recipient == user2).ToList();
            var fromEugenia = ctx.Messages.Where(x => x.Recipient == user1 && x.Sender == user2).ToList();

            Assert.AreEqual(2, fromFedor.Count);
            Assert.AreEqual(2, fromEugenia.Count);

            Assert.AreEqual("������! � �����.", fromFedor[0]?.Text, $"���������� ���������: {fromFedor[0]?.Text}");
            Assert.AreEqual("������, �����! � �������.", fromEugenia[0]?.Text, $"���������� ���������: {fromEugenia[0]?.Text}");
            Assert.AreEqual("������� �������������, �������.", fromFedor[1]?.Text, $"���������� ���������: {fromFedor[1]?.Text}");
            Assert.AreEqual("�������, �����.", fromEugenia[1]?.Text, $"���������� ���������: {fromEugenia[1]?.Text}");
        }

        [TestCleanup]
        [Ignore]
        public void TearDown()
        {
            using var ctx = new ChatDbContext();
            ctx.Messages.RemoveRange(ctx.Messages);
            ctx.Users.RemoveRange(ctx.Users);
            ctx.SaveChanges();
        }
    }
}