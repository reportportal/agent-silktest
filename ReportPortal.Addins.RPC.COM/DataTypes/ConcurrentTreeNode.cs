using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ReportPortal.Addins.RPC.COM.DataTypes
{
    [ComVisible(false)]
    public class ConcurrentTreeNode<TValue> : IEditableNode<TValue>
        where TValue : class
    {
        private readonly List<IEditableNode<TValue>> _children = new List<IEditableNode<TValue>>();

        public ConcurrentTreeNode() : this(null, string.Empty, string.Empty, null)
        {
        }

        private ConcurrentTreeNode(IEditableNode<TValue> parent, string fullName, string name, TValue reporter)
        {
            Parent = parent;
            FullName = fullName;
            Name = name;
            Value = reporter;
        }
        public IEditableNode<TValue> Parent { get; }

        public string FullName { get; }
        public string Name { get; }

        public TValue Value { get; }

        public IReadOnlyList<IReadonlyNode<TValue>> Children => _children;



        public IEditableNode<TValue> AddChild(string childName, Func<IReadonlyNode<TValue>, string, TValue> createValue)
        {
            if (IsChildAlreadyExist(childName))
                throw new Exception($"Node '{childName}' is already exist.");

            string prefix = string.IsNullOrEmpty(FullName) ? string.Empty : FullName + Constants.PathSeparator;
            var root = new ConcurrentTreeNode<TValue>(this, prefix + childName, childName, createValue(this, childName));
            _children.Add(root);
            return root;
        }

        public void DeleteChildren(Action<IReadonlyNode<TValue>> postAction)
        {
            foreach (var child in _children)
            {
                child.DeleteChildren(postAction);

                postAction(child);
            }
            _children.Clear();
        }

        public void DeleteChild(IReadonlyNode<TValue> child, Action<IReadonlyNode<TValue>> postAction)
        {
            bool result = _children.Remove((IEditableNode<TValue>)child);
            if (!result)
            {
                throw new Exception($"Node {child.FullName} is not found in the parent suite {FullName}");
            }
            postAction(child);
        }

        public IReadonlyNode<TValue> FindChild(string name)
        {
            return _children.Find(x => string.Compare(x.Name, name, StringComparison.CurrentCultureIgnoreCase) == 0);
        }

        private bool IsChildAlreadyExist(string name)
        {
            return _children.FindIndex(
                       x => string.Compare(x.Name, name, StringComparison.CurrentCultureIgnoreCase) == 0) >= 0;
        }
    }
}