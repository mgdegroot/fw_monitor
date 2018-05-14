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

    // TODO: Info for manager should be passed as interface-based object, not specific use-case parameters.
    public class ManageInfo
    {
        
    }
    
    public class NFTManager : IManager
    {
//        public IRepository Repository { get; set; } = new Repository();
        
        // TODO: not really proper to add them here just to facilitate unit tests...
        public IExecutor Executor { get; set; }
//        public ListConfig ListConfig { get; set; }
//        public HostConfig HostConfig { get; set; }
        public IListFetcher ListFetcher { get; set; }
        public IRepository<ListConfig>ListConfigRepository { get; set; } = new Repository<ListConfig>();
        public IRepository<HostConfig>HostConfigRepository { get; set; } = new Repository<HostConfig>();
        public IRepository<ContentList>ListRepository { get; set; } = new Repository<ContentList>();
        public IUtil Utility { get; set; } = new Util();

        public async Task ManageLists() => ManageLists(null, null);

        public async Task ManageLists(string listConfigName, string hostConfigName, bool interactive = true)
        {
            ListConfig listConfig = handleGetListConfig(listConfigName, interactive);
            if (listConfig == null)
            {
                throw new NoNullAllowedException("Got null for a ListConfig...")
                {
                    
                };
            }
            
            HostConfig hostConfig = handleGetHostConfig(hostConfigName, interactive);
            if (hostConfig == null)
            {
                throw new NoNullAllowedException("Got null for a HostConfig...")
                {
                    
                };
            }

            if (interactive)
            {
                Console.WriteLine($"Got Listconfig as {listConfig}\nGot Hostconfig as {hostConfig}\n.");
                if (!ConsoleHelper.ReadInputAsBool("Proceed with fetch (y/n)", "y"))
                {
                    return;
                }
            }

            ManageLists(listConfig, hostConfig);
            
        }
        
        public async Task ManageLists(ListConfig listConfig, HostConfig hostConfig)
        {
            Dictionary<string, List<string>> lists = await fetchList(listConfig);
            
            //

            handleSaveFetchedLists(listConfig, lists);
            
            //

            Executor.Connector.ErrorAdded += nftSsh_ErrorAdded;
            Executor.Connector.OutputAdded += nftSsh_OutputAdded;

            Executor.ErrorAdded += nftSsh_ErrorAdded;
            Executor.OutputAdded += nftSsh_OutputAdded;

            Executor.HostConfig = hostConfig;
            Executor.ListConfig = listConfig;

            bool actionResult = Executor.DoPreActions();
            
            
            foreach (string name in lists.Keys.Where(i => i != Util.MAINLISTNAME))
            {
                Console.WriteLine($"Processing {name}...");
                Executor.ProcessList(name, lists[name]);
            }
        }

        private void handleSaveFetchedLists(ListConfig listConfig, Dictionary<string, List<string>> lists)
        {
            foreach (KeyValuePair<string,List<string>> kvp in lists)
            {
//                GenericRepository<ContentList> listRepository = new GenericRepository<ContentList>();
                //ListRepository listRepository = (ListRepository) Repository.GetInstance(typeof(ListRepository));
                
//                List<string> elements = kvp.Value;
                ContentList cl = new ContentList()
                {
                    Name=kvp.Key,
                    Version = listConfig.Version,
                    IsSubList = (kvp.Key != Util.MAINLISTNAME),
                    Elements = kvp.Value,
                };
                
                
                ListRepository.Set(cl);
            }
        }

        private ListConfig handleGetListConfig(string listName=null, bool interactive=false)
        {
//            GenericRepository<ListConfig> listConfigRepo = new GenericRepository<ListConfig>();
//            ListConfigRepository listConfigRepo = (ListConfigRepository) Repository.GetInstance(typeof(ListConfigRepository));
            if (!interactive)
            {
                if (string.IsNullOrEmpty(listName))
                {
                    return null;
                }
                return (ListConfig)ListConfigRepository.Get(listName);
            }
            else
            {
                ListConfig foundList = null;
                bool newlyCreated = false;
                Console.WriteLine("List config retrieval / creation...");
                
                string whileMsg;
                do
                {
                    if (ConsoleHelper.ReadInputAsBool("Create new (y/n)", "n"))
                    {
                        foundList = (ListConfig) ListConfigRepository.Creator.Create(listName);
                        whileMsg = $"Created list with name {foundList?.Name}. Correct (y/n)";
                        newlyCreated = true;
                    }
                    else
                    {
                        listName = ConsoleHelper.ReadInput("list name", listName);
                        foundList = (ListConfig) ListConfigRepository.Get(listName);
                        if (foundList == null)
                        {
                            whileMsg = $"No list found by the name of {listName}. Exit (y/n)?";
                        }
                        else
                        {
                            whileMsg = $"Found list with name {foundList.Name}. Correct(y/n)";
                            newlyCreated = false;
                        }
                        
                    }
                }
                while (!ConsoleHelper.ReadInputAsBool(whileMsg, "y"));

                if (newlyCreated && ConsoleHelper.ReadInputAsBool("Serialize list config (y/n)", "y"))
                {
                    ListConfigRepository[foundList.Name] = foundList;
                }
                
                return foundList;
            }
        }

        private HostConfig handleGetHostConfig(string hostName=null, bool interactive=false)
        {
//            GenericRepository<HostConfig> hostConfigRepo = new GenericRepository<HostConfig>();
            //HostConfigRepository hostConfigRepo = (HostConfigRepository)Repository.GetInstance(typeof(HostConfigRepository));
            if (!interactive)
            {
                if (string.IsNullOrEmpty(hostName))
                {
                    return null;
                }
                return (HostConfig) HostConfigRepository.Get(hostName);
            }
            else
            {
                HostConfig foundHost = null;
                bool newlyCreated = false;
                Console.WriteLine("Host config retrieval / creation...");

                string whileMsg;
                do
                {
                    if (ConsoleHelper.ReadInputAsBool("Create new (y/n)", "n"))
                    {
                        foundHost = (HostConfig) HostConfigRepository.Creator.Create(hostName);
                        whileMsg = $"Created list with name {foundHost?.Name}. Correct (y/n)";
                        newlyCreated = true;
                    }
                    else
                    {
                        hostName = ConsoleHelper.ReadInput("host name", hostName);
                        foundHost = (HostConfig) HostConfigRepository.Get(hostName);
                        if (foundHost == null)
                        {
                            whileMsg = $"No host found by the name of {hostName}. Exit (y/n)";
                        }
                        else
                        {
                            whileMsg = $"Found host with name {foundHost.Name}. Correct (y/n)";
                            newlyCreated = false;
                        }
                    }
                }
                while (!ConsoleHelper.ReadInputAsBool(whileMsg, "y"));

                if (newlyCreated && ConsoleHelper.ReadInputAsBool("Serialize host config (y/n)", "y"))
                {
                    HostConfigRepository[foundHost.Name] = foundHost;
                }
                
                return foundHost;
            }
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
            if (ListFetcher == null)
            {
                ListFetcher = new ListFetcher(listConfig);
            }
            else
            {
                ListFetcher.ListConfig = listConfig;
            }
//            string saveFile = Environment.GetEnvironmentVariable("HOME") + "/test/emerging_threats.txt";
            
            Console.WriteLine($"Trying to fetch list from {listConfig.Name} ({listConfig.URL})...");

            Console.WriteLine("Fetching list.");
            Task fetchTask = Task.Run(ListFetcher.FetchAndParse);

            while (!fetchTask.IsCompleted)
            {
                Console.Write(".");
                Thread.Sleep(500);
            }

            Console.WriteLine("Done fetching list.");

            return ListFetcher.Lists;
        }
    }
}