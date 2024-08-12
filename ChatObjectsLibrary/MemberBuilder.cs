namespace ChatObjectsLibrary
{
    public class MemberBuilder<T> : IMemberBuilder<T> where T : class
    {
        private readonly Member<T> member = new();
        public void BuildName(string name) => member.Name = name;
        public void BuildEndPoint(T endPoint) => member.EndPoint = endPoint;
        public Member<T> GetMember() => member;
    }
}
