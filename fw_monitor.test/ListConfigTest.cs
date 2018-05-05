using System;
using System.Text.RegularExpressions;
using fw_monitor.DataObjects;
using Xunit;

namespace fw_monitor.test
{
    public class ListConfigTest
    {
        private ListConfig createDummyListConfig(string name = null)
        {
            ListConfig listConfig = new ListConfig()
            {
                Name=name ?? "test",
                Description = (name ?? "test") + "description",
                URL = new Uri("http://localhost"),
                IsComposite = true,
                IsRevisioned = true,
                RevisionRegex = new Regex(".*"),
                SubsetHeader = new Regex(".*"),
                EmptyLineIndicators = new Regex(".*"),
                InvalidListnameChars = new Regex("#"),
                InvalidCharReplacement = "#",
                LineSeparator = Environment.NewLine,
            };

            return listConfig;
        }

        [Fact]
        public void ToStringTest()
        {
            string expected = "[Name: test;\n"
+ "Description: testdescription;\n"
+ "URL: http://localhost/;\n"
+ "IsComposite: True;\n"
+ "SubsetHeader: .*;\n"
+ "IsRevisioned: True;\n"
+ "RevisionMatch: .*;\n"
+ "InvalidChars: #;\n"
+ "EmptyLineIndicators: .*;\n"
+ "LineSeparator: \n;]";

            string actual = createDummyListConfig("test").ToString();

            Assert.Equal(expected, actual);
        }
        
    }
}