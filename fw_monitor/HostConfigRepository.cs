using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace fw_monitor
{
    public static class HostConfigRepository
    {
        private static string REPO_FILE_EXTENSION = ".json";
        
        // TODO: move to config file -->
        private static string REPO_BASE_PATH = Environment.GetEnvironmentVariable("HOME") + "/NftHosts/";
        public static Dictionary<string, NftConfig> Repository { get; private set; } = new Dictionary<string, NftConfig>();
        
        public static NftConfig Get(string hostname)
        {
            Repository.TryGetValue(hostname, out NftConfig nftConfig);

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
                    return new NftConfig() {HostName = hostname, Empty = true};
                }
                
            }

            return nftConfig;
        }

        public static void SetConfig(NftConfig nftConfig)
        {
            Repository[nftConfig.HostName] = nftConfig;
            string strConf = serialize(nftConfig);
            writeToFile(Path.Combine(REPO_BASE_PATH, nftConfig.HostName + REPO_FILE_EXTENSION), strConf);
        }
        
        private static string serialize(NftConfig nftConfig)
        {
            // serializing here so set 'NewEntry' to false -->
            nftConfig.Empty = false;
            MemoryStream memoryStream = new MemoryStream();
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(NftConfig));
            serializer.WriteObject(memoryStream, nftConfig);

            byte[] json = memoryStream.ToArray();
            memoryStream.Close();
            return Encoding.UTF8.GetString(json, 0, json.Length);
        }

        private static NftConfig deserialize(string json)
        {
            
            MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(NftConfig));
            NftConfig nftConfig = (NftConfig)serializer.ReadObject(memoryStream) as NftConfig;

            return nftConfig;
        }

        private static string readFromFile(string path)
        {
            string content = File.ReadAllText(path, Encoding.UTF8);
            return content;
        }

        private static void writeToFile(string path, string content)
        {
            File.WriteAllText(path, content, Encoding.UTF8);
        }
        
    }
    
    

    [DataContract]
    public class NftConfig
    {
        [DataMember(Order = 0)] public bool Empty { get; set; } = true;
    
        [DataMember(Order = 1)] public string HostName { get; set; }
        [DataMember(Order = 2)] public string HostIP { get; set; }
        [DataMember(Order = 3)] public bool ConnectUsingIP { get; set; } = true;
        [DataMember(Order = 4)] public string UserName { get; set; }
        [DataMember(Order = 5)] public string Password { get; set; }
        [DataMember(Order = 6)] public bool UsePubkeyLogin { get; set; } = false;
        [DataMember(Order = 7)] public string CertPath { get; set; } = string.Empty;
        [DataMember(Order = 8)] public string TableName { get; set; }
        [DataMember(Order = 9)] public string ChainName { get; set; }
        [DataMember(Order = 10)] public bool FlushChain { get; set; }
        [DataMember(Order = 11)] public string SetName { get; set; }
        [DataMember(Order = 12)] public bool SupportsFlush { get; set; }

        public string GetFormattedConfig(bool incPassword)
        {
            string password = incPassword ? Password : "{hidden}";
            return $@"HostName: {HostName}
HostIP: {HostIP}
UserName: {UserName}
Password: {password}
UsePubkeyLogin: {UsePubkeyLogin.ToString()}
CertPath: {CertPath}
TableName: {TableName}
ChainName: {ChainName}
FlushChain: {FlushChain}
SetName: {SetName}
SupportsFlush: {SupportsFlush.ToString()}";
        }

    }
}