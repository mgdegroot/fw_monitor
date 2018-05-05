using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using fw_monitor.DataObjects;

namespace fw_monitor
{
    
    public abstract class Repository : IRepository
    {
        protected string REPO_FILE_EXTENSION = ".json";
        protected string filenamePrefix = string.Empty;
        protected readonly Dictionary<string, Config> repository = new Dictionary<string, Config>();
        
        private static Dictionary<Type, Repository> _instances = new Dictionary<Type, Repository>();
        
        public static IRepository GetInstance(Type theType)
        {
            // TODO: implement singleton creation based on theType. Possibly directly using theType???
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
        
        public abstract Config this[string index]
        {
            get;
            set;
        }

        public abstract Config Get(string name);
        public abstract void Set(Config item);
        public abstract Config Create(string name);
        
        // TODO: config file option -->
        public bool SerializeToFile { get; set; } = true;
        // TODO: remove hardcoded string -->
        public string SerializePath { get; set; } = Environment.GetEnvironmentVariable("HOME") + "/NftHosts/";

        protected string getFilename(string name) =>
            Path.Combine(SerializePath, $"{filenamePrefix}_{name}{REPO_FILE_EXTENSION}");

        
        protected virtual string readFromFile(string path)
        {
            string content = File.ReadAllText(path, Encoding.UTF8);
            return content;
        }
    
        protected virtual void writeToFile(string path, string content)
        {
            FileInfo attributes = new FileInfo(path);
            if (!Directory.Exists(attributes.DirectoryName))
            {
                Directory.CreateDirectory(attributes.DirectoryName);
            }
            
            File.WriteAllText(path, content, Encoding.UTF8);
        }
    }
}