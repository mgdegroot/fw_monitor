using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace fw_monitor
{
    public class Repository<T> : IRepository<T> where T : class, IRepositoryItem, new()
    {
        private IUtil _util = new Util();
        private Dictionary<string, T> _repository = new Dictionary<string, T>();

//        private static Dictionary<Type, IRepository> _repoInstances = new Dictionary<Type, IRepository>()
//        {
//            {typeof(ListConfigRepository), new ListConfigRepository()},
//            {typeof(HostConfigRepository), new HostConfigRepository()},
//            {typeof(ListRepository), new ListRepository()},
//        };
        
        
        public Repository()
        {
        }

        public Repository(IUtil util) : this()
        {
            _util = util;
        }

        // TODO: BackingStore use is not yet generic / interface based -->
        public Util.BackingStore BackingStore { get; set; } = Util.BackingStoreType;
        public bool SerializeToFile { get; set; } = true;
        public string SerializePath { get; set; }= Util.SerializePath;

        public T this[string key]
        {
            get => Get(key);
            set => Set(value);
        }
        
        public S GetInstance<S>() where S: IRepository<S>, new()
        {
            S instance = new S();
            return instance;
        }
        
        public ICreator Creator { get; set; } = new T().Creator;

        public T Get(string key)
        {
            _repository.TryGetValue(key, out T value);

            if (value == null && BackingStore != Util.BackingStore.NONE)
            {
                string path = getFilename(key);

                if (File.Exists(path))
                {
                    string strValue = readFromFile(path);
                    value = deserialize(strValue);
                    _repository.Add(key, value);
                }
                else
                {
                    return null;
                }
            }
            
            return value;
        }

        public void Set(T value)
        {
            _repository[value.Name] = value;

            if (BackingStore == Util.BackingStore.FILE)
            {
                string strValue = serialize(value);
                writeToFile(getFilename(value.Name), strValue);
            }
        }

        public T Create(string key)
        {
            T created = new T();
            created.Name = key;

            return created;
        }

        private string serialize(T repoItem)
        {
            MemoryStream memoryStream = new MemoryStream();
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
            try
            {
                serializer.WriteObject(memoryStream, repoItem);
            }
            catch (Exception ex)
            {
                return null;
            }
            
            byte[] json = memoryStream.ToArray();
            memoryStream.Close();
            return Encoding.UTF8.GetString(json, 0, json.Length);
        }

        private T deserialize(string json)
        {
            MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
            T repoItem = (T)serializer.ReadObject(memoryStream) as T;

            return repoItem;
        }

        private string getFilename(string key)
        {
            return Path.Combine(SerializePath, $"{typeof(T).Name}_{key}{Util.Extension.JSON}");
        }

        private string readFromFile(string path)
        {
            return _util.ReadFromFile(path, false);
        }

        private void writeToFile(string path, string content)
        {
            _util.WriteToFile(path, content, false);
        }
        
        
        
    }
}