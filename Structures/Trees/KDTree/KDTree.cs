using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Structures.Trees.Tree;

namespace Structures.Trees.KDTree
{
    public class KDTree<TKey, TValue> : Tree<TKey, TValue>, IEnumerable<KDTNode<TKey, TValue>> where TKey : IComparable
    {
        public KDTNode<TKey, TValue> Root { get; private set; }
        public int Count { get; private set; }
        public int KeyCount { get; private set; }

        public KDTree(int keyCount)
        {
            Root = null;
            Count = 0;
            KeyCount = keyCount;
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
                    var result = keyList[level].CompareTo(lastNode.Keys[level]);

                    if (lastNode[result] == null)
                    {
                        newNode.Level = (level + 1) % KeyCount;
                        lastNode[result] = newNode;
                        ++Count;
                        foundPlace = true;
                    }
                    else
                    {
                        lastNode = lastNode[result];
                    }
                }
            }

        }

        private bool TryFindBSTNode(IEnumerable<TKey> keys, out int count)
        {
            count = 0;
            var lastNode = Root;
            var founded = false;
            
            if (lastNode == null)
            {
                return false;
            }

            var keyList = keys.ToList();
            // var firstMatch = false;
            
            while (lastNode != null)
            {
                if (lastNode.Keys == keyList)
                {
                    // firstMatch = true;
                    var matches = new List<KDTNode<TKey, TValue>>();
                    matches.Add(lastNode);
                    var lastLevel = lastNode.Level;

                    lastNode = lastNode.LeftChild; // lebo tam ešte môže byť noda s rovnakými kľúčmi
                    if (lastNode.Keys[lastLevel].CompareTo(keyList[lastLevel]) == 0)
                    {
                        
                    }
                }
                else
                {
                    var level = lastNode.Level;
                    lastNode = lastNode[lastNode.Keys[level].CompareTo(keyList[level])];
                }
            }
            
            return founded;
        }

        public IEnumerator<KDTNode<TKey, TValue>> GetEnumerator()
        {
            return new KDTEnumerator<TKey, TValue>(Root);
        }
    }
}