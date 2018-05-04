using System;
using System.Collections.Generic;
using System.Data;
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
            ListConfig listConfig = handleGetListConfig(listConfigName, interactive);
            HostConfig hostConfig = handleGetHostConfig(hostConfigName, interactive);
            
            if (listConfig == null)
            {
                throw new NoNullAllowedException("Got a nullzie herezies...")
                {
                    
                };
            }

            if (hostConfig == null)
            {
                throw new NoNullAllowedException("Got a nullzie herezies...")
                {
                    
                };
            }
            

            Dictionary<string, List<string>> lists = await fetchList(listConfig);
            
            SshConnector connector = new SshConnector(hostConfig);

            if (hostConfig.FlushChain)
            {
                connector.FlushChain(hostConfig.ChainName);
            }
            connector.ErrorAdded += nftSsh_ErrorAdded;
            connector.OutputAdded += nftSsh_OutputAdded;
            
            
            foreach (string name in lists.Keys.Where(i => i != "COMBINED"))
            {
                Console.WriteLine($"Processing {name}...");

                connector.CreateSet(name);
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

                connector.CreateRuleReferencingSet(name);
            }
        }

        private ListConfig handleGetListConfig(string listName=null, bool interactive=false)
        {
            ListConfigRepository listConfigRepo = (ListConfigRepository) Repository.GetInstance(typeof(ListConfigRepository));
            if (!interactive)
            {
                if (string.IsNullOrEmpty(listName))
                {
                    throw new ArgumentNullException("listName needs to be set in non-interactive mode.");
                }
                return (ListConfig)listConfigRepo.Get(listName);
            }
            else
            {
                ListConfig foundList = null;
                Console.WriteLine("List config retrieval / creation...");
                do
                {
                    if (ConsoleHelper.ReadInputAsBool("Create new (y/n)", "n"))
                    {
                        foundList = (ListConfig) listConfigRepo.CreateNew(listName);
                    }
                    else
                    {
                        listName = ConsoleHelper.ReadInput("list name", listName);
                        foundList = (ListConfig) listConfigRepo.Get(listName);
                    }
                }
                while (!ConsoleHelper.ReadInputAsBool($"Found list with URL {foundList.URL}. Correct(y/n)", "y"));

                if (ConsoleHelper.ReadInputAsBool("Serialize list config (y/n)", "y"))
                {
                    listConfigRepo[foundList.Name] = foundList;
                }
                return foundList;
            }
        }

        private void handleSaveConfig(Config config)
        {
            switch (config)
            {
                case ListConfig lc:
                    break;
                case HostConfig hc:
                    break;
                default:
                    throw new NotSupportedException($"Config {config.GetType()} not yet supported.");
            }
        }
        
        private HostConfig handleGetHostConfig(string hostName=null, bool interactive=false)
        {
            HostConfigRepository hostConfigRepo = (HostConfigRepository)Repository.GetInstance(typeof(HostConfigRepository));
            if (!interactive)
            {
                if (string.IsNullOrEmpty(hostName))
                {
                    throw new ArgumentNullException("hostName needs to be set in non-interactive mode.");
                }
                return (HostConfig) hostConfigRepo.Get(hostName);
            }
            else
            {
                HostConfig foundHost = null;
                Console.WriteLine("Host config retrieval / creation...");
                do
                {
                    if (ConsoleHelper.ReadInputAsBool("Create new (y/n)", "n"))
                    {
                        foundHost = (HostConfig) hostConfigRepo.CreateNew(hostName);
                    }
                    else
                    {
                        hostName = ConsoleHelper.ReadInput("host name", hostName);
                        foundHost = (HostConfig) hostConfigRepo.Get(hostName);
                    }
                }
                while (!ConsoleHelper.ReadInputAsBool($"Found host {hostName} with IP {foundHost.HostIP} and config {foundHost.GetFormattedConfig(false)}. Correct(y/n)", "y"));

                if (ConsoleHelper.ReadInputAsBool("Serialize host config (y/n)", "y"))
                {
                    hostConfigRepo[foundHost.Name] = foundHost;
                }
                
                return foundHost;
            }
        }
        
        private void updateRemoteConfig(HostConfig hostConfig, List<string>lines)
        {
            SshConnector sshConnector = new SshConnector(hostConfig);
            sshConnector.ErrorAdded += nftSsh_ErrorAdded;
            sshConnector.OutputAdded += nftSsh_OutputAdded;
            
            Console.WriteLine($"Adding elements to nft host {hostConfig.Name} at IP {hostConfig.HostIP}...");
//            int handle = nftSshConnector.findRuleHandle("", "inet filter input", "testset");
            bool result = sshConnector.AddElementsSequentially(null, lines);
        }

        private void nftSsh_ErrorAdded(object sender, EventArgs e)
        {
            SshConnector connector = (SshConnector) sender;
            Console.WriteLine($"ERROR ADDED: {connector.LastError}");
        }

        private void nftSsh_OutputAdded(object sender, EventArgs e)
        {
            SshConnector connector = (SshConnector) sender;
            Console.WriteLine($"OUTPUT ADDED: {connector.LastOutput}");
        }
        
        private async Task<Dictionary<string, List<string>>> fetchList(ListConfig listConfig)
        {
//            string saveFile = Environment.GetEnvironmentVariable("HOME") + "/test/emerging_threats.txt";
            
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

            return fetcher.Lists;
        }
    }
}