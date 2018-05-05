using System.Collections.Generic;
using fw_monitor.DataObjects;

namespace fw_monitor
{
    public class NFTablesExecutor : IExecutor
    {
        public IConnector Connector { get; set; }
        public HostConfig HostConfig { get; set; }
        public ListConfig ListConfig { get; set; }
        
        public bool AddElement(string element)
        {
            string cmd = "";
            return Connector.ExecuteCommand(cmd);

        }

        public bool AddElements(IEnumerable<string> elements)
        {
            throw new System.NotImplementedException();
        }
    }
}