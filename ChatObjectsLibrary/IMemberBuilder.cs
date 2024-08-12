namespace ChatObjectsLibrary
{
    public interface IMemberBuilder<T> where T : class
    {
        void BuildName(string name);
        void BuildEndPoint(T endPoint);
        Member<T> GetMember();
    }
}
