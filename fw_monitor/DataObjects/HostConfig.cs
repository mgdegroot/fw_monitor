using System;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.Serialization;

namespace fw_monitor.DataObjects
{
    [DataContract]
    public class HostConfig : Config
    {
        private ICreator _creator = new HostConfigFromStdInCreator();
        
        [DataMember(Order = 2)] public string HostIP { get; set; }
        [DataMember(Order = 3)] public bool ConnectUsingIP { get; set; } = true;
        [DataMember(Order = 4)] public string Username { get; set; }
        [DataMember(Order = 5)] public string Password { get; set; }
        [DataMember(Order = 6)] public bool UsePubkeyLogin { get; set; } = false;
        [DataMember(Order = 7)] public string CertPath { get; set; } = string.Empty;
        [DataMember(Order = 8)] public string Table { get; set; }
        [DataMember(Order = 9)] public string Chain { get; set; }
        [DataMember(Order = 10)] public bool FlushChain { get; set; }
        [DataMember(Order = 11)] public string Set { get; set; }
        [DataMember(Order = 12)] public bool SupportsFlush { get; set; }


        public string GetFormattedConfig(bool incSensitive)
        {
            string password = incSensitive ? Password : "{hidden}";

            return $@"[HostName: {Name};
HostIP: {HostIP};
Username: {Username};
Password: {password};
UsePubkeyLogin: {UsePubkeyLogin.ToString()};
CertPath: {CertPath};
Table: {Table};
Chain: {Chain};
FlushChain: {FlushChain.ToString()};
Set: {Set};
SupportsFlush: {SupportsFlush.ToString()};]";
        }

        public override ICreator Creator
        {
            get => _creator;
            set => _creator = value;
        }

        public override string ToString() => GetFormattedConfig(false);
        public override bool Equals(object obj) => obj?.ToString() == ToString();
        public override int GetHashCode() => GetFormattedConfig(true).GetHashCode();

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