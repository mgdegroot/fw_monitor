using System.Collections.Generic;
using fw_monitor.DataObjects;

namespace fw_monitor
{
    public interface IListFetcher
    {
        ListConfig ListConfig { get; set; }
        
        IList<string> this[string index] { get; set; }
        IList<string> Get(string name);
        void Set(string name, IList<string> item);
    }
}