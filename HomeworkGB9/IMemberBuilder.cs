using System.Net;

namespace HomeworkGB9
{
    internal interface IMemberBuilder
    {
        void BuildName(string name);
        void BuildEndPoint(IPEndPoint endPoint);
        Member GetMember();
    }
}
