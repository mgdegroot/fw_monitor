using System.Reflection.Metadata.Ecma335;

namespace fw_monitor
{
    public interface IContentList
    {
        string Name { get; set; }
        int Version { get; set; }
        bool IsSubList { get; set; }
        
//        string this[string key] { get; set; }
//
//        string Get(string key);
//        void Set(string key, string value);

    }
}