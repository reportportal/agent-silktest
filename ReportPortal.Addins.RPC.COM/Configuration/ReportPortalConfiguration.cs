using System;
using System.Runtime.InteropServices;

namespace ReportPortal.Addins.RPC.COM
{
    [ComVisible(false)]
    [Serializable]
    public class ReportPortalConfiguration
    {
        public GeneralConfiguration GeneralConfiguration { get; set; }
        public LaunchConfiguration LaunchConfiguration { get; set; }
        public ServerConfiguration ServerConfiguration { get; set; }
    }
}