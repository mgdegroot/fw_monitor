using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using fw_monitor.DataObjects;

namespace fw_monitor
{
    
    public class Repository : IRepository
    {
        protected string REPO_FILE_EXTENSION = Util.Extension.JSON;
        protected string filenamePrefix = string.Empty;
        protected readonly Dictionary<string, Config> repository = new Dictionary<string, Config>();
        
        protected IUtil _util;
        protected ICreator _creator;

        private static Dictionary<Type, IRepository> _instances = new Dictionary<Type, IRepository>()
        {
            {typeof(ListConfigRepository), new ListConfigRepository()},
            {typeof(HostConfigRepository), new HostConfigRepository()},
            {typeof(ListRepository), new ListRepository()},
        };


        public Repository()
        {
            this._util = new Util();
        }
        
        public Repository(IUtil util)
        {
            this._util = util;
        }
        
        // TODO: config file option -->
        public bool SerializeToFile { get; set; } = true;
        // TODO: remove hardcoded string -->
        public string SerializePath { get; set; } = Util.SerializePath;

        public virtual ICreator Creator
        {
            get => _creator;
            set => _creator = value;
        }

        public virtual IRepositoryItem this[string index]
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException(); 
        }
        
        
        
        public IRepository GetInstance(Type theType)
        {
            _instances.TryGetValue(theType, out IRepository repo);
            return repo;
        }
        
        public virtual IRepositoryItem Get(string name) => throw new NotImplementedException();
        public virtual void Set(IRepositoryItem item) =>throw new NotImplementedException();
//        public virtual IRepositoryItem Create(string name) => throw new NotImplementedException();

        protected string getFilename(string name) =>
            Path.Combine(SerializePath, $"{filenamePrefix}_{name}{REPO_FILE_EXTENSION}");

        
        protected virtual string readFromFile(string path)
        {
            string content = File.ReadAllText(path, Encoding.UTF8);
            return content;
        }
    
        protected virtual void writeToFile(string path, string content)
        {
            new Util().WriteToFile(path, content, false);
        }
    }
}