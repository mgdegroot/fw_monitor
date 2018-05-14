namespace fw_monitor
{
    public interface IRepository<T>
    {
        T this[string index] { get; set; }

        bool SerializeToFile { get; set; }
        string SerializePath { get; set; }
        ICreator Creator { get; set; }

        T Get(string name);
        void Set(T item);
    }
}