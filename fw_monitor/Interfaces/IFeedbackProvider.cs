using System;
using System.Collections.Generic;

namespace fw_monitor
{
    public interface IFeedbackProvider
    {
        string Owner { get; set; }
        IEnumerable<string> Errors { get; }
        IEnumerable<string> Output { get; }
        string LastError { get; }
        string LastOutput { get; }

        event Action<IFeedbackProvider, string> ErrorAdded;
        event Action<IFeedbackProvider, string> OutputAdded;

        void AddError(string msg);
        void AddOutput(string msg);
    }
}