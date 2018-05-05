using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security;
using fw_monitor.DataObjects;
using Renci.SshNet;

namespace fw_monitor
{
    public class SshConnector : IConnector
    {
        private readonly ConnectionInfo connectionInfo;
        private readonly List<string> errors = new List<string>();
        private readonly List<string> output = new List<string>();
        
        public void Connect()
        {
            
            throw new NotImplementedException();
        }

        public bool ExecuteCommand(string command)
        {

            (bool result, string output) = execSshCommand(command);
            return result;
        }

        public bool[] ExecuteCommands(IEnumerable<string> commands)
        {
            List<(bool result, string output)> result = execSshCommands(commands) as List<(bool, string)>;
            return result.Select(i => i.result) as bool[];
            
        }

        public HostConfig HostConfig { get; set; }

        public IEnumerable<string> Errors => errors;
        public IEnumerable<string> Output => output;

        public string LastError => Errors.Last();
        public string LastOutput => Output.Last();

        public event EventHandler ErrorAdded;
        public event EventHandler OutputAdded; 



        public virtual void OnErrorAdded(EventArgs e)
        {
            ErrorAdded?.Invoke(this, e);
        }

        public virtual void OnOutputAdded(EventArgs e)
        {
            OutputAdded?.Invoke(this, e);
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

        public SshConnector(HostConfig hostConfig)
        {
            this.HostConfig = hostConfig;
            if (HostConfig.UsePubkeyLogin)
            {
                connectionInfo = new ConnectionInfo(host: HostConfig.HostIP, username: HostConfig.Username, new PrivateKeyAuthenticationMethod(HostConfig.CertPath) );
            }
            else
            {
                connectionInfo = new ConnectionInfo(host: HostConfig.HostIP, username: HostConfig.Username,
                    new PasswordAuthenticationMethod(username: HostConfig.Username, password: HostConfig.Password));
            }
        }

        public bool AddElementsSequentially(string setName, IEnumerable<string> elements)
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

            List<(bool result, string output)> res = execSshCommands(cmdList) as List<(bool,string)>;

            return res.Exists(x => x.result == false);

        }

        public bool AddElementsParallel(string setName, IEnumerable<string> elements)
        {
            if (string.IsNullOrEmpty(setName))
            {
                setName = this.HostConfig.Set;
            }

            string elemStr = string.Join(",", elements);
//            elemStr = "{" + elemStr + "}";
            string cmd = $"sudo nft add element {HostConfig.Table} {setName} {{ {elemStr} }}";
            (bool result, string output) = execSshCommand(cmd);
//            if (!result)
//            {
//                addError(output);
//            }

            return result;            
        }

        private (bool, IEnumerable<string>) checkSetup()
        {
            List<string> errors = new List<string>();

            if (string.IsNullOrEmpty(HostConfig.Username))
            {
                errors.Add("Username is empty");
            }
            
            if (HostConfig.UsePubkeyLogin && string.IsNullOrEmpty(HostConfig.CertPath))
            {
                errors.Add($"UsePubkeyLogin is true but CertPath is empty");
            }

            if (!HostConfig.UsePubkeyLogin && string.IsNullOrEmpty(HostConfig.Password))
            {
                errors.Add($"Password login but Password is empty");
            }
            
            return (errors.Count == 0, errors);
        }
        
 
        public int FindRuleHandle(string fwConfig, string chain,string setName)
        {
            string ruleSignature = $"ip [s|d]addr @{setName}";
            string cmdNftList = $"sudo nft list chain {chain} -a";
            // TODO: grep regex \d{0-3} no match???
            // TODO: use pcregrep
            string cmdGrep = $@"grep ""ip [s|d]addr @{setName}"" | grep -Eo ""[0-9]?[0-9]?[0-9]""";
            
            string cmdCpl = $"{cmdNftList} | {cmdGrep}";

            (bool result, string output) = execSshCommand(cmdCpl);

            int.TryParse(output, out int handle);

            return handle;
        }

