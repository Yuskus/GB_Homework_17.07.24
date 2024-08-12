namespace ChatObjectsLibrary
{
    public class Member<T> where T : class
    {
        public string Name { get; set; } = "";
        public T? EndPoint { get; set; }
        public override string ToString() => $"[ {Name} : {EndPoint} ]";
    }
}
