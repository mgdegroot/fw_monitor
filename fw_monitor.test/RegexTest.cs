using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
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

        [Fact]
        public void ListSerializationTest()
        {
            List<string> test = new List<string>(new string[] {"1","2","3"});
            
            MemoryStream memoryStream = new MemoryStream();
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<string>));

            try
            {
                serializer.WriteObject(memoryStream, test);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"nope: {ex.Message}");
            }

            byte[] json = memoryStream.ToArray();
            memoryStream.Close();

            string res = Encoding.UTF8.GetString(json);

            Assert.NotEmpty(res);
        }
    }
}