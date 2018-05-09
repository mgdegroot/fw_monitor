using fw_monitor.DataObjects;
using Xunit;
using NSubstitute;

namespace fw_monitor.test
{
    public class ContentListTest
    {
        [Fact]
        public void Dummy()
        {
            Assert.True(true);
        }

        [Fact]
        public void SerializeTest()
        {
            ContentList contentList = new ContentList();
            contentList.IsSubList = false;
            contentList.Name = "testlist";
            contentList.Version = 1;
            contentList.SerializePath = "/tmp/";

            contentList.Add("a.b.c.d");
            contentList.Add("e.f.g.h");
        }
        
    }
}