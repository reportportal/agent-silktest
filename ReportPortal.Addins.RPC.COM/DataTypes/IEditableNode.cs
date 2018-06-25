using System;
using System.Runtime.InteropServices;

namespace ReportPortal.Addins.RPC.COM.DataTypes
{
    [ComVisible(false)]
    public interface IEditableNode<TValue> : IReadonlyNode<TValue>
    {
        IEditableNode<TValue> Parent { get; }

        IEditableNode<TValue> AddChild(string name, Func<IReadonlyNode<TValue>, string, TValue> createValue);
        void DeleteChildren(Action<IReadonlyNode<TValue>> postAction);
        void DeleteChild(IReadonlyNode<TValue> child, Action<IReadonlyNode<TValue>> postAction);
    }
}