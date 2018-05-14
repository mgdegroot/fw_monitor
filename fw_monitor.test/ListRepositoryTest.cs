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
            // TODO replace this non-sensible test with something usefulzzz -->
            const string expectedName = "test";
            ContentList expectedContentList = createDummyContentList(expectedName);
            
            ICreator substCreator = Substitute.For<ICreator>();
            substCreator.Create(Arg.Any<string>()).Returns(expectedContentList);
            
            IUtil util = Substitute.For<IUtil>();
            Repository<ContentList> listRepository = new Repository<ContentList>()
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