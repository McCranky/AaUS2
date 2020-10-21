using System;
using Structures.Trees.Tree;

namespace Structures.Trees.KDTree
{
    public class KDTNode<TKeyPart, TValue> : TreeNode<TKeyPart, TValue> where TKeyPart : IComparable<TKeyPart>
    {
        public int Level { get; set; }
        public KDTNode(TreeKey<TKeyPart> key, TValue value) : base(key, value) { }
        public KDTNode(TreeNode<TKeyPart, TValue> other) : base(other) { }
    }
}