using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace fw_monitor
{
    public static class ListConfigRepository
    {
        public static Dictionary<string, ListConfig> Repository { get; private set; } = new Dictionary<string, ListConfig>();

        static ListConfigRepository()
        {
            fillDefaults();
        }

        public static ListConfig Get(string listName)
        {
            Repository.TryGetValue(listName, out ListConfig retVal);
            return retVal;
        }
        
        public static void Serialize()
        {
            throw new NotImplementedException("Nog niet");
        }

        public static string Deserialize()
        {
            throw new NotImplementedException("Nog niet");
        }

        private static void fillDefaults()
        {
            ListConfig nwConfig = new ListConfig()
            {
                Name = "emergingthreats",
                Description = "Emerging Threats combined blocklist",
                URL = new Uri(@"https://rules.emergingthreats.net/fwrules/emerging-Block-IPs.txt"),
                IsComposite = true,
                IsRevisioned = true,
                LineSeparator = Environment.NewLine,
                RevisionRegex = new Regex(@"^# Rev (\d*)$"),
                SubsetHeader = new Regex(@"^#.*$"),
                SubsetSeparator = "#",
            };

            Repository[nwConfig.Name] = nwConfig;
            
            nwConfig = new ListConfig()
            {
                Name = "locallist",
                Description = "Locally hosted list for testing",
                URL = new Uri(@"http://localhost/emerging_threats.txt"),
                IsComposite = true,
                IsRevisioned = true,
                LineSeparator = Environment.NewLine,
                RevisionRegex = null,
                SubsetSeparator = "#",
            };

            Repository[nwConfig.Name] = nwConfig;

        }

    }

    public class ListConfig
    {
        public ListConfig()
        {
            
        }
        public ListConfig(string name, Uri url = null)
        {
            Name = name;
            URL = url;
        }
        
        public string Name { get; set; }
        public string Description { get; set; }
        public Uri URL { get; set; }
        public string URL_Str => URL.ToString();
        public bool IsComposite { get; set; } = false;
        public bool IsRevisioned { get; set; } = false;
        public Regex RevisionRegex { get; set; }
        public Regex SubsetHeader { get; set; }
        public string SubsetSeparator { get; set; } = "#";
        public string LineSeparator { get; set; } = Environment.NewLine;

    }
}