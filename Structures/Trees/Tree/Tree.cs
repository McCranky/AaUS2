using System;

namespace Structures.Trees.Tree
{
    public abstract class Tree<TKeyPart, TValue> where TKeyPart : IComparable<TKeyPart>
    {
        public abstract TValue Search(TreeKey<TKeyPart> key);
        public abstract void Remove(TreeKey<TKeyPart> key);
        public abstract void Add(TreeKey<TKeyPart> key, TValue value);
    }
}