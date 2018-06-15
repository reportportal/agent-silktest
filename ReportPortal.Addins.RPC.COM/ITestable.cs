using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ReportPortal.Addins.RPC.COM
{
    [ComVisible(false)]
    public interface ITestable
    {
        string FullTestName { get; }
        IEnumerable<string> Hierarchy { get;  }
    }
}