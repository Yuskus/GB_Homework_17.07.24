using System.Net;

namespace HomeworkGB9
{
    public class MemberBuilder : IMemberBuilder
    {
        private readonly Member member = new();
        public void BuildName(string name) => member.Name = name;
        public void BuildEndPoint(IPEndPoint endPoint) => member.EndPoint = endPoint;
        public Member GetMember() => member;
    }
}
