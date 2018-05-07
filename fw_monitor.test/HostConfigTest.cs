using System;
using System.Text.RegularExpressions;
using fw_monitor.DataObjects;
using Xunit;
using Xunit.Sdk;

namespace fw_monitor.test
{
    public class HostConfigTest
    {
        [Fact]
        public void ToStringTest()
        {
            string expected = @"[HostName: test;
HostIP: 127.0.0.1;
Username: username;
Password: {hidden};
UsePubkeyLogin: False;
CertPath: certpath;
Table: table;
Chain: chain;
FlushChain: True;
Set: set;
SupportsFlush: True;]";

            string actual = TestHelper.CreateDummyHostConfig("test").ToString();

            Assert.Equal(expected, actual);

        }

        [Fact]
        public void GetFormattedConfig_hideSensitiveTest()
        {
            string expected = @"[HostName: test;
HostIP: 127.0.0.1;
Username: username;
Password: {hidden};
UsePubkeyLogin: False;
CertPath: certpath;
Table: table;
Chain: chain;
FlushChain: True;
Set: set;
SupportsFlush: True;]";

            string actual = TestHelper.CreateDummyHostConfig("test").GetFormattedConfig(false);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetFormattedConfig_showSensitiveTest()
        {
            string expected = @"[HostName: test;
HostIP: 127.0.0.1;
Username: username;
Password: password;
UsePubkeyLogin: False;
CertPath: certpath;
Table: table;
Chain: chain;
FlushChain: True;
Set: set;
SupportsFlush: True;]";

            string actual = TestHelper.CreateDummyHostConfig("test").GetFormattedConfig(true);

            Assert.Equal(expected, actual);
        }
        
    }
}