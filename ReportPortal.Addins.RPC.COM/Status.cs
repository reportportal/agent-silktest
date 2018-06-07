using System.Runtime.InteropServices;

namespace ReportPortal.Addins.RPC.COM
{
    [
        ComVisible(true),
        Guid("6AD11207-83EE-443B-9EB7-FF47D2511FBC"),
    ]
    public enum Status
    {
        None,
        InProgress,
        Passed,
        Failed,
        Skipped,
        Interrupted
    }
}