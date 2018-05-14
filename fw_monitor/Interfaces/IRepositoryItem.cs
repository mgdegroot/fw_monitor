namespace fw_monitor
{
    public interface IRepositoryItem
    {
        string Name { get; set; }

        ICreator Creator { get; set; }

    }
}