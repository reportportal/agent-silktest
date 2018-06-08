using System.Runtime.InteropServices;

namespace ReportPortal.Addins.RPC.COM
{
    [
        ComVisible(true),
        Guid("AB67C52B-980A-41DA-B82E-A4E943C7788F")
    ]
    public enum LogLevel
    {
        Trace = Client.Models.LogLevel.Trace, //
        Debug = Client.Models.LogLevel.Debug, //
        Info = Client.Models.LogLevel.Info, //
        Warning = Client.Models.LogLevel.Warning, //
        Error = Client.Models.LogLevel.Error //
    }
}