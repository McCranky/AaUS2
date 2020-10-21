using System;
using System.Collections.Generic;
using System.Linq;
using Structures.Common;

namespace Structures.Trees.Tree
{
    public interface IKdComparable<TKeyPart> where TKeyPart : IComparable<TKeyPart>
    {
        int CompareTo(TreeKey<TKeyPart> other, int level);
    }

    public class TreeKey<TKeyPart> : IKdComparable<TKeyPart> where TKeyPart : IComparable<TKeyPart>
    {
        public TreeKey(params IComparable<TKeyPart>[] keyParts)
        {
            KeyParts = keyParts.ToList() ?? throw new ArgumentNullException(nameof(keyParts));
        }

        public IReadOnlyList<IComparable<TKeyPart>> KeyParts { get; private set; }

        public int CompareTo(TreeKey<TKeyPart> other, int level)
        {
            if (KeyParts.Count != other.KeyParts.Count)
                throw new ArgumentException("Other must have the same length", nameof(other));

            return KeyParts[level].CompareTo((TKeyPart)other.KeyParts[level]);
        }
    }

    public abstract class TreeNode<TKeyPart, TValue> : ValueItem<TValue> where TKeyPart : IComparable<TKeyPart>
    {
        public TreeKey<TKeyPart> Key { get; private set; }
        public TreeNode<TKeyPart, TValue> Parent { get; private set; }
        public TreeNode<TKeyPart, TValue> LeftChild { get; private set; }
        public TreeNode<TKeyPart, TValue> RightChild { get; private set; }

        public TreeNode(TreeKey<TKeyPart> key, TValue value) : base(value)
        {
            Parent = null;
            Key = key;
        }

        public TreeNode(TreeNode<TKeyPart, TValue> other) : base(other.Parent.Data)
        {
            Parent = other.Parent;
            Key = other.Key;
        }

        public bool IsRoot => Parent == null;
        public void ResetParent()
        {
            Parent = null;
        }

        public void SetParent(TreeNode<TKeyPart, TValue> parent)
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