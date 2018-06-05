using System;
using System.Collections.Generic;
using System.Linq;

namespace fw_monitor
{
    public class FeedbackProvider : IFeedbackProvider
    {
        public FeedbackProvider(){}
        public FeedbackProvider(string owner) => Owner = owner;
        protected static string UnspecifiedOwner = "UNSPECIFIED";
        public string Owner { get; set; } = UnspecifiedOwner;
        public IEnumerable<string> Errors { get; } = new List<string>();
        public IEnumerable<string> Output { get; } = new List<string>();
        public string LastError => Errors.LastOrDefault();
        public string LastOutput => Output.LastOrDefault();
        public event Action<IFeedbackProvider, string> ErrorAdded;
        public event Action<IFeedbackProvider, string> OutputAdded;
        
        public void AddError(string msg)
        {
            (Errors as List<string>)?.Add(msg);
            ErrorAdded?.Invoke(this, msg);
        }

        public void AddOutput(string msg)
        {
            (Output as List<string>)?.Add(msg);
            OutputAdded?.Invoke(this, msg);
        }
    }
}