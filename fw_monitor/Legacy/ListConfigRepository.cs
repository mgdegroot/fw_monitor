//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Runtime.Serialization;
//using System.Runtime.Serialization.Json;
//using System.Text;
//using System.Text.RegularExpressions;
//using fw_monitor.DataObjects;
//
//namespace fw_monitor
//{
//    public class ListConfigRepository : Repository, IRepository
//    {
//        
//        public override ICreator Creator { get; set; } = new ListConfigFromStdInCreator();
//
//        public ListConfigRepository()
//        {
//            filenamePrefix = "listconfig";
//        }
//        
//        public ListConfigRepository(IUtil util) : this()
//        {
//            this._util = util;
//        }
//        
//        public override IRepositoryItem this[string key]
//        {
//            get => Get(key);
//            set => Set(value);
//        }
//        
//        public override IRepositoryItem Get(string name)
//        {
//            repository.TryGetValue(name, out Config config);
//            
//            if (config == null)
//            {
//                string path = getFilename(name);
//                
//                if (File.Exists(path))
//                {
//                    string strConf = readFromFile(path);
//                    config = deserialize(strConf);
//                    repository.Add(name, config);
//                }
//                else
//                {
//                    return null;
//                }
//            }
//            
//            return config;
//        }
//        
//        public override void Set(IRepositoryItem item)
//        {
//            if (item is ListConfig listConfig)
//            {
//                repository[listConfig.Name] = listConfig;
//            
//                if (SerializeToFile)
//                {
//                    string strConf = serialize(listConfig as ListConfig);
//                    writeToFile(getFilename(listConfig.Name), strConf);
//                }
//            }
//        }
//        
//        private string serialize(ListConfig listConfig)
//        {
//            MemoryStream memoryStream = new MemoryStream();
//            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(ListConfig));
//            try
//            {
//                serializer.WriteObject(memoryStream, listConfig);
//            }
//            catch (Exception ex)
//            {
//                return null;
//            }
//            
//            byte[] json = memoryStream.ToArray();
//            memoryStream.Close();
//            return Encoding.UTF8.GetString(json, 0, json.Length);
//        }
//
//        private ListConfig deserialize(string json)
//        {
//            MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json));
//            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(ListConfig));
//            ListConfig listConfig = (ListConfig)serializer.ReadObject(memoryStream) as ListConfig;
//
//            return listConfig;
//        }
//        
//        private void loadRepoFromRepoDir()
//        {
//            
//        }
//    }
//}