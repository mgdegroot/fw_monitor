using System.Collections.Generic;
using System.Threading.Tasks;
using fw_monitor.DataObjects;

namespace fw_monitor
{
    public interface IListFetcher
    {
        Dictionary<string, List<string>> Lists { get; set; }
        ListConfig ListConfig { get; set; }
        IFeedbackProvider Feedback { get; set; }
        
        IList<string> this[string index] { get; set; }
        IList<string> Get(string name);
        Task<string> GetNewestVersion();
        
        void Set(string name, List<string> item);
        Task FetchAndParse();

    }
}