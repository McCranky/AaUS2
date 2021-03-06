﻿using System;
using System.Collections.Generic;
using Structures.Common;

namespace Structures.Trees.KDTree
{
    public class KDTNode<TKey, TValue> : ValueItem<TValue> where TKey : IComparable
    {
        public List<TKey> Keys { get; set; }
        public KDTNode<TKey, TValue> Parent { get; set; }
        public KDTNode<TKey, TValue> LeftChild { get; set; }
        public KDTNode<TKey, TValue> RightChild { get; set; }
        public Guid PrimaryKey { get; set; } = Guid.NewGuid();
        public int Level { get; set; }

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
            get => childIndex <= 0 ? LeftChild : RightChild;

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
        public bool HasParent => Parent != null;
        public bool IsLeaf => LeftChild == null && RightChild == null;
        public bool IsLeftSon => Parent != null && Parent.LeftChild == this;
        public bool IsRightSon => Parent != null && Parent.RightChild == this;
    }
}