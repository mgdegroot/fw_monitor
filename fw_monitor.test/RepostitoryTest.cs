//using System;
//using Xunit;
//using NSubstitute;
//
//namespace fw_monitor.test
//{
//    public class RepostitoryTest
//    {
//
//        [Fact]
//        public void Dummy()
//        {
//            Assert.True(true);
//        }
//
//
//        [Fact]
//        public void GetInstance_WhenKnownTypeAskedThenInstanceReturned()
//        {
//            
//            IUtil substUtil = Substitute.For<IUtil>();
//            
//            Repository repository = new Repository(substUtil);
//            
//            IRepository actualRepository = repository.GetInstance(typeof(HostConfigRepository));
//
//            Assert.IsAssignableFrom<HostConfigRepository>(actualRepository);
//
//            actualRepository = repository.GetInstance(typeof(ListConfigRepository));
//
//            Assert.IsAssignableFrom<ListConfigRepository>(actualRepository);
//
//            actualRepository = repository.GetInstance(typeof(ListRepository));
//
//            Assert.IsAssignableFrom<ListRepository>(actualRepository);
//
//
//        }
//
//        [Fact]
//        public void GetInstance_WhenAskedAgainThenSameInstanceReturned()
//        {
//            IUtil substUtil = Substitute.For<IUtil>();
//            
//            Repository repository = new Repository(substUtil);
//            
//            IRepository actualFirstRepository = repository.GetInstance(typeof(HostConfigRepository));
//            IRepository actualSecondRepository = repository.GetInstance(typeof(HostConfigRepository));
//
//            Assert.Same(actualFirstRepository, actualSecondRepository);
//            
//            actualFirstRepository = repository.GetInstance(typeof(HostConfigRepository));
//            IRepository intermediateRepository = repository.GetInstance(typeof(ListConfigRepository));
//            IRepository nullRepository = repository.GetInstance(typeof(String));
//            actualSecondRepository = repository.GetInstance(typeof(HostConfigRepository));
//
//            Assert.Same(actualFirstRepository, actualSecondRepository);
//
//        }
//
//        [Fact]
//        public void GetInstance_WhenUnknownTypeAskedThenNullReturned()
//        {
//            IUtil substUtil = Substitute.For<IUtil>();
//            
//            Repository repository = new Repository(substUtil);
//            
//            IRepository actualRepository = repository.GetInstance(typeof(String));
//            Assert.Null(actualRepository);
//        }
//        
//    }
//}