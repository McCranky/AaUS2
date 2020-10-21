using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Structures.Trees.Tree;

namespace Structures.Trees.KDTree
{
    public class KDTree<TKey, TValue> : Tree<TKey, TValue> where TKey :  IList<TKey>, IComparable<TKey>
    {
        public KDTNode<TKey, TValue> Root { get; private set; }
        public int Count { get; private set; }

        public KDTree()
        {
            Root = null;
            Count = 0;
        }
        
        public override TValue Search(TKey key)
        {
            throw new NotImplementedException();
        }

        public override void Remove(TKey key)
        {
            throw new NotImplementedException();
        }

        public override void Add(IEnumerable<TKey> keys, TValue value)
        {
            var keyList = keys.ToList();
            var newNode = new KDTNode<TKey, TValue>(keyList, value);
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
                    var result = keyList[level][level].CompareTo(lastNode.Keys[level]);
                    var child = result <= 0 ? lastNode.LeftChild : lastNode.RightChild;
                    if (child == null)
                    {
                        newNode.Level = level;
                        child = newNode;
                        foundPlace = true;
                    }
                    else
                    {
                        lastNode = child as KDTNode<TKey, TValue>;
                    }
                }
            }

        }

        private bool TryFindBSTNode(KDTNode<TKey, TValue> node, out KDTNode<TKey, TValue> nearestNode)
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