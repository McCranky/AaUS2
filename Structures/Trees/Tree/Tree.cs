using System;
using System.Collections;
using System.Collections.Generic;
using Structures.Common;

namespace Structures.Trees.Tree
{
    public abstract class Tree<TKey, TValue> where TKey : IList<TKey>, IComparable<TKey>
    {
        public abstract TValue Search(TKey key);
        public abstract void Remove(TKey key);
        public abstract void Add(IEnumerable<TKey> keys, TValue value);
    }
}