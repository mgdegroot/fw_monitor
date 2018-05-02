using System;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace fw_monitor.DataObjects
{
    [DataContract]
    public class ListConfig : Config
    {
        public ListConfig()
        {
            
        }
        public ListConfig(string name, Uri url = null)
        {
            Name = name;
            URL = url;
        }
        
        [DataMember(Order = 0)] public bool Empty { get; set; } = true;
        
        [DataMember(Order = 1)] public override string Name { get; set; }
        [DataMember(Order = 3)] public Uri URL { get; set; }
        [DataMember(Order = 4)] public bool IsComposite { get; set; } = false;
        [DataMember(Order = 5)] public bool IsRevisioned { get; set; } = false;
        [DataMember(Order = 6)] public Regex RevisionRegex { get; set; }
        [DataMember(Order = 7)] public Regex SubsetHeader { get; set; }
        [DataMember(Order = 8)] public Regex InvalidListnameChars { get; set; } = new Regex(@"[^A-Za-z0-9\-_]");

        [DataMember(Order = 9)] public string InvalidCharReplacement { get; set; } = "_";
//        public string SubsetSeparator { get; set; } = "#";
        [DataMember(Order = 10)] public string LineSeparator { get; set; } = Environment.NewLine;

    }
}