using System;
using System.Collections;
using System.Collections.Generic;
using fw_monitor.DataObjects;

namespace fw_monitor
{
    public interface IExecutor
    {
        IConnector Connector { get; set; }
        // TODO: Interfaces or concrete ones here -->
        HostConfig HostConfig { get; set; }
        ListConfig ListConfig { get; set; }
        IFeedbackProvider Feedback { get; set; }
        bool ProcessList(string name, IEnumerable<string> elements);
        bool AddElement(string set, string element);
        bool AddElements(string set, IEnumerable<string>elements);
        bool DoPreActions();
        bool DoPostActions();
    }
}