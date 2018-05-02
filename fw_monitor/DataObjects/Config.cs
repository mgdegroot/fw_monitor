namespace fw_monitor.DataObjects
{
    public abstract class Config : IConfig
    {
        public abstract string Name { get; set; }
        public virtual string Description { get; set; } = string.Empty;

    }
}