using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace fw_monitor.DataObjects
{
    public class ContentList : Repository, IContentList, IEnumerable<string>
    {
        private List<string> _content = new List<string>();

        public ContentList() => filenamePrefix = "list";
        public string Name { get; set; }
        public int Version { get; set; }
        public bool IsSubList { get; set; } = false;
        public bool SerializeToFile { get; set; } = true;
        
        public string this[int index]
        {
            get => Get(index);
            set => Set(index, value);
        }
        
        public string Get(int index)
        {
            
            return _content[index];
        }

        public void Add(string value)
        {
            _content.Add(value);
        }
        public void Set(int index, string value)
        {
            _content[index] = value;
            
            if (SerializeToFile)
            {
                string strList = serialize();
                writeToFile(getFilename(Name), strList);
            }
        }


        public string GetFormattedConfig(bool incSensitive = false)
        {    
            return $@"[Type: {this.GetType()};
Name: {Name};
Version: {Version};
IsSubList: {IsSubList};
NrElements: {_content.Count};]
ContentHash: {_content.GetHashCode()}";
        }

        public override string ToString() => GetFormattedConfig(false);
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) _content).GetEnumerator();
        }

        public IEnumerator<string> GetEnumerator()
        {
            return _content.GetEnumerator();
        }

        public override bool Equals(object obj) => obj.ToString() == ToString();
        public override int GetHashCode() => GetFormattedConfig().GetHashCode();

        private string serialize()
        {
            
            MemoryStream memoryStream = new MemoryStream();
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(GetType());
            try
            {
                serializer.WriteObject(memoryStream, this);
            }
            catch (Exception ex)
            {
                return null;
            }

            byte[] json = memoryStream.ToArray();
            memoryStream.Close();
            
            return Encoding.UTF8.GetString(json, 0, json.Length);
        }

        private ContentList deserialize(string json)
        {
            MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(GetType());
            ContentList contentList = (ContentList) serializer.ReadObject(memoryStream);

            return contentList;

        }

    }
}