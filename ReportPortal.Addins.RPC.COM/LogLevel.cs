using System.Runtime.InteropServices;

namespace ReportPortal.Addins.RPC.COM
{
    [
        ComVisible(true),
        Guid("AB67C52B-980A-41DA-B82E-A4E943C7788F")
    ]
    public enum LogLevel
    {
        None = 0,
        Trace = 1, //
        Debug = 2, //
        Info = 3, //
        Warning = 4, //
        Error = 5 //
    }
}