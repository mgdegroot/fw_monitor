﻿using System;
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
        public override ICreator Creator { get; set; } = new HostConfigFromStdInCreator();
        
        public HostConfigRepository() : base()
        {
            filenamePrefix = "hostconfig";
        }

        public HostConfigRepository(IUtil util) : this()
        {
            this._util = util;
        }
        public override IRepositoryItem this[string key]
        {
            get => Get(key); 
            set => Set(value);
        }
        
        public override IRepositoryItem Get(string name)
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

            return config as IRepositoryItem;
        }

        public override void Set(IRepositoryItem item)
        {
            if (item is HostConfig hostConfig)
            {
                repository[hostConfig.Name] = hostConfig;

                if (SerializeToFile)
                {
                    string strConf = serialize(hostConfig);
                    writeToFile(getFilename(hostConfig.Name), strConf);
                }    
            }
        }

//        public override IRepositoryItem Create(string name)
//        {
//            return readFromSTDIN(name);
//        }
        
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
    }

    public class HostConfigFromStdInCreator : ICreator
    {
        public IRepositoryItem Create(string name)
        {
            return readFromSTDIN(name);
        }
        
        private HostConfig readFromSTDIN(string name=null)
        {
            HostConfig hostConfig = new HostConfig() { Name=name,};
            hostConfig.Name = ConsoleHelper.ReadInput("hostname", hostConfig.Name);
            hostConfig.HostIP = ConsoleHelper.ReadInput("host ip", hostConfig.HostIP);
            hostConfig.Username = ConsoleHelper.ReadInput("username", hostConfig.Username);
            hostConfig.Password = ConsoleHelper.ReadInput("password", hostConfig.Password);
            hostConfig.CertPath = ConsoleHelper.ReadInput("certificate path", hostConfig.CertPath);
            hostConfig.Table = ConsoleHelper.ReadInput("table name", hostConfig.Table);
            hostConfig.Chain = ConsoleHelper.ReadInput("chain name", hostConfig.Chain);
            hostConfig.FlushChain = ConsoleHelper.ReadInputAsBool("flush chain", hostConfig.FlushChain ? "y" : "n");
            hostConfig.Set = ConsoleHelper.ReadInput("set name", hostConfig.Set);
            hostConfig.SupportsFlush = ConsoleHelper.ReadInputAsBool("supports flush", hostConfig.SupportsFlush ? "y" : "n");
            
            hostConfig.UsePubkeyLogin = String.IsNullOrEmpty(hostConfig.CertPath) == false;

            return hostConfig;
        }
    }

}