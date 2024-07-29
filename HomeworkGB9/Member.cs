using System.Net;

namespace HomeworkGB9
{
    public class Member
    {
        public string Name { get; set; } = "Unknown";
        public IPEndPoint? EndPoint { get; set; }
        public override string ToString()
        {
            return $"[ {Name} : {EndPoint} ]";
        }
    }
}
