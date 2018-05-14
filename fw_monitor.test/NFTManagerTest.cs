using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using fw_monitor.DataObjects;
using Xunit;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace fw_monitor.test
{
    /// <summary>
    /// TODO: NSubstitute setup via helper methods to reduce repetition.
    /// TODO: Asserts on method calls
    /// </summary>
    public class NFTManagerTest
    {

        private NFTManager createTestSubject()
        {
            IConnector substConnector = Substitute.For<IConnector>(); 
            IExecutor substExecutor = Substitute.For<IExecutor>();
            IUtil substUtil = Substitute.For<IUtil>();
            
            IListFetcher substListFetcher = Substitute.For<IListFetcher>();

            IRepository<ListConfig> substListConfigRepository = Substitute.For<IRepository<ListConfig>>();
            IRepository<HostConfig> substHostConfigRepository = Substitute.For<IRepository<HostConfig>>();
            IRepository<ContentList> substListRepository = Substitute.For<IRepository<ContentList>>();
            
           
            substExecutor.Connector = substConnector;
            
            NFTManager nftManager = new NFTManager()
            {
                Executor = substExecutor,
                HostConfigRepository = substHostConfigRepository,
                ListConfigRepository = substListConfigRepository,
                ListRepository = substListRepository,
                Utility = substUtil,
                ListFetcher = substListFetcher,
            };

            return nftManager;
        }
        
        [Fact]
        public void Dummy()
        {
            Assert.True(true);
        }

        [Fact]
        public void ManageLists_WhenNotInteractiveAndNoConfigsPassedThenException()
        {
            NFTManager nftManager = createTestSubject();

            Task<NoNullAllowedException> ex = Assert.ThrowsAsync<NoNullAllowedException>(() =>
                nftManager.ManageLists(listConfigName: null, hostConfigName: null, interactive: false));
            
            
            Assert.Equal("Got null for a ListConfig...", ex.Result.Message);
        }

        [Fact]
        public void ManageLists_WhenNotInteractiveAndNoListConfigPassedThenException()
        {
            NFTManager nftManager = createTestSubject();

            Task<NoNullAllowedException> ex = Assert.ThrowsAsync<NoNullAllowedException>(() =>
                nftManager.ManageLists(listConfigName: null, hostConfigName: "no_match", interactive: false));
            
            
            Assert.Equal("Got null for a ListConfig...", ex.Result.Message);
        }
        
        [Fact]
        public void ManageLists_WhenNotInteractiveAndNoHostConfigPassedThenException()
        {
            const string LISTCONFIGNAME = "testlist";

            NFTManager nftManager = createTestSubject();
            nftManager.ListConfigRepository.Get(Arg.Any<string>())
                .Returns(TestHelper.CreateDummyListConfig(LISTCONFIGNAME));
            
            Task<NoNullAllowedException> ex = Assert.ThrowsAsync<NoNullAllowedException>(() =>
                nftManager.ManageLists(listConfigName: LISTCONFIGNAME, hostConfigName: null, interactive: false));
            
            
            Assert.Equal("Got null for a HostConfig...", ex.Result.Message);
        }

        [Fact]
        public void ManageLists_WhenNotInteractive()
        {
            const string LISTCONFIGNAME = "testlist";
            const string HOSTCONFIGNAME = "testhost";

            ListConfig listConfigDummy = TestHelper.CreateDummyListConfig(LISTCONFIGNAME);
            HostConfig hostConfigDummy = TestHelper.CreateDummyHostConfig(HOSTCONFIGNAME);
            
            NFTManager nftManager = createTestSubject();
            nftManager.HostConfigRepository.Get(Arg.Any<string>()).Returns(hostConfigDummy);
            nftManager.ListConfigRepository.Get(Arg.Any<string>()).Returns(listConfigDummy);


            Assert.True(true);
        }

        [Fact]
        public void ManageList_ListConfig_HostConfig_test()
        {
            const string LISTCONFIGNAME = "testlist";
            const string HOSTCONFIGNAME = "testhost";

            ListConfig listConfigDummy = TestHelper.CreateDummyListConfig(LISTCONFIGNAME);
            HostConfig hostConfigDummy = TestHelper.CreateDummyHostConfig(HOSTCONFIGNAME);

            NFTManager nftManager = createTestSubject();
            
            Dictionary<string,List<string>> listsDummy = new Dictionary<string, List<string>>();
            listsDummy["TEST01"] = new List<string>(new string[]{"TEST_ELEM_01"});
            
            nftManager.Executor.DoPreActions().Returns(true);
            nftManager.Executor.ProcessList("TEST01", listsDummy["TEST01"]).Returns(true);

            nftManager.ListConfigRepository.Get(LISTCONFIGNAME).Returns(listConfigDummy);
            nftManager.HostConfigRepository.Get(HOSTCONFIGNAME).Returns(hostConfigDummy);

            nftManager.ListFetcher.Lists = listsDummy;
            nftManager.ListFetcher.FetchAndParse().Returns(Task.CompletedTask);
            
            nftManager.ManageLists(listConfigDummy, hostConfigDummy);
            
            // TODO: asserts that required calls have been made.
            Assert.True(true);
        }
    }
}