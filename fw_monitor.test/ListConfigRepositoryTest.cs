using System;
using System.Text.RegularExpressions;
using fw_monitor.DataObjects;
using Xunit;


namespace fw_monitor.test
{
    public class ListConfigRepositoryTest
    {
        private ListConfig createDummy(string name = null)
        {
            ListConfig listConfig = new ListConfig()
            {
                Name=name ?? "test",
                Description = (name ?? "test") + "description",
                RevisionRegex = new Regex(".*"),
                EmptyLineIndicators = new Regex(".*"),
                SubsetHeader = new Regex(".*"),
                InvalidCharReplacement = "#",
                InvalidListnameChars = new Regex("#"),
                IsComposite = true,
                IsRevisioned = true,
                URL = new Uri("http://localhost"),
            };

            return listConfig;
        }
        
        [Fact]
        public void SerializeTest()
        {
            // TODO: asserts ...
            ListConfig listConfig = createDummy();
            
            ListConfigRepository tested = new ListConfigRepository();
            tested[listConfig.Name] = listConfig;
        }

        [Fact]
        public void DeserializeTest()
        {
            string expectedName = "test";
            ListConfig expected = createDummy(expectedName);
            ListConfigRepository tested = new ListConfigRepository();
            ListConfig actual = (ListConfig)tested[expectedName];
            // TODO: Equals implementation for complex types...
            Assert.Equal(expected, actual);
        }
    }
}