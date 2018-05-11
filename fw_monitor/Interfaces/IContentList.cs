using System.Collections.Generic;

namespace fw_monitor
{
    public interface IContentList : IRepositoryItem
    {
        string Name { get; set; }
        string Version { get; set; }
        bool IsSubList { get; set; }
        IList<string> Elements { get;}
        string Serialize();
        IContentList Deserialize(string json);


//        string this[string key] { get; set; }
//
//        string Get(string key);
//        void Set(string key, string value);

    }
}