using System.Collections.Generic;
using fw_monitor.DataObjects;

namespace fw_monitor
{
    public interface IRepository
    {
        Config this[string index] { get; set; }
        Config Get(string name);
        void Set(Config item);
        Config Create(string name);
        bool SerializeToFile { get; set; }
        string SerializePath { get; set; }
    }
}