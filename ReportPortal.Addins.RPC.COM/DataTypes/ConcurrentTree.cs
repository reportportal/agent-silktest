using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using ReportPortal.Client.Models;

namespace ReportPortal.Addins.RPC.COM.DataTypes
{
    [ComVisible(false)]
    public class ConcurrentTree<TValue>
        where TValue : class
    {
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        private readonly IEditableNode<TValue> _superRoot = new ConcurrentTreeNode<TValue>();

        public IEnumerable<IReadonlyNode<TValue>> RunningTests
        {
            get
            {
                _lock.EnterReadLock();
                try
                {
                    Stack<IReadonlyNode<TValue>> stack = new Stack<IReadonlyNode<TValue>>();
                    stack.Push(_superRoot);

                    do
                    {
                        var parent = stack.Pop();
                        foreach (var current in parent.Children)
                        {
                            if (current.Children.Count == 0)
                            {
                                yield return current;
                            }
                            else
                            {
                                stack.Push(current);
                            }
                        }
                    } while (stack.Count != 0);
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
        }

        public void AddPath(string pathName, Func<IReadonlyNode<TValue>, string, TestItemType, TValue> createValue)
        {
            _lock.EnterWriteLock();
            try
            {
                var names = pathName.Split(Constants.PathSeparator);
                IEditableNode<TValue> node = _superRoot;
                for (int i = 0; i < names.Length; i++)
                {
                    TestItemType type = TestItemType.Suite;
                    if (i == names.Length - 2)
                        type = TestItemType.Test;
                    else if (i == names.Length - 1)
                        type = TestItemType.Step;

                    node = (IEditableNode<TValue>) TryToAddChild(node, names[i], type, createValue);
                }
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public void Clear(Action<IReadonlyNode<TValue>> postAction)
        {
            _lock.EnterWriteLock();
            try
            {
                _superRoot.DeleteChildren(postAction);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public void DeleteNodeWithChildren(IReadonlyNode<TValue> node, Action<IReadonlyNode<TValue>> postAction)
        {
            _lock.EnterWriteLock();
            try
            {
                var editable = (IEditableNode<TValue>) node;
                editable.DeleteChildren(postAction);
                editable.Parent.DeleteChild(editable, postAction);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public IReadonlyNode<TValue> FindNode(string pathName)
        {
            _lock.EnterReadLock();
            try
            {
                var names = pathName.Split(Constants.PathSeparator);
                var parent = _superRoot;

                foreach (string name in names)
                {
                    var child = parent.FindChild(name);
                    if (child == null)
                    {
                        return null;
                    }
                    parent = (IEditableNode<TValue>) child;
                }

                return parent;
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        private IReadonlyNode<TValue> TryToAddChild(IReadonlyNode<TValue> parent, string name, TestItemType type,
            Func<IReadonlyNode<TValue>, string, TestItemType, TValue> createValue)
        {
            var node = (IEditableNode<TValue>)(parent ?? _superRoot);
            return node.FindChild(name) ?? node.AddChild(name, (nod, nam) => createValue(nod, nam, type));
        }
    }
}