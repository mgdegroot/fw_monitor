using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using fw_monitor.DataObjects;

namespace fw_monitor
{
    public class ListConfigRepository : RepositoryBase, IRepository
    {
//        private static ListConfigRepository _instance = null;
//        public override IRepository Instance
//        {
//            get
//            {
//                if (_instance == null)
//                {
//                    _instance = new ListConfigRepository();
//                }
//
//                return _instance;
//            }
//        }

        public override Dictionary<string, Config> Repository { get; set; } = new Dictionary<string, Config>();

//        private ListConfigRepository()
//        {
//            loadRepoFromRepoDir();
//        }

        public override Config Get(string listName)
        {
            Repository.TryGetValue(listName, out Config retVal);
            return retVal;
        }

        public override void Set(Config listConfig)
        {
            Repository[listConfig.Name] = listConfig;
            
            string strConf = serialize(listConfig as ListConfig);
            writeToFile(Path.Combine(REPO_BASE_PATH, listConfig.Name + REPO_FILE_EXTENSION), strConf);
        }

        public override Config CreateNew(string name)
        {
            return readFromSTDIN(name);
        }

        private string serialize(ListConfig listConfig)
        {
            // serializing here so set 'NewEntry' to false -->
            listConfig.Empty = false;
            MemoryStream memoryStream = new MemoryStream();
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(ListConfig));
            serializer.WriteObject(memoryStream, listConfig);

            byte[] json = memoryStream.ToArray();
            memoryStream.Close();
            return Encoding.UTF8.GetString(json, 0, json.Length);
            throw new NotImplementedException("Nog niet");
        }

        private ListConfig deserialize(string json)
        {
            MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(ListConfig));
            ListConfig listConfig = (ListConfig)serializer.ReadObject(memoryStream) as ListConfig;

            return listConfig;
        }
        
        private static ListConfig readFromSTDIN(string name=null)
        {
            ListConfig listConfig = new ListConfig();

            listConfig.Name = ConsoleHelper.ReadInput("name", name);
            listConfig.Description = ConsoleHelper.ReadInput("description");
            listConfig.URL = new Uri(ConsoleHelper.ReadInput("URL"));
            listConfig.IsComposite = ConsoleHelper.ReadInputAsBool("contains sublists (y/n)");
            if (listConfig.IsComposite)
            {
                listConfig.SubsetHeader = new Regex(ConsoleHelper.ReadInput("regex for subset name"));
            }
            listConfig.IsRevisioned = ConsoleHelper.ReadInputAsBool("is versioned (y/n");
            if (listConfig.IsRevisioned)
            {
                listConfig.RevisionRegex = new Regex(ConsoleHelper.ReadInput("regex for version number"));
            }

            return listConfig;
        }

        private void loadRepoFromRepoDir()
        {
            
        }

//        private static void fillDefaults()
//        {
//            ListConfig nwConfig = new ListConfig()
//            {
//                Name = "emergingthreats",
//                Description = "Emerging Threats combined blocklist",
//                URL = new Uri(@"https://rules.emergingthreats.net/fwrules/emerging-Block-IPs.txt"),
//                IsComposite = true,
//                IsRevisioned = true,
//                LineSeparator = Environment.NewLine,
//                RevisionRegex = new Regex(@"^# Rev (\d*)$"),
//                SubsetHeader = new Regex(@"^#\s*(\w*)\s*$"),
////                SubsetSeparator = "#",
//            };
//
//            Repository[nwConfig.Name] = nwConfig;
//            
//            nwConfig = new ListConfig()
//            {
//                Name = "locallist",
//                Description = "Locally hosted list for testing",
//                URL = new Uri(@"http://localhost/emerging_threats.txt"),
//                IsComposite = true,
//                IsRevisioned = true,
//                LineSeparator = Environment.NewLine,
//                RevisionRegex = new Regex(@"^# Rev (\d*)$"),
//                SubsetHeader =  new Regex(@"^#\s*(\w*)\s*$"),
////                SubsetSeparator = "#",
//            };
//
//            Repository[nwConfig.Name] = nwConfig;
//
//        }

    }

    
}