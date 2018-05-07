using System;
using System.Text.RegularExpressions;
using fw_monitor.DataObjects;
using Xunit;

namespace fw_monitor.test
{
    public class ListConfigTest
    {

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