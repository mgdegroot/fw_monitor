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
        
        
        [Fact]
        public void Dummy()
        {
            Assert.True(true);
            IConnector connector = Substitute.For<IConnector>(); 
            IExecutor executor = Substitute.For<IExecutor>();
            executor.Connector = connector;            
//            HostConfig hostConfig = Substitute.For<HostConfig>();
//            ListConfig listConfig = Substitute.For<ListConfig>();
            HostConfig hostConfig = TestHelper.CreateDummyHostConfig("testhostconfig");
            ListConfig listConfig = TestHelper.CreateDummyListConfig("testlistconfig");
            

            
            executor.DoPreActions().Returns(true);
            NFTManager nftManager = new NFTManager();
            
            nftManager.HostConfig = hostConfig;
            nftManager.ListConfig = listConfig;
            nftManager.Executor = executor;
            
            nftManager.ManageLists(listConfig, hostConfig);
        }

        [Fact]
        public void ManageLists_WhenNotInteractiveAndNoConfigsPassedThenException()
        {
            IConnector connector = Substitute.For<IConnector>(); 
            IExecutor executor = Substitute.For<IExecutor>();
            IRepository repository = Substitute.For<IRepository>();
            repository.GetInstance(typeof(ListConfigRepository)).Returns(new ListConfigRepository());
            repository.GetInstance(typeof(HostConfigRepository)).Returns(new HostConfigRepository());
            
            executor.Connector = connector;
            HostConfig hostConfig = TestHelper.CreateDummyHostConfig("testhostconfig");
            
            NFTManager nftManager = new NFTManager()
            {
                Repository =  repository,
            };

            Task<NoNullAllowedException> ex = Assert.ThrowsAsync<NoNullAllowedException>(() =>
                nftManager.ManageLists(listConfigName: null, hostConfigName: null, interactive: false));
            
            
            Assert.Equal("Got null for a ListConfig...", ex.Result.Message);
        }

        [Fact]
        public void ManageLists_WhenNotInteractiveAndNoListConfigPassedThenException()
        {
            IConnector connector = Substitute.For<IConnector>(); 
            IExecutor executor = Substitute.For<IExecutor>();
            IRepository repository = Substitute.For<IRepository>();
            repository.GetInstance(typeof(ListConfigRepository)).Returns(new ListConfigRepository());
            repository.GetInstance(typeof(HostConfigRepository)).Returns(new HostConfigRepository());
            
            executor.Connector = connector;
            
            NFTManager nftManager = new NFTManager()
            {
                Repository =  repository,
            };

            Task<NoNullAllowedException> ex = Assert.ThrowsAsync<NoNullAllowedException>(() =>
                nftManager.ManageLists(listConfigName: null, hostConfigName: "no_match", interactive: false));
            
            
            Assert.Equal("Got null for a ListConfig...", ex.Result.Message);
        }
        
        [Fact]
        public void ManageLists_WhenNotInteractiveAndNoHostConfigPassedThenException()
        {
            const string LISTCONFIGNAME = "testlist";
            
            IConnector substConnector = Substitute.For<IConnector>(); 
            IExecutor substExecutor = Substitute.For<IExecutor>();
            IRepository substRepository = Substitute.For<IRepository>();
            ListConfigRepository substListConfigRepository = Substitute.For<ListConfigRepository>();
            substListConfigRepository.Get(LISTCONFIGNAME).Returns(TestHelper.CreateDummyListConfig(LISTCONFIGNAME));
            
            substRepository.GetInstance(typeof(ListConfigRepository)).Returns(substListConfigRepository);
            substRepository.GetInstance(typeof(HostConfigRepository)).Returns(new HostConfigRepository());
            
            substExecutor.Connector = substConnector;
            
            NFTManager nftManager = new NFTManager()
            {
                Repository =  substRepository,
            };

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
            IConnector substConnector = Substitute.For<IConnector>();
            IExecutor substExecutor = Substitute.For<IExecutor>();
            ListConfigRepository substListConfigRepository = Substitute.For<ListConfigRepository>();
            substListConfigRepository.Get(LISTCONFIGNAME).Returns(listConfigDummy);
            
            
            HostConfigRepository substHostConfigRepository = Substitute.For<HostConfigRepository>();
            substHostConfigRepository.Get(HOSTCONFIGNAME).Returns(hostConfigDummy);
            
            IRepository substRepository = Substitute.For<IRepository>();
            
            substRepository.GetInstance(typeof(ListConfigRepository)).Returns(substListConfigRepository);
            substRepository.GetInstance(typeof(HostConfigRepository)).Returns(substHostConfigRepository);
            
            substExecutor.Connector = substConnector;
            
            NFTManager nftManager = new NFTManager()
            {
                Repository =  substRepository,
                Executor = substExecutor,
            };
            
//            NFTManager substNftManager = Substitute.For<NFTManager>();
//            substNftManager.Repository = substRepository;
//            
//            substNftManager.ManageLists(listConfigDummy, hostConfigDummy).ReturnsNull();
            
            
            
//            Task<NoNullAllowedException> ex = Assert.ThrowsAsync<NoNullAllowedException>(() =>
//                nftManager.ManageLists(listConfigName: LISTCONFIGNAME, hostConfigName: null, interactive: false));
            
            
            Assert.True(false, "do somethingzzzz");
        }

        [Fact]
        public void ManageList_ListConfig_HostConfig_test()
        {
            const string LISTCONFIGNAME = "testlist";
            const string HOSTCONFIGNAME = "testhost";

            ListConfig listConfigDummy = TestHelper.CreateDummyListConfig(LISTCONFIGNAME);
            HostConfig hostConfigDummy = TestHelper.CreateDummyHostConfig(HOSTCONFIGNAME);
            
            Dictionary<string,List<string>> listsDummy = new Dictionary<string, List<string>>();
            listsDummy["TEST01"] = new List<string>(new string[]{"TEST_ELEM_01"});
            
            IConnector substConnector = Substitute.For<IConnector>();
            IExecutor substExecutor = Substitute.For<IExecutor>();
            substExecutor.DoPreActions().Returns(true);
            substExecutor.ProcessList("TEST01", listsDummy["TEST01"]).Returns(true);
            
            ListConfigRepository substListConfigRepository = Substitute.For<ListConfigRepository>();
            substListConfigRepository.Get(LISTCONFIGNAME).Returns(listConfigDummy);
            
            
            HostConfigRepository substHostConfigRepository = Substitute.For<HostConfigRepository>();
            substHostConfigRepository.Get(HOSTCONFIGNAME).Returns(hostConfigDummy);
            
            IRepository substRepository = Substitute.For<IRepository>();
            
            substRepository.GetInstance(typeof(ListConfigRepository)).Returns(substListConfigRepository);
            substRepository.GetInstance(typeof(HostConfigRepository)).Returns(substHostConfigRepository);
            
            substExecutor.Connector = substConnector;

            IListFetcher substListFetcher = Substitute.For<IListFetcher>();
            substListFetcher.Lists = listsDummy;
            substListFetcher.FetchAndParse().Returns(Task.CompletedTask);
            
            NFTManager nftManager = new NFTManager()
            {
                Repository =  substRepository,
                Executor = substExecutor,
                ListFetcher = substListFetcher,
            };

            nftManager.ManageLists(listConfigDummy, hostConfigDummy);
            
            // TODO: asserts that required calls have been made.
            Assert.True(true);



        }
        
        
    }
}