using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace fw_monitor.DataObjects
{
    [DataContract]
    public class ContentList : IContentList, IEnumerable<string>
    {
        private string filenamePrefix = string.Empty;
        
        [DataMember]
        private List<string> _content = new List<string>();

        public ContentList()
        {
            filenamePrefix = "list";
        }

        public IList<string> Elements
        {
            get => _content;
            set => _content = value as List<string>;
        }
        
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Version { get; set; }
        [DataMember]
        public bool IsSubList { get; set; } = false;
        
        public string this[int index]
        {
            get => Get(index);
            set => Set(index, value);
        }
        
        public string Get(int index)
        {
            return Elements[index];
        }

        public void Add(string value)
        {
            // Trim but don't add if empty -->
            value = value.Trim();
            if (string.IsNullOrEmpty(value)) return;
            
            Elements.Add(value);
        }
        public void Set(int index, string value)
        {
            Elements[index] = value;
        }

//        public void SaveToFile()
//        {
//            string strList = Serialize();
//            util.WriteToFile(getFilename(Name), strList, false);
//        }


        public string GetFormattedConfig(bool incSensitive = false)
        {    
            return $@"[Type: {this.GetType()};
Name: {Name};
Version: {Version};
IsSubList: {IsSubList};
NrElements: {Elements.Count};]
ContentHash: {Elements.GetHashCode()}";
        }

        public override string ToString() => GetFormattedConfig(false);
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) Elements).GetEnumerator();
        }

        public IEnumerator<string> GetEnumerator()
        {
            return Elements.GetEnumerator();
        }

        public override bool Equals(object obj) => obj.ToString() == ToString();
        public override int GetHashCode() => GetFormattedConfig().GetHashCode();
        
        protected string getFilename(string name) =>
            Path.Combine(Util.SerializePath, $"{filenamePrefix}_{name}{Util.Extension.JSON}");

        public string Serialize()
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

        public IContentList Deserialize(string json)
        {
            MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(GetType());
            ContentList contentList = (ContentList) serializer.ReadObject(memoryStream);

            return contentList;
        }

    }
}