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
    public class HostConfigRepository : Repository, IRepository
    {
        public HostConfigRepository()
        {
            filenamePrefix = "host";
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

        public override void Set(Config hostConfig)
        {
            repository[hostConfig.Name] = hostConfig;

            if (SerializeToFile)
            {
                string strConf = serialize(hostConfig as HostConfig);
                writeToFile(getFilename(hostConfig.Name), strConf);
            }
        }

        public override Config CreateNew(string name)
        {
            return readFromSTDIN(name);
        }
        
        private string serialize(HostConfig hostConfig)
        {
            MemoryStream memoryStream = new MemoryStream();
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(HostConfig));
            serializer.WriteObject(memoryStream, hostConfig);

            byte[] json = memoryStream.ToArray();
            memoryStream.Close();
            return Encoding.UTF8.GetString(json, 0, json.Length);
        }

        private HostConfig deserialize(string json)
        {
            
            MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(HostConfig));
            HostConfig hostConfig = (HostConfig)serializer.ReadObject(memoryStream) as HostConfig;

            return hostConfig;
        }
        
        private static HostConfig readFromSTDIN(string name=null)
        {
            HostConfig hostConfig = new HostConfig();
            hostConfig.Name = ConsoleHelper.ReadInput("hostname", hostConfig.Name);
            hostConfig.HostIP = ConsoleHelper.ReadInput("host ip", hostConfig.HostIP);
            hostConfig.UserName = ConsoleHelper.ReadInput("username", hostConfig.UserName);
            hostConfig.Password = ConsoleHelper.ReadInput("password", hostConfig.Password);
            hostConfig.CertPath = ConsoleHelper.ReadInput("certificate path", hostConfig.CertPath);
            hostConfig.TableName = ConsoleHelper.ReadInput("table name", hostConfig.TableName);
            hostConfig.ChainName = ConsoleHelper.ReadInput("chain name", hostConfig.ChainName);
            hostConfig.FlushChain = ConsoleHelper.ReadInputAsBool("flush chain", hostConfig.FlushChain ? "y" : "n");
            hostConfig.SetName = ConsoleHelper.ReadInput("set name", hostConfig.SetName);
            hostConfig.SupportsFlush = ConsoleHelper.ReadInputAsBool("supports flush", hostConfig.SupportsFlush ? "y" : "n");
            
            hostConfig.UsePubkeyLogin = String.IsNullOrEmpty(hostConfig.CertPath) == false;

            return hostConfig;
        }
    }

}