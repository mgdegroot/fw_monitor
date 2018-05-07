using System.Threading.Tasks;
using fw_monitor.DataObjects;

namespace fw_monitor
{
    public interface IManager
    {
        ListConfig ListConfig { get; set; }
        HostConfig HostConfig { get; set; }
        IExecutor Executor { get; set; }
        Task ManageLists(ListConfig listConfig, HostConfig hostConfig);
    }
}