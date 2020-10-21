using System;
using Structures.Trees.Tree;

namespace Structures.Trees.KDTree
{
    public class KDTree<TKeyPart, TValue> : Tree<TKeyPart, TValue> where TKeyPart : IComparable<TKeyPart>
    {
        public KDTNode<TKeyPart, TValue> Root { get; private set; }
        public int Count { get; private set; }

        public KDTree()
        {
            Root = null;
            Count = 0;
        }

        public override TValue Search(TreeKey<TKeyPart> key)
        {
            throw new NotImplementedException();
        }

        public override void Remove(TreeKey<TKeyPart> key)
        {
            throw new NotImplementedException();
        }

        public override void Add(TreeKey<TKeyPart> key, TValue value)
        {
            var newNode = new KDTNode<TKeyPart, TValue>(key, value);
            if (Root == null)
            {
                newNode.Level = 0;
                Root = newNode;
            }
            else
            {
                var lastNode = Root;
                var foundPlace = false;
                while (!foundPlace)
                {
                    var level = lastNode.Level;
                    var result = key.CompareTo(lastNode.Key, level);
                    var child = result <= 0 ? lastNode.LeftChild : lastNode.RightChild;
                    if (child == null)
                    {
                        newNode.Level = level;
                        child = newNode;
                        foundPlace = true;
                    }
                    else
                    {
                        lastNode = child as KDTNode<TKeyPart, TValue>;
                    }
                }
            }

        }

        private bool TryFindBSTNode(KDTNode<TKeyPart, TValue> node, out KDTNode<TKeyPart, TValue> nearestNode)
        {
            nearestNode = null;
            if (Root == null)
            {
                return false;
            }

            nearestNode = Root;

            return false;
        }
    }
}