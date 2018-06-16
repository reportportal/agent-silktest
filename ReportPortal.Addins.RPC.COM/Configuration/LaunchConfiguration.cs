using System;
using System.Runtime.InteropServices;

namespace ReportPortal.Addins.RPC.COM
{
    [ComVisible(false)]
    [Serializable]
    public class LaunchConfiguration
    {
        public string LaunchName { get; set; }
        public bool DebugMode { get; set; }
        public string Tags { get; set; }
    }
}