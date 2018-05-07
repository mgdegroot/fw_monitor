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
            executor.Connector = connector;

            executor.DoPreActions().Returns(true);
            executor.ProcessList()


        }
        
    }
}