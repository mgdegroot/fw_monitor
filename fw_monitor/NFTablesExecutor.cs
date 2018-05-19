using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using fw_monitor.DataObjects;
using RailwaySharp.ErrorHandling;

namespace fw_monitor
{

    public class NFTablesExecutor : IExecutor, IOutputProvider
    {
        private string preRuleset = string.Empty, 
            postRuleset = string.Empty;
        
        private readonly List<string> _errors = new List<string>();
        private readonly List<string> _output = new List<string>();
        private HostConfig _hostConfig;
        
        public NFTablesExecutor() {}
        public NFTablesExecutor(IConnector connector)
        {
            Connector = connector;
            
        }

        public enum MatchDirection
        {
            SOURCE,
            DEST,
        }

        public enum MatchAction
        {
            ACCEPT,
            COUNT,
            DROP,
            REJECT,
        }
        
        
        public IConnector Connector { get; set; }

        public HostConfig HostConfig
        {
            get { return _hostConfig; }
            set
            {
                _hostConfig = value;
                if (Connector != null)
                {
                    Connector.HostConfig = _hostConfig;
                }
                
            }
        }

        public ListConfig ListConfig { get; set; }

        public IEnumerable<string> Errors => _errors;
        public IEnumerable<string> Output => _output;

        public string LastError => Errors.Last();
        public string LastOutput => Output.Last();

        public event Action<IOutputProvider, string> ErrorAdded;
        public event Action<IOutputProvider, string> OutputAdded;
        
        public bool ProcessList(string name, IEnumerable<string>elements)
        {
            bool res = true;
            CreateSet(name);
            if (elements.Any())
            {
                res = AddElementsParallel(name, elements);
                if (!res)
                {
                    res = AddElementsSequentially(name, elements);
                }
            }
            
            CreateRuleReferencingSet(name, MatchDirection.SOURCE, MatchAction.DROP);
            
            return res;
        }
        
        public bool AddElement(string set, string element)
        {
            string cmd = $@"sudo nft add element {HostConfig.Table} {set} {{ {element} }}";
            return Connector.ExecuteCommand(cmd);

        }

        public bool AddElements(string set, IEnumerable<string> elements)
        {
            bool res = true;
            if (elements.Any())
            {
                res = AddElementsParallel(set, elements);
                if (!res)
                {
                    res = AddElementsSequentially(set, elements);
                }
            }

            return res;
        }

        public bool DoPreActions()
        {
            preRuleset = getCurrentRuleset();
            if (!queryChainExists())
            {
                CreateChain(HostConfig.Table, HostConfig.Chain);
            }

            if (HostConfig.FlushChain)
            {
                FlushChain();
            }

            return true;
        }

        public bool DoPostActions()
        {
            return true;
        }

        private void addError(string msg)
        {
            _errors.Add(msg);
            ErrorAdded?.Invoke(this, msg);
        }
        
        private void addOutput(string msg)
        {
            _output.Add(msg);
            OutputAdded?.Invoke(this, msg);
        }
        
        private bool AddElementsSequentially(string setName, IEnumerable<string> elements)
        {
            bool retVal = false;
            if (string.IsNullOrEmpty(setName))
            {
                setName = this.HostConfig.Set;
            }
            
            string cmdTemplate = $"sudo nft add element {HostConfig.Table} {setName} {{{{0}}}}";
            List<string> cmdList = new List<string>();
            foreach (string element in elements)
            {
                cmdList.Add($"sudo nft add element {HostConfig.Table} {setName} {{{element}}}");
            }
            
            List<bool> result = Connector.ExecuteCommands(cmdList) as List<bool>; 

            return result?.Exists(x => x == false) ?? false;
        }

        private bool AddElementsParallel(string setName, IEnumerable<string> elements)
        {
            if (string.IsNullOrEmpty(setName))
            {
                setName = this.HostConfig.Set;
            }

            string elemStr = string.Join(",", elements);

            string cmd = $"sudo nft add element {HostConfig.Table} {setName} {{ {elemStr} }}";
            
            bool result = Connector.ExecuteCommand(cmd);

            return result;            
        }
        
        

        private int FindRuleHandle(string fwConfig, string chain,string setName)
        {
            string ruleSignature = $"ip [s|d]addr @{setName}";
            string cmdNftList = $"sudo nft list chain {chain} -a";
            // TODO: grep regex \d{0-3} no match???
            // TODO: use pcregrep
            string cmdGrep = $@"grep ""ip [s|d]addr @{setName}"" | grep -Eo ""[0-9]?[0-9]?[0-9]""";
            
            string cmdCpl = $"{cmdNftList} | {cmdGrep}";
            (bool result, string output) = Connector.ExecuteQuery(cmdCpl);

            int.TryParse(output, out int handle);

            return handle;
        }

        private void FlushChain(string chain=null)
        {
            if (string.IsNullOrEmpty(chain))
            {
                chain = HostConfig.Chain;
            }
            
            string cmd = $"sudo nft flush chain {HostConfig.Table} {chain}";
            bool result = Connector.ExecuteCommand(cmd);
        }

        private void DeleteRule(string chain, int handle)
        {
            string cmd = $"sudo nft delete rule {chain} handle {handle}";
            bool result = Connector.ExecuteCommand(cmd);
        }

        private void DeleteSet(string chain, string setName)
        {
            string cmd = $"sudo nft delete set {chain} {setName}";
            bool result = Connector.ExecuteCommand(cmd);
        }

        private void CreateChain(string table, string chainName)
        {
            string cmd = $@"sudo nft add chain {table} {chainName}";
            bool result = Connector.ExecuteCommand(cmd);
        }

        private bool queryChainExists(string table=null, string chain=null)
        {
            if (table == null) table = HostConfig.Table;
            if (chain == null) chain = HostConfig.Chain;

            if (string.IsNullOrEmpty(preRuleset))
            {
                string qry = $@"sudo nft list chain {table} {chain}";
                (bool result, string output) = Connector.ExecuteQuery(qry);
                return !output.Contains("does not exist");
            }
            else
            {
                return preRuleset.Contains($"chain {chain} {{");
            }
        }

        private void CreateSet(string setName)
        {
            // NOTE: semicolon (;) needs to be escaped in Bash -->
            string cmd = $@"sudo nft create set {HostConfig.Table} {setName} {{type ipv4_addr\; flags interval\;}}";

            bool result = Connector.ExecuteCommand(cmd);
        }

        private void CreateRuleReferencingSet(string setName, MatchDirection direction=MatchDirection.SOURCE, MatchAction action=MatchAction.DROP)
        {
            string dirStr = direction == MatchDirection.DEST ? "daddr" : "saddr";
            string actStr = string.Empty;
            switch (action)
            {
                case MatchAction.ACCEPT:
                    actStr = "accept";
                    break;
                case MatchAction.DROP:
                default:
                    actStr = "drop";
                    break;
                    
            }
            
            string cmd = $@"sudo nft add rule {HostConfig.Table} {HostConfig.Chain} ip {dirStr} @{setName} {actStr}";
            bool result = Connector.ExecuteCommand(cmd);
        }
        
        private string getCurrentRuleset()
        {
            string query = $@"sudo nft list ruleset -a";
            (bool _, string output) = Connector.ExecuteQuery(query);

            return output;
        }


        
    }
}