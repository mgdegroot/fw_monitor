
using fw_monitor.DataObjects;
using Xunit;

namespace fw_monitor.test
{
    public class NFT_SshConnectorTest
    {
        private string 
            expectedCertPath = "certpath",
            expectedChainName = "chainName",
            expectedTableName = "tableName",
            expectedSetName = "setName",
            expectedUsername = "userName",
            expectedPassword = "password",
            expectedName = "hostname",
            expectedHostIP = "hostip";

        private bool 
            expectedSupportsFlush = true,
            expectedUsePubkeyLogin = false;
        
        private HostConfig generateDummyNftConfig()
        {
            HostConfig retVal = new HostConfig()
            {
                Name = expectedName,
                HostIP = expectedHostIP,
                Username = expectedUsername,
                Password = expectedPassword,
                CertPath = expectedCertPath,
                UsePubkeyLogin = expectedUsePubkeyLogin,
                Table = expectedTableName,
                Chain = expectedChainName,
                Set = expectedSetName,
                SupportsFlush = expectedSupportsFlush,
            };

            return retVal;
        }
        
        [Fact]
        public void ConstructorTest()
        {
            SshConnector sshConnector = new SshConnector(generateDummyNftConfig());

            Assert.Equal(expectedName, sshConnector.HostConfig.Name);
            Assert.Equal(expectedHostIP, sshConnector.HostConfig.HostIP);
            Assert.Equal(expectedUsername, sshConnector.HostConfig.Username);
            Assert.Equal(expectedPassword, sshConnector.HostConfig.Password);
            Assert.Equal(expectedCertPath, sshConnector.HostConfig.CertPath);
            Assert.Equal(expectedUsePubkeyLogin, sshConnector.HostConfig.UsePubkeyLogin);
            Assert.Equal(expectedTableName, sshConnector.HostConfig.Table);
            Assert.Equal(expectedChainName, sshConnector.HostConfig.Chain);
            Assert.Equal(expectedSetName, sshConnector.HostConfig.Set);
            Assert.Equal(expectedSupportsFlush, sshConnector.HostConfig.SupportsFlush);
        }
        
    }
}