
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
                UserName = expectedUsername,
                Password = expectedPassword,
                CertPath = expectedCertPath,
                UsePubkeyLogin = expectedUsePubkeyLogin,
                TableName = expectedTableName,
                ChainName = expectedChainName,
                SetName = expectedSetName,
                SupportsFlush = expectedSupportsFlush,
            };

            return retVal;
        }
        
        [Fact]
        public void ConstructorTest()
        {
            SshConnector sshConnector = new SshConnector(generateDummyNftConfig());

            Assert.Equal(expectedName, sshConnector.HostName);
            Assert.Equal(expectedHostIP, sshConnector.HostIP);
            Assert.Equal(expectedUsername, sshConnector.Username);
            Assert.Equal(expectedPassword, sshConnector.Password);
            Assert.Equal(expectedCertPath, sshConnector.CertPath);
            Assert.Equal(expectedUsePubkeyLogin, sshConnector.UsePubkeyLogin);
            Assert.Equal(expectedTableName, sshConnector.Table);
            Assert.Equal(expectedChainName, sshConnector.Chain);
            Assert.Equal(expectedSetName, sshConnector.Set);
            Assert.Equal(expectedSupportsFlush, sshConnector.SupportsFlush);
        }
        
    }
}