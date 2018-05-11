using System.Collections.Generic;
using fw_monitor.DataObjects;
using Xunit;
using NSubstitute;

namespace fw_monitor.test
{
    public class ContentListTest
    {

        private List<string> createDummyList()
        {
            List<string> retVal = new List<string>(new [] {
                "192.168.100.1", 
                "192.168.100.2", 
                "192.168.100.3",
            });

            return retVal;
        }
        
        [Fact]
        public void Dummy()
        {
            Assert.True(true);
        }

        [Fact]
        public void WhenAddThenGettableTest()
        {
            const string expectedItem1 = "a.b.c.d";
            const string expectedItem2 = "e.f.g.h";
            
//            var substUtil = Substitute.For<IUtil>();
//            substUtil.WriteToFile(Arg.Any<string>(), Arg.Any<string>(), false).Returns(true);
            
            ContentList contentList = new ContentList();
            contentList.IsSubList = false;
            contentList.Name = "testlist";
            contentList.Version = "1";

            contentList.Add(expectedItem1);
            contentList.Add(expectedItem2);
            
            Assert.Equal(expectedItem1, contentList.Get(0));
            Assert.Equal(expectedItem2, contentList.Get(1));
            
        }
        
        [Fact]
        public void SerializeTest()
        {
            const string addedItem1 = "a.b.c.d";
            const string addedItem2 = "e.f.g.h";
            const string expectedSerialization = @"{""IsSubList"":false,""Name"":""testlist"",""Version"":""1"",""_content"":[""a.b.c.d"",""e.f.g.h""]}";
            
//            var substUtil = Substitute.For<IUtil>();
//            substUtil.WriteToFile(Arg.Any<string>(), Arg.Any<string>(), false).Returns(true);
            
            ContentList contentList = new ContentList();
            contentList.IsSubList = false;
            contentList.Name = "testlist";
            contentList.Version = "1";

            contentList.Add(addedItem1);
            contentList.Add(addedItem2);
            string actualSerialization = contentList.Serialize();

            Assert.Equal(expectedSerialization, actualSerialization);

        }

        [Fact]
        public void IterateTest()
        {
            ContentList contentList = new ContentList();
            contentList.Add("0");
            contentList.Add("1");
            contentList.Add("2");
            int expectedItem = 0;
            foreach (string actualValue in contentList)
            {
                Assert.Equal(expectedItem++.ToString(), actualValue);
            }
        }
        
    }
}