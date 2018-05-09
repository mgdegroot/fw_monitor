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
    public class ListConfigRepository : Repository, IRepository
    {

        public ListConfigRepository()
        {
            filenamePrefix = "listconfig";
        }
        
        public override Config this[string key]
        {
            get => Get(key);
            set => Set(value);
        }
        
        public override Config Get(string name)
        {
            repository.TryGetValue(name, out Config config);
            
            if (config == null)
            {
                string path = getFilename(name);
                
                if (File.Exists(path))
                {
                    string strConf = readFromFile(path);
                    config = deserialize(strConf);
                    repository.Add(name, config);
                }
                else
                {
                    return null;
                }
            }
            
            return config;
        }
        
        public override void Set(Config listConfig)
        {
            repository[listConfig.Name] = listConfig;
            
            if (SerializeToFile)
            {
                string strConf = serialize(listConfig as ListConfig);
                writeToFile(getFilename(listConfig.Name), strConf);
            }
        }
        
        public override Config Create(string name)
        {
            return readFromSTDIN(name);
        }
        
        private string serialize(ListConfig listConfig)
        {
            MemoryStream memoryStream = new MemoryStream();
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(ListConfig));
            try
            {
                serializer.WriteObject(memoryStream, listConfig);
            }
            catch (Exception ex)
            {
                return null;
            }
            
            byte[] json = memoryStream.ToArray();
            memoryStream.Close();
            return Encoding.UTF8.GetString(json, 0, json.Length);
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
//                SubsetHeader = new Regex(@"^#\s*(.*)\s*$"),
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
//                SubsetHeader =  new Regex(@"^#\s*(.*)\s*$"),
////                SubsetSeparator = "#",
//            };
//
//            Repository[nwConfig.Name] = nwConfig;
//
//        }

    }

    
}