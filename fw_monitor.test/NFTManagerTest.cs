using System;
using System.Text.RegularExpressions;
using fw_monitor.DataObjects;
using Xunit;
using NSubstitute;

namespace fw_monitor.test
{
    public class NFTManagerTest
    {
        
        
        [Fact]
        public void Dummy()
        {
            Assert.True(true);
            IConnector connector = Substitute.For<IConnector>(); 
            IExecutor executor = Substitute.For<IExecutor>();
            
//            HostConfig hostConfig = Substitute.For<HostConfig>();
//            ListConfig listConfig = Substitute.For<ListConfig>();
            HostConfig hostConfig = TestHelper.CreateDummyHostConfig("testhostconfig");
            ListConfig listConfig = TestHelper.CreateDummyListConfig("testlistconfig");
            
            executor.Connector = connector;
            
            executor.DoPreActions().Returns(true);
            NFTManager nftManager = new NFTManager();
            
            nftManager.HostConfig = hostConfig;
            nftManager.ListConfig = listConfig;
            nftManager.Executor = executor;
            
            nftManager.ManageLists(listConfig, hostConfig);
        }
    }
}