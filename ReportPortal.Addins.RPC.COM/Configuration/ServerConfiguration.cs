using System;
using System.Runtime.InteropServices;

namespace ReportPortal.Addins.RPC.COM
{
    [ComVisible(false)]
    [Serializable]
    public class ServerConfiguration
    {
        public string Url { get; set; }
        public string Project { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}