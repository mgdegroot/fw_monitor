using System;
using System.Collections.Generic;
using fw_monitor.DataObjects;
using Renci.SshNet;

namespace fw_monitor
{
    public interface IConnector
    {
        HostConfig HostConfig { get; set; }
        IFeedbackProvider Feedback { get; set; }
        
        void Connect();
        bool ExecuteCommand(string command);
        IEnumerable<bool> ExecuteCommands(IEnumerable<string> commands);

        (bool, string) ExecuteQuery(string query);
        IEnumerable<(bool, string)> ExecuteQueries(IEnumerable<string> queries);
    }
}