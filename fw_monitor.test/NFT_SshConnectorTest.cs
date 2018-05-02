
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
            expectedHostName = "hostname",
            expectedHostIP = "hostip";

        private bool 
            expectedSupportsFlush = true,
            expectedEmpty = false,
            expectedUsePubkeyLogin = false;
        
        private HostConfig generateDummyNftConfig()
        {
            HostConfig retVal = new HostConfig()
            {
                Empty = expectedEmpty,
                HostName = expectedHostName,
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
            NFT_SshConnector nftSshConnector = new NFT_SshConnector(generateDummyNftConfig());

            Assert.Equal(expectedEmpty, nftSshConnector.Empty);
            Assert.Equal(expectedHostName, nftSshConnector.HostName);
            Assert.Equal(expectedHostIP, nftSshConnector.HostIP);
            Assert.Equal(expectedUsername, nftSshConnector.Username);
            Assert.Equal(expectedPassword, nftSshConnector.Password);
            Assert.Equal(expectedCertPath, nftSshConnector.CertPath);
            Assert.Equal(expectedUsePubkeyLogin, nftSshConnector.UsePubkeyLogin);
            Assert.Equal(expectedTableName, nftSshConnector.Table);
            Assert.Equal(expectedChainName, nftSshConnector.Chain);
            Assert.Equal(expectedSetName, nftSshConnector.Set);
            Assert.Equal(expectedSupportsFlush, nftSshConnector.SupportsFlush);
        }
        
    }
}