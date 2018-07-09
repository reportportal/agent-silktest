using System;
using System.Runtime.InteropServices;

namespace ReportPortal.Addins.RPC.COM
{
    [ComVisible(false)]
    [Serializable]
    public class ReportPortalConfiguration
    {
        public ServerConfiguration ServerConfiguration { get; set; }
        public ProxyConfiguration ProxyConfiguration { get; set; }
    }
}