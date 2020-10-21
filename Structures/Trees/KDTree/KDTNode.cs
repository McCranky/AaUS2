using System;
using System.Collections.Generic;
using Structures.Trees.Tree;

namespace Structures.Trees.KDTree
{
    public class KDTNode<TKey, TValue> : TreeNode<TKey, TValue> where TKey : IList<IComparable<TKey>>
    {
        public int Level { get; set; }
        public KDTNode(List<TKey> keys, TValue value) : base(keys, value) { }
        public KDTNode(TreeNode<TKey, TValue> other) : base(other) { }
    }
}