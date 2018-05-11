using System;
using System.Collections.Generic;
using fw_monitor.DataObjects;

namespace fw_monitor
{
    public interface IRepository
    {
        IRepositoryItem this[string index] { get; set; }

        bool SerializeToFile { get; set; }
        string SerializePath { get; set; }
        ICreator Creator { get; set; }

        IRepositoryItem Get(string name);
        void Set(IRepositoryItem item);
//        IRepositoryItem Create(string name);
        
        IRepository GetInstance(Type theType);
    }
}