using System;
using System.Collections;
using System.Collections.Generic;
using fw_monitor.DataObjects;

namespace fw_monitor
{
    public interface IExecutor : IOutputProvider
    {
        IConnector Connector { get; set; }
        // TODO: Interfaces or concrete ones here -->
        HostConfig HostConfig { get; set; }
        ListConfig ListConfig { get; set; }
        bool ProcessList(string name, IEnumerable<string> elements);
        bool AddElement(string set, string element);
        bool AddElements(string set, IEnumerable<string>elements);
        bool DoPreActions();
        bool DoPostActions();
        
//        IEnumerable<string>Errors { get; }
//        IEnumerable<string>Output { get; }
//        
//        string LastError { get; }
//        string LastOutput { get; }
//
//        event EventHandler ErrorAdded;
//        event EventHandler OutputAdded;
//
//        void OnErrorAdded(EventArgs e);
//        void OnOutputAdded(EventArgs e);
    }
}