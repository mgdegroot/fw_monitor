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

        public static ListConfig ReadFromSTDIN()
        {
            ListConfig listConfig = new ListConfig();

            listConfig.Name = ConsoleHelper.readInput("name");
            listConfig.Description = ConsoleHelper.readInput("description");
            listConfig.URL = new Uri(ConsoleHelper.readInput("URL"));
            listConfig.IsComposite = ConsoleHelper.readInputAsBool("contains sublists (y/n)");
            if (listConfig.IsComposite)
            {
                listConfig.SubsetHeader = new Regex(ConsoleHelper.readInput("regex for subset name"));
            }
            listConfig.IsRevisioned = ConsoleHelper.readInputAsBool("is versioned (y/n");
            if (listConfig.IsRevisioned)
            {
                listConfig.RevisionRegex = new Regex(ConsoleHelper.readInput("regex for version number"));
            }

            return listConfig;
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
                SubsetHeader = new Regex(@"^#\s*(\w*)\s*$"),
//                SubsetSeparator = "#",
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
                RevisionRegex = new Regex(@"^# Rev (\d*)$"),
                SubsetHeader =  new Regex(@"^#\s*(\w*)\s*$"),
//                SubsetSeparator = "#",
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
        public bool IsComposite { get; set; } = false;
        public bool IsRevisioned { get; set; } = false;
        public Regex RevisionRegex { get; set; }
        public Regex SubsetHeader { get; set; }
        public Regex InvalidListnameChars { get; set; } = new Regex(@"[^A-Za-z0-9\-_]");

        public string InvalidCharReplacement { get; set; } = "_";
//        public string SubsetSeparator { get; set; } = "#";
        public string LineSeparator { get; set; } = Environment.NewLine;

    }
}