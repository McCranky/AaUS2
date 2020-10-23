using System;
using System.Collections.Generic;
using Structures.Common;

namespace Structures.Trees.Tree
{
    public abstract class TreeNode<TKey, TValue>  : ValueItem<TValue> where TKey : IComparable
    {
        public List<TKey> Keys { get; private set; }
        public TreeNode<TKey, TValue> Parent { get; private set; }
        public TreeNode<TKey, TValue> LeftChild { get; private set; }
        public TreeNode<TKey, TValue> RightChild { get; private set; }

        public TreeNode(List<TKey> keys, TValue value) : base(value)
        {
            Parent = null;
            Keys = keys;
        }

        public TreeNode(TreeNode<TKey, TValue> other) : base(other.Parent.Data)
        {
            Parent = other.Parent;
            Keys = other.Keys;
        }

        public bool IsRoot => Parent == null;
        public void ResetParent()
        {
            Parent = null;
        }

        public void SetParent(TreeNode<TKey, TValue> parent)
        {
            Parent = parent;
        }

        // public abstract TreeNode<T> ShallowCopy();
        // public abstract TreeNode<T> DeepCopy();
        // public abstract bool IsLeaf();
        // public abstract TreeNode<T> GetBrother(int brotherOrder);
        // public abstract TreeNode<T> GetSon(int sonOrder);
        // public abstract void InsertSon(TreeNode<T> son, int order) { }
        // public abstract TreeNode<T> ReplaceSon(TreeNode<T> son, int order);
        // public abstract TreeNode<T> RemoveSon(int order);
        // public abstract int Degree();
        // public abstract int SizeOfSubtree();
    }
}