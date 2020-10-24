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

        public bool TryFindKDTNodes(IEnumerable<TKey> keys, out List<KDTNode<TKey, TValue>> values)
        {
            values = new List<KDTNode<TKey, TValue>>();
            var lastNode = Root;
            var founded = false;
            
            if (lastNode == null)
            {
                return false;
            }

            var keyList = keys.ToList();
            // var firstMatch = false;
            
            while (!founded)
            {
                if (lastNode.Keys.SequenceEqual(keyList))
                {
                    values.Add(lastNode);
                    founded = true;
                    var matches = new List<KDTNode<TKey, TValue>>();
                    matches.Add(lastNode);
                    // var lastLevel = lastNode.Level;

                    if (lastNode.LeftChild == null ||
                        keyList[lastNode.Level].CompareTo(lastNode.LeftChild.Keys[lastNode.Level]) != 0)
                    {
                        break; // prišli sme po koreň alebo ľavý syn má už inú hodnotu kľúča
                    }
                    
                    // lastNode = lastNode.LeftChild; // v tomto podstrome ešte môžme nájsť rovnaké hodnotyy klúčov
                    do
                    {
                        var level = lastNode.Level;
                        lastNode = lastNode[
                            keyList[level].CompareTo(lastNode.Keys[level])
                        ];
                        
                        if (lastNode.Keys.SequenceEqual(keyList))
                        {
                            values.Add(lastNode);
                        }
                    } while (!lastNode.IsLeaf);
                }
                else
                {
                    // Console.WriteLine(lastNode.Data);
                    var level = lastNode.Level;
                    lastNode = lastNode[
                            keyList[level].CompareTo(lastNode.Keys[level])
                    ];
                    if (lastNode == null)
                    {
                        break;
                    }
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