using System;
using System.Runtime.InteropServices;

namespace ReportPortal.Addins.RPC.COM
{
    [ComVisible(false)]
    [Serializable]
    public class GeneralConfiguration
    {
        public bool DebugMode { get; set; }
        public ProxyConfiguration ProxyConfiguration { get; set; }
    }
}