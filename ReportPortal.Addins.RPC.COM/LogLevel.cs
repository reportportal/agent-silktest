using System.Runtime.InteropServices;

namespace ReportPortal.Addins.RPC.COM
{
    [
        ComVisible(true),
        Guid("AB67C52B-980A-41DA-B82E-A4E943C7788F")
    ]
    public enum LogLevel
    {
        None = EPAM.ReportPortal.Client.Models.LogLevel.None,
        Trace = EPAM.ReportPortal.Client.Models.LogLevel.Trace, //
        Debug = EPAM.ReportPortal.Client.Models.LogLevel.Debug, //
        Info = EPAM.ReportPortal.Client.Models.LogLevel.Info, //
        Warning = EPAM.ReportPortal.Client.Models.LogLevel.Warning, //
        Error = EPAM.ReportPortal.Client.Models.LogLevel.Error //
    }
}