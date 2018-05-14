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
//    public class HostConfigRepository : Repository, IRepository
//    {
//        public override ICreator Creator { get; set; } = new HostConfigFromStdInCreator();
//        
//        public HostConfigRepository() : base()
//        {
//            filenamePrefix = "hostconfig";
//        }
//
//        public HostConfigRepository(IUtil util) : this()
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
//            return config as IRepositoryItem;
//        }
//
//        public override void Set(IRepositoryItem item)
//        {
//            if (item is HostConfig hostConfig)
//            {
//                repository[hostConfig.Name] = hostConfig;
//
//                if (SerializeToFile)
//                {
//                    string strConf = serialize(hostConfig);
//                    writeToFile(getFilename(hostConfig.Name), strConf);
//                }    
//            }
//        }
//
//        private string serialize(HostConfig hostConfig)
//        {
//            MemoryStream memoryStream = new MemoryStream();
//            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(HostConfig));
//            serializer.WriteObject(memoryStream, hostConfig);
//
//            byte[] json = memoryStream.ToArray();
//            memoryStream.Close();
//            return Encoding.UTF8.GetString(json, 0, json.Length);
//        }
//
//        private HostConfig deserialize(string json)
//        {
//            
//            MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json));
//            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(HostConfig));
//            HostConfig hostConfig = (HostConfig)serializer.ReadObject(memoryStream) as HostConfig;
//
//            return hostConfig;
//        }
//    }
//}