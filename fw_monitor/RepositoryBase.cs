using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using fw_monitor.DataObjects;

namespace fw_monitor
{
    
    public abstract class RepositoryBase : IRepository
    {
        protected string REPO_FILE_EXTENSION = ".json";
        
        // TODO: move to config file -->
        protected string REPO_BASE_PATH = Environment.GetEnvironmentVariable("HOME") + "/NftHosts/";

        public static IRepository GetInstance(Type theType)
        {
            switch (theType.Name)
            {
                case nameof(ListConfigRepository):
                    return new ListConfigRepository();
                case nameof(HostConfigRepository):
                    return new HostConfigRepository();
                default:
                    return null;
            }
        }
        
        protected virtual string readFromFile(string path)
        {
            string content = File.ReadAllText(path, Encoding.UTF8);
            return content;
        }
    
        protected virtual void writeToFile(string path, string content)
        {
            File.WriteAllText(path, content, Encoding.UTF8);
        }


//        public abstract IRepository Instance { get; }
        public abstract Dictionary<string, Config> Repository { get; set; }
        public abstract Config Get(string name);
        public abstract void Set(Config item);

        public abstract Config CreateNew(string name);
    }
}