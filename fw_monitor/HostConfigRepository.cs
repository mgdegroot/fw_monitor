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
    public class HostConfigRepository : RepositoryBase, IRepository
    {
//        private static HostConfigRepository _instance = new HostConfigRepository();
//
//        public static IRepository Instance => _instance;
        

        public override Dictionary<string, Config> Repository { get; set; } = new Dictionary<string, Config>();
        
        public override Config Get(string hostname)
        {
            Repository.TryGetValue(hostname, out Config nftConfig);

            if (nftConfig == null)
            {
                string path = Path.Combine(REPO_BASE_PATH, hostname + REPO_FILE_EXTENSION);
                if (File.Exists(path))
                {
                    string strConf = readFromFile(path);
                    nftConfig = deserialize(strConf);
                    Repository.Add(hostname, nftConfig);
                }
                else
                {
                    return null;
//                    return new HostConfig() {HostName = hostname, Empty = true};
                }
                
            }

            return nftConfig;
        }

        public override void Set(Config hostConfig)
        {
            Repository[hostConfig.Name] = hostConfig;
            string strConf = serialize(hostConfig as HostConfig);
            writeToFile(Path.Combine(REPO_BASE_PATH, hostConfig.Name + REPO_FILE_EXTENSION), strConf);
        }

        public override Config CreateNew(string name)
        {
            return readFromSTDIN(name);
        }

//        public HostConfig ReadFromSTDIN()
//        {
//            HostConfig hostConfig = new HostConfig();
//            hostConfig.Name = ConsoleHelper.readInput("hostname", hostConfig.Name);
//            hostConfig.HostIP = ConsoleHelper.readInput("host ip", hostConfig.HostIP);
//            hostConfig.UserName = ConsoleHelper.readInput("username", hostConfig.UserName);
//            hostConfig.Password = ConsoleHelper.readInput("password", hostConfig.Password);
//            hostConfig.CertPath = ConsoleHelper.readInput("certificate path", hostConfig.CertPath);
//            hostConfig.TableName = ConsoleHelper.readInput("table name", hostConfig.TableName);
//            hostConfig.ChainName = ConsoleHelper.readInput("chain name", hostConfig.ChainName);
//            hostConfig.FlushChain = ConsoleHelper.readInputAsBool("flush chain", hostConfig.FlushChain ? "y" : "n");
//            hostConfig.SetName = ConsoleHelper.readInput("set name", hostConfig.SetName);
//            hostConfig.SupportsFlush = ConsoleHelper.readInputAsBool("supports flush", hostConfig.SupportsFlush ? "y" : "n");
//            
//            hostConfig.UsePubkeyLogin = String.IsNullOrEmpty(hostConfig.CertPath) == false;
//
//            return hostConfig;
//        }
        
        private string serialize(HostConfig hostConfig)
        {
            // serializing here so set 'NewEntry' to false -->
            hostConfig.Empty = false;
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

//        private string readFromFile(string path)
//        {
//            string content = File.ReadAllText(path, Encoding.UTF8);
//            return content;
//        }
//
//        private void writeToFile(string path, string content)
//        {
//            File.WriteAllText(path, content, Encoding.UTF8);
//        }

        
    }

}