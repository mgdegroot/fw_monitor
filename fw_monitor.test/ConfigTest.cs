using System;
using System.Text.RegularExpressions;
using fw_monitor.DataObjects;
using Xunit;
using Xunit.Sdk;

namespace fw_monitor.test
{
    public class ConfigTest
    {
        private ListConfig createDummyListConfig(string name = null)
        {
            ListConfig listConfig = new ListConfig()
            {
                Name=name ?? "test",
                Description = (name ?? "test") + "description",
                URL = new Uri("http://localhost"),
                IsComposite = true,
                IsRevisioned = true,
                RevisionRegex = new Regex(".*"),
                SubsetHeader = new Regex(".*"),
                EmptyLineIndicators = new Regex(".*"),
                InvalidListnameChars = new Regex("#"),
                InvalidCharReplacement = "#",
                LineSeparator = Environment.NewLine,
            };

            return listConfig;
        }

        private HostConfig createDummyHostConfig(string name = null)
        {
            HostConfig hostConfig = new HostConfig()
            {
                Name=name ?? "test",
                Description = (name ?? "test") + "description",
                HostIP="127.0.0.1",
                ConnectUsingIP = true,
                UserName = "username",
                Password = "password",
                UsePubkeyLogin = false,
                CertPath = "certpath",
                TableName = "tablename",
                ChainName = "chainname",
                FlushChain = true,
                SetName = "setname",
                SupportsFlush = true,
            };

            return hostConfig;
        }
        
        [Fact]
        public void HostConfig_ToStringTest()
        {
            string expected = @"[HostName: test;
HostIP: 127.0.0.1;
UserName: username;
Password: {hidden};
UsePubkeyLogin: False;
CertPath: certpath;
TableName: tablename;
ChainName: chainname;
FlushChain: True;
SetName: setname;
SupportsFlush: True;]";

            string actual = createDummyHostConfig("test").ToString();

            Assert.Equal(expected, actual);

        }
        
    }
}