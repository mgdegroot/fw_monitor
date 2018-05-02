using System.Text.RegularExpressions;
using Xunit;

namespace fw_monitor.test
{
    public class RegexTest
    {
        [Fact]
        public void RevisionRegexTest()
        {
            Regex regex = new Regex("");
        }

        [Fact]
        public void SubsetHeaderRegexTest()
        {
            Regex regex = new Regex(@"^#\s*(\w*)\s*$");
            const string expected = "testsubset";

            string line = "# testsubset";
            Match match = regex.Match(line);
            Assert.True(match.Success);
            Assert.Equal(2, match.Groups.Count);
            string actual = match.Groups[1].Value;

            Assert.Equal(expected, actual);

        }
    }
}