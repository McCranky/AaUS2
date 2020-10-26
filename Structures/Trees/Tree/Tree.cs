using System;
using System.Collections;
using System.Collections.Generic;
using Structures.Common;

namespace Structures.Trees.Tree
{
    public abstract class Tree<TKey, TValue> : IEnumerable where TKey : IComparable
    {
        protected Queue<TreeNode<TKey, TValue>> Path { get; set; }
        public abstract void Remove(IEnumerable<TKey> keys);
        public abstract void Add(IEnumerable<TKey> keys, TValue value);
        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}