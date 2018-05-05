using System.Collections;
using System.Collections.Generic;

namespace fw_monitor
{
    public interface IExecutor
    {
        IConnector Connector { get; set; }
        bool AddElement(string element);
        bool AddElements(IEnumerable<string>elements);
    }
}