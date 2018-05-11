using System.Collections.Generic;
using fw_monitor.DataObjects;
using Xunit;
using NSubstitute;
using NSubstitute.Core.Arguments;

namespace fw_monitor.test
{
    public class ListRepositoryTest
    {
        private ContentList createDummyContentList(string name)
        {
            ContentList contentList = new ContentList()
            {
                Name = name,
            };
            return contentList;
        }
        
        [Fact]
        public void Dummy()
        {
            Assert.True(true);
        }

        [Fact]
        public void WhenCreateThenNewContentListTest()
        {
            const string expectedName = "test";
            ContentList expectedContentList = createDummyContentList(expectedName);
            
            ICreator substCreator = Substitute.For<ICreator>();
            substCreator.Create(Arg.Any<string>()).Returns(expectedContentList);
            
            IUtil util = new Util();
            ListRepository listRepository = new ListRepository(util)
            {
                Creator = substCreator, 
            };

            ContentList actualContentList = listRepository.Creator.Create(expectedName) as ContentList;

            Assert.NotNull(actualContentList);
            Assert.Equal(expectedName, actualContentList.Name);

            Assert.Equal(expectedContentList, actualContentList);

        }
    }
}