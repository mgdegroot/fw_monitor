using System.Collections.Generic;
using System.Threading.Tasks;
using fw_monitor.DataObjects;

namespace fw_monitor
{
    public interface IListFetcher : IOutputProvider
    {
        ListConfig ListConfig { get; set; }
        
        IList<string> this[string index] { get; set; }
        IList<string> Get(string name);
        
        Dictionary<string, List<string>> Lists { get; set; }
        void Set(string name, List<string> item);
        Task FetchAndParse();

    }
}