using System;
using System.Collections.Generic;
using Structures.Common;
using Structures.Trees.Tree;

namespace Structures.Trees.KDTree
{
    public class KDTNode<TKey, TValue> : ValueItem<TValue> where TKey : IComparable
    {
        public Guid PrimaryKey { get; } = Guid.NewGuid();
        public int Level { get; set; }
        public List<TKey> Keys { get; private set; }
        public KDTNode<TKey, TValue> Parent { get; private set; }
        public KDTNode<TKey, TValue> LeftChild { get; private set; }
        public KDTNode<TKey, TValue> RightChild { get; private set; }

        public KDTNode(List<TKey> keys, TValue value) : base(value)
        {
            Parent = null;
            Keys = keys;
        }

        public KDTNode(KDTNode<TKey, TValue> other) : base(other)
        {
            Parent = other.Parent;
            Keys = other.Keys;
        }

        public KDTNode<TKey, TValue> this[int childIndex]
        {
            get
            {
                return childIndex <= 0 ? LeftChild : RightChild;
            }

            set
            {
                if (childIndex <= 0)
                {
                    LeftChild = value;
                }
                else
                {
                    RightChild = value;
                }
            }
        }

        public bool IsLeaf => LeftChild == null && RightChild == null;
    }
}