using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using fw_monitor.DataObjects;

namespace fw_monitor
{
    public class NFTManager
    {

        public async Task ManageLists(string listConfigName = null, string hostConfigName = null, bool interactive = true)
        {
            if (!interactive)
            {
                if (string.IsNullOrEmpty(listConfigName) || string.IsNullOrEmpty(hostConfigName))
                {
                    throw new ArgumentException("If not interactive configNames need to be set.");
                }
            }

            
            ListConfig listConfig = getListConfig(listConfigName, interactive);
            HostConfig hostConfig = getHostConfig(hostConfigName, interactive);
            
            if (listConfig == null)
            {
                
            }

            if (hostConfig == null)
            {
                
            }

            Dictionary<string, List<string>> lists = await fetchList(listConfig);
            
            NFT_SshConnector connector = new NFT_SshConnector(hostConfig);

            if (hostConfig.FlushChain)
            {
                connector.flushChain(hostConfig.ChainName);
            }
            connector.ErrorAdded += nftSsh_ErrorAdded;
            connector.OutputAdded += nftSsh_OutputAdded;
            
            
            foreach (string name in lists.Keys.Where(i => i != "COMBINED"))
            {
                Console.WriteLine($"Processing {name}...");

                connector.createSet(name);
                // Create set, but don't try to add elements if empty -->
                if (lists[name].Count > 0)
                {
                    // Try to add in bulk first.
                    // If that fails (mostly due to overlapping intervals) add entries sequentially -->
                    bool result = connector.AddElementsParallel(name, lists[name]);

                    if (!result)
                    {
                        connector.AddElementsSequentially(name, lists[name]);
                    }
                }

                connector.createRuleReferencingSet(name);
            }
        }

        private ListConfig getListConfig(string listName=null, bool interactive=false)
        {
            ListConfigRepository listConfigRepo = (ListConfigRepository) RepositoryBase.GetInstance(typeof(ListConfigRepository));
            if (!interactive)
            {
                return (ListConfig)listConfigRepo.Get(listName);
            }
            else
            {
                ListConfig foundList = null;

                if (ConsoleHelper.ReadInputAsBool("Create new (y/n)?", "n"))
                {
                    do
                    {
                        foundList = (ListConfig) listConfigRepo.CreateNew(listName);
                    }while(!ConsoleHelper.ReadInputAsBool($"Found list with URL {foundList.URL}. Correct(y/n)?", "y"));
                }
                else
                {
                    do
                    {

                        listName = ConsoleHelper.ReadInput("list", listName);
                        foundList = (ListConfig) listConfigRepo.Get(listName);

                    } while (!ConsoleHelper.ReadInputAsBool($"Found list with URL {foundList.URL}. Correct(y/n)?",
                        "y"));
                }

                return foundList;
            }
        }
        
        private HostConfig getHostConfig(string hostName=null, bool interactive=false)
        {
            HostConfigRepository hostConfigRepo = (HostConfigRepository)RepositoryBase.GetInstance(typeof(HostConfigRepository));
            if (!interactive)
            {
                return (HostConfig) hostConfigRepo.Get(hostName);
            }
            else
            {
                HostConfig foundConfig = null;
                
                do
                {
                    hostName = ConsoleHelper.ReadInput("hostname", hostName);
                    foundConfig = (HostConfig)hostConfigRepo.Get(hostName);
                    
                    if (foundConfig == null)
                    {
                        foundConfig = (HostConfig)hostConfigRepo.CreateNew(hostName);
                    }

                } while (!ConsoleHelper.ReadInputAsBool($"Found host {hostName} with IP {foundConfig.HostIP} and config {foundConfig.GetFormattedConfig(false)}. Correct(y/n)?", "y"));

                return foundConfig;
            }
        }

        private void updateRemoteConfig(HostConfig hostConfig, List<string>lines)
        {
            NFT_SshConnector nftSshConnector = new NFT_SshConnector(hostConfig);
            nftSshConnector.ErrorAdded += nftSsh_ErrorAdded;
            nftSshConnector.OutputAdded += nftSsh_OutputAdded;
            
            Console.WriteLine($"Adding elements to nft host {hostConfig.Name} at IP {hostConfig.HostIP}...");
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