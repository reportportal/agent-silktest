using System.Runtime.InteropServices;
using ReportPortal.Client.Models;

namespace ReportPortal.Addins.RPC.COM
{
    [
        ComVisible(true),
        Guid("EC53CE0E-C3B9-4EBA-893F-6CE587208B7E")
    ]
    public enum Mode
    {
        Default = LaunchMode.Default,
        Debug = LaunchMode.Debug
    }
}