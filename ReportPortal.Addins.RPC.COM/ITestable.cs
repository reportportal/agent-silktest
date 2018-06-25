using System.Collections.Generic;
using System.Runtime.InteropServices;
using ReportPortal.Addins.RPC.COM.DataTypes;
using ReportPortal.Shared;

namespace ReportPortal.Addins.RPC.COM
{
    [ComVisible(false)]
    public interface ITestable
    {
        IEnumerable<IReadonlyNode<TestReporter>> RunningTests { get; }
    }
}