using System;
using System.Text.RegularExpressions;
using fw_monitor.DataObjects;
using Xunit;

namespace fw_monitor.test
{
    public class ListConfigTest
    {
//        private static void fillDefaults()
//        {
//            ListConfig nwConfig = new ListConfig()
//            {
//                Name = "emergingthreats",
//                Description = "Emerging Threats combined blocklist",
//                URL = new Uri(@"https://rules.emergingthreats.net/fwrules/emerging-Block-IPs.txt"),
//                IsComposite = true,
//                IsRevisioned = true,
//                LineSeparator = Environment.NewLine,
//                RevisionRegex = new Regex(@"^# Rev (\d*)$"),
//                SubsetHeader = new Regex(@"^#\s*(.*)\s*$"),
////                SubsetSeparator = "#",
//            };
//
//            Repository[nwConfig.Name] = nwConfig;
//            
//            nwConfig = new ListConfig()
//            {
//                Name = "locallist",
//                Description = "Locally hosted list for testing",
//                URL = new Uri(@"http://localhost/emerging_threats.txt"),
//                IsComposite = true,
//                IsRevisioned = true,
//                LineSeparator = Environment.NewLine,
//                RevisionRegex = new Regex(@"^# Rev (\d*)$"),
//                SubsetHeader =  new Regex(@"^#\s*(.*)\s*$"),
////                SubsetSeparator = "#",
//            };
//
//            Repository[nwConfig.Name] = nwConfig;
//
//        }
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

            string actual = TestHelper.CreateDummyListConfig("test").ToString();

            Assert.Equal(expected, actual);
        }
        
    }
}