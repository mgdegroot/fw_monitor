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
    public class NFT_SshConnector
    {
        public HostConfig HostConfig { get; set; }

        
        public bool Empty => HostConfig.Empty; 
        public string HostName => HostConfig.Name;
        public string HostIP => HostConfig.HostIP;
        public bool ConnectUsingIP => HostConfig.ConnectUsingIP;
        public string Username => HostConfig.UserName;
        public string Password => HostConfig.Password; 

        // TODO: pubkey auth implementation -->
        public string CertPath => HostConfig.CertPath; 
        public bool UsePubkeyLogin => HostConfig.UsePubkeyLogin;
        public string Table => HostConfig.TableName;
        public string Chain => HostConfig.ChainName;
        public string Set => HostConfig.SetName;
        public bool SupportsFlush => HostConfig.SupportsFlush;

        public IEnumerable<string> Errors => errors;
        public IEnumerable<string> Output => output;

        public string LastError => Errors.Last();
        public string LastOutput => Output.Last();

        public event EventHandler ErrorAdded;
        public event EventHandler OutputAdded; 

        private readonly ConnectionInfo connectionInfo;
        private readonly List<string> errors = new List<string>();
        private readonly List<string> output = new List<string>();

        public virtual void OnErrorAdded(EventArgs e)
        {
            ErrorAdded?.Invoke(this, e);
        }

        public virtual void OnOutputAdded(EventArgs e)
        {
            OutputAdded?.Invoke(this, e);
        }

        
        enum RemoteAction
        {
            FLUSH,
            FLUSH_AND_REFRESH,
            
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

        public NFT_SshConnector(HostConfig hostConfig)
        {
            this.HostConfig = hostConfig;
            if (HostConfig.UsePubkeyLogin)
            {
                connectionInfo = new ConnectionInfo(host: HostConfig.HostIP, username: HostConfig.UserName, new PrivateKeyAuthenticationMethod(HostConfig.CertPath) );
            }
            else
            {
                connectionInfo = new ConnectionInfo(host: HostConfig.HostIP, username: HostConfig.UserName,
                    new PasswordAuthenticationMethod(username: HostConfig.UserName, password: HostConfig.Password));
            }
        }


//        public NFT_SshConnector(string remotehost, string username, string password, string certPath=null, AuthMethod authMethod=AuthMethod.PASSWORD, string table="inet filter", string chain="input", string set="wan_blocks")
//        {
//            HostName = remotehost;
//            Username = username;
//            Password = password;
//            Table = table;
//            Chain = chain;
//            Set = set;
//
//            if (authMethod == AuthMethod.PASSWORD)
//            {
//                connectionInfo = new ConnectionInfo(host: remotehost, username: username,
//                    new PasswordAuthenticationMethod(username: username, password: password));    
//            }
//            else
//            {
//                connectionInfo = new ConnectionInfo(host: remotehost, username: username, new PrivateKeyAuthenticationMethod(certPath) );
//            }
//            
//        }
        

        public void TestConnect()
        {
            if (ConnectUsingIP && string.IsNullOrEmpty(HostIP))
            {
                throw new ArgumentException("Host IP not set");
            }

            if (!ConnectUsingIP && string.IsNullOrEmpty(HostName))
            {
                throw new ArgumentException("Host name not set");
            }

            using (SshClient ssh = new SshClient(ConnectUsingIP ? HostIP : HostName, Username, Password))
            {
                ssh.Connect();
                var result = ssh.RunCommand("ls -lsa");
                ssh.Disconnect();
                Console.WriteLine(result.Result);
            }
            
        }

        

        public void orchestrateActions()
        {
            throw new NotImplementedException("nog niet");
        }

        public bool AddElementsSequentially(string setName, IEnumerable<string> elements)
        {
            if (string.IsNullOrEmpty(setName))
            {
                setName = this.Set;
            }
            string cmdTemplate = $"sudo nft add element {Table} {setName} {{{{0}}}}";
            List<string> cmdList = new List<string>();
            foreach (string element in elements)
            {
                cmdList.Add($"sudo nft add element {Table} {setName} {{{element}}}");
            }

            (bool result, string output) = execSshCommands(cmdList);

            return result;
        }

        public bool AddElementsParallel(string setName, IEnumerable<string> elements)
        {
            if (string.IsNullOrEmpty(setName))
            {
                setName = this.Set;
            }

            string elemStr = string.Join(",", elements);
//            elemStr = "{" + elemStr + "}";
            string cmd = $"sudo nft add element {Table} {setName} {{ {elemStr} }}";
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

            if (string.IsNullOrEmpty(Username))
            {
                errors.Add("Username is empty");
            }
            
            if (UsePubkeyLogin && string.IsNullOrEmpty(CertPath))
            {
                errors.Add($"UsePubkeyLogin is true but CertPath is empty");
            }

            if (!UsePubkeyLogin && string.IsNullOrEmpty(Password))
            {
                errors.Add($"Password login but Password is empty");
            }
            
            return (errors.Count == 0, errors);
        }
        
 
        public int findRuleHandle(string fwConfig, string chain,string setName)
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

        public void flushChain(string chain)
        {
            if (string.IsNullOrEmpty(chain))
            {
                chain = Chain;
            }
            
            string cmd = $"sudo nft flush chain {Table} {chain}";
            (bool _, string _) = execSshCommand(cmd);
        }

        public void deleteRule(string chain, int handle)
        {
            string cmd = $"sudo nft delete rule {chain} handle {handle}";

            (bool _, string _) = execSshCommand(cmd);
        }

        public void deleteSet(string chain, string setName)
        {
            string cmd = $"sudo nft delete set {chain} {setName}";

            (bool _, string _) = execSshCommand(cmd);
        }

        public void createChain(string table, string chainName)
        {
            string cmd = $@"sudo nft add chain {table} {chainName}";

            (bool _, string _) = execSshCommand(cmd);
        }

        public void createSet(string setName)
        {
            // NOTE: semicolon (;) needs to be escaped in Bash -->
            string cmd = $@"sudo nft create set {Table} {setName} {{type ipv4_addr\; flags interval\;}}";

            (bool _, string _) = execSshCommand(cmd);
        }

        public void createRuleReferencingSet(string setName, MatchDirection direction=MatchDirection.SOURCE, MatchAction action=MatchAction.DROP)
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
            
            string cmd = $@"sudo nft add rule {Table} {Chain} ip {dirStr} @{setName} {actStr}";

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

        private (bool, string) execSshCommands(IEnumerable<string> commands)
        {
            (bool success, string output) retVal = (false, string.Empty);
            SshCommand commandResult = null;
            using (SshClient ssh = new SshClient(connectionInfo))
            {
                ssh.Connect();
                foreach (string command in commands)
                {
                    commandResult = ssh.RunCommand(command);
                    retVal = handleSshCommandResult(commandResult);

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