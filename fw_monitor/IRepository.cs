using System.Collections.Generic;
using fw_monitor.DataObjects;

namespace fw_monitor
{
    public interface IRepository
    {
//        IRepository Instance { get; }
//        Dictionary<string, Config> Repository { get; set; }
        bool SerializeToFile { get; set; }
        string SerializePath { get; set; }
        Config this[string index] { get; set; }
        Config Get(string name);
        void Set(Config item);
        
    }
}