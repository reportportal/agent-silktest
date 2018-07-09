using System.Runtime.InteropServices;

namespace ReportPortal.Addins.RPC.COM
{
    [ComVisible(false)]
    public interface IConfiguration
    {
        string ServerUrl { get; }
        string ServerProjectName { get; }
        string ServerPassword { get; }
        bool ProxyAvailable { get; }
        string ProxyDomain { get; }
        string ProxyServer { get; }
        string ProxyUser { get; }
        string ProxyPassword { get; }

    }
}