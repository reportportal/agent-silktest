using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ReportPortal.Addins.RPC.COM.DataTypes
{
    [ComVisible(false)]
    public interface IReadonlyNode<out TValue>
    {
        string FullName { get;  }
        string Name { get; }

        TValue Value { get; }

        IReadOnlyList<IReadonlyNode<TValue>> Children { get; }

        IReadonlyNode<TValue> FindChild(string name);
    }
}