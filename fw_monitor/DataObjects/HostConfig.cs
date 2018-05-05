using System.Runtime.Serialization;

namespace fw_monitor.DataObjects
{
    [DataContract]
    public class HostConfig : Config
    {
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


        public string GetFormattedConfig(bool incSensitive)
        {
            string password = incSensitive ? Password : "{hidden}";

            return $@"[HostName: {Name};
HostIP: {HostIP};
UserName: {UserName};
Password: {password};
UsePubkeyLogin: {UsePubkeyLogin.ToString()};
CertPath: {CertPath};
TableName: {TableName};
ChainName: {ChainName};
FlushChain: {FlushChain.ToString()};
SetName: {SetName};
SupportsFlush: {SupportsFlush.ToString()};]";
        }

        public override string ToString() => GetFormattedConfig(false);
        public override bool Equals(object obj) => obj?.ToString() == ToString();
        public override int GetHashCode() => GetFormattedConfig(true).GetHashCode();

    }
}