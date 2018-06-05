using System;
using System.Text.RegularExpressions;
using fw_monitor.DataObjects;

namespace fw_monitor.test
{
    public static class TestHelper
    {
        /// <summary>
        /// TODO: make configurable.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ListConfig CreateDummyListConfig(string name = null)
        {
            ListConfig listConfig = new ListConfig()
            {
                Name=name ?? "test",
                Description = (name ?? "test") + "description",
                Url = new Uri("http://localhost"),
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
        
        /// <summary>
        /// TODO: make configurable.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static HostConfig CreateDummyHostConfig(string name = null)
        {
            HostConfig hostConfig = new HostConfig()
            {
                Name=name ?? "test",
                Description = (name ?? "test") + "description",
                HostIP="127.0.0.1",
                ConnectUsingIP = true,
                Username = "username",
                Password = "password",
                UsePubkeyLogin = false,
                CertPath = "certpath",
                Table = "table",
                Chain = "chain",
                FlushChain = true,
                Set = "set",
                SupportsFlush = true,
            };

            return hostConfig;
        }
        
    }
}