        public void FlushChain(string chain)
        {
            if (string.IsNullOrEmpty(chain))
            {
                chain = HostConfig.Chain;
            }
            
            string cmd = $"sudo nft flush chain {HostConfig.Table} {chain}";
            (bool _, string _) = execSshCommand(cmd);
        }

        public void DeleteRule(string chain, int handle)
        {
            string cmd = $"sudo nft delete rule {chain} handle {handle}";

            (bool _, string _) = execSshCommand(cmd);
        }

        public void DeleteSet(string chain, string setName)
        {
            string cmd = $"sudo nft delete set {chain} {setName}";

            (bool _, string _) = execSshCommand(cmd);
        }

        public void CreateChain(string table, string chainName)
        {
            string cmd = $@"sudo nft add chain {table} {chainName}";

            (bool _, string _) = execSshCommand(cmd);
        }

        public void CreateSet(string setName)
        {
            // NOTE: semicolon (;) needs to be escaped in Bash -->
            string cmd = $@"sudo nft create set {HostConfig.Table} {setName} {{type ipv4_addr\; flags interval\;}}";

            (bool _, string _) = execSshCommand(cmd);
        }

        public void CreateRuleReferencingSet(string setName, MatchDirection direction=MatchDirection.SOURCE, MatchAction action=MatchAction.DROP)
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

            (bool _, string _) = execSshCommand(cmd);
        }

        
//        private void createRule(string table, string chain, string setName, MatchDirection direction, MatchAction action)
//        {
//            string dirStr = direction == MatchDirection.DEST ? "daddr" : "saddr";
//            string actStr = action == MatchAction.ACCEPT ? "accept" : "drop";
//            
//            string cmd = $@"nft add rule {table} {chain} ip {dirStr} @{setName} {actStr}";
//
//            (bool _, string _) = execSshCommand(cmd);
//        }

        private IEnumerable<(bool, string)> execSshCommands(IEnumerable<string> commands)
        {
            List<(bool,string)> retVal = new List<(bool, string)>(commands.Count());
            
            SshCommand commandResult = null;
            using (SshClient ssh = new SshClient(connectionInfo))
            {
                ssh.Connect();
                foreach (string command in commands)
                {
                    commandResult = ssh.RunCommand(command);
                    retVal.Add(handleSshCommandResult(commandResult));
                }
                ssh.Disconnect();
            }

            return retVal;
        }

        private (bool, string) execSshCommand(string command)
        {
            (bool success, string output) retVal;
            SshCommand commandResult = null;
            using (SshClient ssh = new SshClient(connectionInfo))
            {
                ssh.Connect();
                commandResult = ssh.RunCommand(command);
                ssh.Disconnect();
            }

            retVal = handleSshCommandResult(commandResult);


            return retVal;
        }

        private (bool, string) handleSshCommandResult(SshCommand sshCommand)
        {
            (bool success, string output) retVal;
            
            if (sshCommand == null)
            {
                string err = "sshCommand is null";
                addError(err);
                retVal.success = false;
                retVal.output = err;
            }
            else
            {
                if (!string.IsNullOrEmpty(sshCommand.Error))
                {
                    addError($"{sshCommand.CommandText}: {sshCommand.Error}");
                    retVal.success = false;
                }

                if (!string.IsNullOrEmpty(sshCommand.Result))
                {
                    addOutput($"{sshCommand.CommandText}: {sshCommand.Result}");
                }

                retVal.success = string.IsNullOrEmpty(sshCommand.Error);
                retVal.output = $"ERROR: {sshCommand.Error} - RESULT: {sshCommand.Result}";
                
            }

            return retVal;
        }

        private void addError(string value)
        {
            errors.Add(value);
            OnErrorAdded(EventArgs.Empty);
        }

        private void addOutput(string value)
        {
            output.Add(value);
            OnOutputAdded(EventArgs.Empty);
        }
    }
}