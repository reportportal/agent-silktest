using System.Runtime.InteropServices;

namespace ReportPortal.Addins.RPC.COM
{
    [
        ComVisible(true),
        Guid("6AD11207-83EE-443B-9EB7-FF47D2511FBC"),
    ]
    public enum Status
    {
        None = EPAM.ReportPortal.Client.Models.Status.None,
        InProgress = EPAM.ReportPortal.Client.Models.Status.InProgress,
        Passed = EPAM.ReportPortal.Client.Models.Status.Passed,
        Failed = EPAM.ReportPortal.Client.Models.Status.Failed,
        Skipped = EPAM.ReportPortal.Client.Models.Status.Skipped,
        Interrupted = EPAM.ReportPortal.Client.Models.Status.Interrupted
    }
}