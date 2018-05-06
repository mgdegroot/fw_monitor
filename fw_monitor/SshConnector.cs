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
        private ConnectionInfo _connectionInfo;
        private readonly List<string> _errors = new List<string>();
        private readonly List<string> _output = new List<string>();
        private HostConfig _hostConfig;


        public SshConnector()
        {
        }

        public SshConnector(HostConfig hostConfig)
        {
            this.HostConfig = hostConfig;

        }

        public HostConfig HostConfig
        {
            get => _hostConfig;
            set
            {
                _hostConfig = value;
                if (HostConfig.UsePubkeyLogin)
                {
                    _connectionInfo = new ConnectionInfo(host: HostConfig.HostIP, username: HostConfig.Username, new PrivateKeyAuthenticationMethod(HostConfig.CertPath) );
                }
                else
                {
                    _connectionInfo = new ConnectionInfo(host: HostConfig.HostIP, username: HostConfig.Username,
                        new PasswordAuthenticationMethod(username: HostConfig.Username, password: HostConfig.Password));
                }
            }
        }

        public IEnumerable<string> Errors => _errors;
        public IEnumerable<string> Output => _output;

        public string LastError => Errors.Last();
        public string LastOutput => Output.Last();

        public event EventHandler ErrorAdded;
        public event EventHandler OutputAdded;
        
        public void Connect()
        {
            
            throw new NotImplementedException();
        }

        public bool ExecuteCommand(string command)
        {

            (bool result, string _) = execSshCommand(command);
            return result;
        }

        public IEnumerable<bool> ExecuteCommands(IEnumerable<string> commands)
        {
            List<(bool result, string output)> result = execSshCommands(commands) as List<(bool, string)>;
            return result.Select(i => i.result) as bool[];
        }

        public (bool, string) ExecuteQuery(string query)
        {
            (bool result, string output) result = execSshCommand(query);
            return result;
        }

        public IEnumerable<(bool, string)> ExecuteQueries(IEnumerable<string> queries)
        {
            List<(bool result, string output)> result = execSshCommands(queries) as List<(bool, string)>;
            return result;
        }


        public virtual void OnErrorAdded(EventArgs e)
        {
            ErrorAdded?.Invoke(this, e);
        }

        public virtual void OnOutputAdded(EventArgs e)
        {
            OutputAdded?.Invoke(this, e);
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
        
 


        private IEnumerable<(bool, string)> execSshCommands(IEnumerable<string> commands)
        {
            List<(bool,string)> retVal = new List<(bool, string)>(commands.Count());
            
            SshCommand commandResult = null;
            using (SshClient ssh = new SshClient(_connectionInfo))
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
            using (SshClient ssh = new SshClient(_connectionInfo))
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
            _errors.Add(value);
            OnErrorAdded(EventArgs.Empty);
        }

        private void addOutput(string value)
        {
            _output.Add(value);
            OnOutputAdded(EventArgs.Empty);
        }
    }
}