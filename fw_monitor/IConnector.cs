using System;
using System.Collections.Generic;
using fw_monitor.DataObjects;
using Renci.SshNet;

namespace fw_monitor
{
    public interface IConnector
    {
        void Connect();
        bool ExecuteCommand(string command);
        bool[] ExecuteCommands(IEnumerable<string> commands);
        
        HostConfig HostConfig { get; set; }
        
        IEnumerable<string>Errors { get; }
        IEnumerable<string>Output { get; }
        
        string LastError { get; }
        string LastOutput { get; }

        event EventHandler ErrorAdded;
        event EventHandler OutputAdded;

        void OnErrorAdded(EventArgs e);
        void OnOutputAdded(EventArgs e);
    }
}