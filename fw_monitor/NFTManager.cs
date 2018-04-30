using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace fw_monitor
{
    public class NFTManager
    {
//        private const string URL_EMERGING_THREATS_BLOCK_IPS =
//            @"https://rules.emergingthreats.net/fwrules/emerging-Block-IPs.txt";
//        
//        private const string URL_DBG_EMERGING_THREATS = 
//            @"http://localhost/emerging_threats.txt";
        
        public async Task ManageLists()
        {
            ListConfig listConfig = getListConfig();
            if (listConfig == null)
            {
                Console.WriteLine("No config selected. Exiting...");
                Environment.Exit(1);
            }
            
            NftConfig hostConfig = getRemoteHostConfig();
            Dictionary<string, List<string>> lists = await fetchList(listConfig);
            
            NFT_SshConnector connector = new NFT_SshConnector(hostConfig);
            connector.ErrorAdded += nftSsh_ErrorAdded;
            connector.OutputAdded += nftSsh_OutputAdded;
            
            string answer = readEntry("Flush chain containing sets first (y/n)?", "n");

            if (answer.ToLower().Contains("y"))
            {
                connector.flushChain(hostConfig.ChainName);
            }
            
            foreach (string listName in lists.Keys.Where(i => i != "COMBINED"))
            {
                Console.WriteLine($"Processing {listName}...");

                connector.createSet(listName);
                // Create set, but don't try to add elements if empty -->
                if (lists[listName].Count > 0)
                {
                    // Try to add in bulk first.
                    // If that fails (mostly due to overlapping intervals) add entries sequentially -->
                    bool result = connector.AddElementsParallel(listName, lists[listName]);

                    if (!result)
                    {
                        connector.AddElementsSequentially(listName, lists[listName]);
                    }
                }

                connector.createRuleReferencingSet(listName);

            }
        }

        private ListConfig getListConfig()
        {
            string listName = null;
            ListConfig foundList = null;
            bool correctList = false;
            
            do
            {
                listName = readEntry("list", "emergingthreats");
                foundList = ListConfigRepository.Get(listName);

                correctList = readEntry($"Found list with URL {foundList.URL_Str}. Correct?", "y").ToLower().Contains("y");
            } while (!correctList);

            return foundList;
        }
        
        private NftConfig getRemoteHostConfig()
        {
            string hostName = readEntry("hostname", null);
            
            HostConfigRepository hostConfigRepository = new HostConfigRepository();

            NftConfig testConfig = hostConfigRepository.GetConfig(hostName);

            if (testConfig.Empty)
            {
                askForInput(testConfig);
                
                while (!confirmInput(testConfig))
                {
                    askForInput(testConfig);
                }

                hostConfigRepository.SetConfig(testConfig);
            }
            else
            {
                if (!confirmInput(testConfig))
                {
                    Environment.Exit(1);
                }
                else
                {
                    Console.WriteLine($"Using config {testConfig.GetFormattedConfig(true)}...");
                }
            }

            return testConfig;

        }

        private void askForInput(NftConfig nftConfig)
        {
            nftConfig.HostName = readEntry("hostname", nftConfig.HostName);
            nftConfig.HostIP = readEntry("host ip", nftConfig.HostIP);
            nftConfig.UserName = readEntry("username", nftConfig.UserName);
            nftConfig.Password = readEntry("password", nftConfig.Password);
            nftConfig.CertPath = readEntry("certificate path", nftConfig.CertPath);
            nftConfig.TableName = readEntry("table name", nftConfig.TableName);
            nftConfig.ChainName = readEntry("chain name", nftConfig.ChainName);
            nftConfig.SetName = readEntry("set name", nftConfig.SetName);
            nftConfig.SupportsFlush = readEntry("supports flush", nftConfig.SupportsFlush ? "y" : "n").ToLower().Contains("y");
            nftConfig.UsePubkeyLogin = String.IsNullOrEmpty(nftConfig.CertPath) == false;
        }

        private bool confirmInput(NftConfig nftConfig)
        {
            string answer =
                readEntry($"Received this config:\r\n{nftConfig.GetFormattedConfig(false)}\r\n\r\nCorrect (y/n)?");

            return answer.ToLower().Contains("y");
        }

        private string readEntry(string entry, string defaultValue = null)
        {
            
            if (String.IsNullOrEmpty(defaultValue))
            {
                Console.Write($"{entry}: ");
//                Console.In.
            }
            else
            {
                Console.Write($"{entry} ({defaultValue}): ");    
            }
            
            string input = Console.ReadLine();
            while (Console.KeyAvailable)
            {
                Console.ReadKey(true);
            }
            if (string.IsNullOrEmpty(input))
            {
                return defaultValue;
            }
            else
            {
                return input;
            }
        }

        private void updateRemoteConfig(NftConfig nftConfig, List<string>lines)
        {
            NFT_SshConnector nftSshConnector = new NFT_SshConnector(nftConfig);
            nftSshConnector.ErrorAdded += nftSsh_ErrorAdded;
            nftSshConnector.OutputAdded += nftSsh_OutputAdded;
            
            Console.WriteLine($"Adding elements to nft host {nftConfig.HostName} at IP {nftConfig.HostIP}...");
//            int handle = nftSshConnector.findRuleHandle("", "inet filter input", "testset");
            bool result = nftSshConnector.AddElementsSequentially(null, lines);
        }

        private void nftSsh_ErrorAdded(object sender, EventArgs e)
        {
            NFT_SshConnector connector = (NFT_SshConnector) sender;
            Console.WriteLine($"ERROR ADDED: {connector.LastError}");
        }

        private void nftSsh_OutputAdded(object sender, EventArgs e)
        {
            NFT_SshConnector connector = (NFT_SshConnector) sender;
            Console.WriteLine($"OUTPUT ADDED: {connector.LastOutput}");
        }
        
        private async Task<Dictionary<string, List<string>>> fetchList(ListConfig listConfig)
        {
            string saveFile = Environment.GetEnvironmentVariable("HOME") + "/test/emerging_threats.txt";
            
            Console.WriteLine($"Trying to fetch list from {listConfig.Name} ({listConfig.URL})...");
            ListFetcher fetcher = new ListFetcher(listConfig);

            Console.WriteLine("Fetching list.");
            Task fetchTask = Task.Run(fetcher.FetchAndParse);

            while (!fetchTask.IsCompleted)
            {
                Console.Write(".");
                Thread.Sleep(500);
            }

            Console.WriteLine("Done fetching list.");

            fetcher.splitListInParts("#");
//            fetcher.DisplayLists();

//            List<string> lines = fetcher.RawLines;
            
//            fetcher.SaveRawToFile(saveFile);
//            List<string> lines = fetcher.Lines;

            return fetcher.Lists;
        }
    }
}