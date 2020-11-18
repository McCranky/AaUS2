using System;
using System.Collections;
using System.Collections.Generic;

namespace Structures.Trees.Tree
{
    public abstract class Tree<TKey, TValue> : IEnumerable where TKey : IComparable
    {
        public abstract void Remove(IEnumerable<TKey> keys, Guid id);
        public abstract Guid Add(IEnumerable<TKey> keys, TValue value);
        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}