using System;
using System.Text.RegularExpressions;
using fw_monitor.DataObjects;
using Xunit;
using NSubstitute;


namespace fw_monitor.test
{
    public class ListConfigRepositoryTest
    {
//        private ListConfig createDummy(string name = null)
//        {
//            ListConfig listConfig = new ListConfig()
//            {
//                Name=name ?? "test",
//                Description = (name ?? "test") + "description",
//                RevisionRegex = new Regex(".*"),
//                EmptyLineIndicators = new Regex(".*"),
//                SubsetHeader = new Regex(".*"),
//                InvalidCharReplacement = "#",
//                InvalidListnameChars = new Regex("#"),
//                IsComposite = true,
//                IsRevisioned = true,
//                URL = new Uri("http://localhost"),
//            };
//            
//            return listConfig;
//        }
        
        [Fact]
        public void SerializeTest()
        {
            // TODO: asserts ...
            ListConfig listConfig = TestHelper.CreateDummyListConfig("test");

            IRepository<ListConfig> tested = new Repository<ListConfig>();
            tested[listConfig.Name] = listConfig;

            Assert.True(true);
        }

        [Fact]
        public void DeserializeTest()
        {
            string expectedName = "test";
            ListConfig expected = TestHelper.CreateDummyListConfig(expectedName);

            IRepository<ListConfig> tested = new Repository<ListConfig>();
            ListConfig actual = (ListConfig)tested[expectedName];
            // TODO: Equals implementation for complex types...
            
            Assert.Equal(expected, actual);
        }
    }
}