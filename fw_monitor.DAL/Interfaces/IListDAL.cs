using System.Collections.Generic;

namespace fw_monitor.DAL.Interfaces
{
    public interface IListDAL
    {
        bool AddElement(string value, string type);
        bool AddAllElements(IEnumerable<string> values, string type);
        string GetElements();

    }
}