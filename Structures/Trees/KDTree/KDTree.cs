using System;
using System.Collections.Generic;
using System.Linq;
using Structures.Trees.Tree;

namespace Structures.Trees.KDTree
{
    public enum Side
    {
        Right,
        Left
    }
    
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

        public override Guid Add(IEnumerable<TKey> keys, TValue value)
        {
            var keyList = keys.ToList();
            var newNode = new KDTNode<TKey, TValue>(keyList, value);
            if (Root == null)
            {
                newNode.Level = 0;
                Root = newNode;
                ++Count;
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
                        newNode.Parent = lastNode;
                        ++Count;
                        foundPlace = true;
                    }
                    else
                    {
                        lastNode = lastNode[result];
                    }
                }
            }
            return newNode.PrimaryKey;
        }
        
        public override void Remove(IEnumerable<TKey> keys, Guid id)
        {
            if (Root == null) return;
            if (!TryFindKdtNodes(keys, out var candidates))
            {
                Console.WriteLine("Node with given keys doesn't exist.");
                return;
            }

            var index = 0;
            // vyber konkretnej nody pre mazanie ak je poskytnute id
            if (candidates.Count > 1 && id != Guid.Empty)
            {
                for (int i = 0; i < candidates.Count; i++)
                {
                    if (candidates[i].PrimaryKey == id)
                    {
                        index = i;
                    }
                }
            }
            
            var nodeToReplace = candidates[index];

            while (nodeToReplace != null)
            {
                if (nodeToReplace.IsLeaf)
                {
                    if (nodeToReplace.HasParent)
                    {
                        if (nodeToReplace.IsLeftSon)
                            nodeToReplace.Parent.LeftChild = null;
                        else
                            nodeToReplace.Parent.RightChild = null;
                        nodeToReplace.Parent = null;
                    }
                    else
                    {
                        Root = null;
                    }

                    --Count;
                    nodeToReplace = null;
                }
                else
                {
                    List<KDTNode<TKey, TValue>> replacementCandidates;
                    if (nodeToReplace.LeftChild != null) // najdi in order predchodcu
                    {
                        replacementCandidates = FindMaximum(nodeToReplace, Side.Left);
                        // pokial je možne tak za kandidáta zvilíme list, inak prvého nájdeného
                        KDTNode<TKey, TValue> theChosenOne = null;
                        foreach (var cand in replacementCandidates.Where(cand => cand.IsLeaf))
                        {
                            theChosenOne = cand;
                            break;
                        }
                        theChosenOne ??= replacementCandidates[0];
                        AssignNodes(ref nodeToReplace,ref  theChosenOne);
                        nodeToReplace = theChosenOne;
                    }
                    else // najdi in order nasledovnika
                    {
                        replacementCandidates = FindMaximum(nodeToReplace, Side.Right);
                        var theChosenOne = replacementCandidates[0];
                        AssignNodes(ref nodeToReplace,ref  theChosenOne);
                        if (nodeToReplace.LeftChild == null)
                        {
                            nodeToReplace.LeftChild = nodeToReplace.RightChild;
                            nodeToReplace.RightChild = null;
                        }
                        nodeToReplace = theChosenOne;
                    }
                }
            }
        }

        public bool TryFindKdtNodes(IEnumerable<TKey> keys, out List<KDTNode<TKey, TValue>> values)
        {
            values = FindKdtNodes(keys);
            return values != null && values.Count > 0;
        }

        public bool TryFindKdtNode(IEnumerable<TKey> keys, Guid id, out KDTNode<TKey, TValue> node)
        {
            node = FindKdtNode(keys, id);
            return node != null;
        }

        public KDTNode<TKey, TValue> FindKdtNode(IEnumerable<TKey> keys, Guid id)
        {
            var pointFrom = keys as TKey[] ?? keys.ToArray();
            var nodes = FindInRange(pointFrom, pointFrom);
            return nodes.FirstOrDefault(node => node.PrimaryKey == id);
        }
        
        public List<KDTNode<TKey, TValue>> FindKdtNodes(IEnumerable<TKey> keys)
        {
            var pointFrom = keys as TKey[] ?? keys.ToArray();
            return FindInRange(pointFrom, pointFrom);
        }

        public List<KDTNode<TKey, TValue>> FindInRange(IEnumerable<TKey> pointFrom, IEnumerable<TKey> pointTo)
        {
            var from = pointFrom.ToList();
            var to = pointTo.ToList();
            if (Root == null || from.Count != to.Count && to.Count != KeyCount) return null;
            
            var result = new List<KDTNode<TKey, TValue>>();
            var toProcess = new Queue<KDTNode<TKey, TValue>>();
            toProcess.Enqueue(Root);
            
            while (toProcess.Count > 0)
            {
                var current = toProcess.Dequeue();
                if (IsBetween(from, to, current.Keys))
                {
                    result.Add(current);
                }
                
                var level = current.Level;
                // ak je spodná hranica <= akutálnej hodnote, tak ešte môžme ísť vľavo
                if (from[level].CompareTo(current.Keys[level]) <= 0 && current.LeftChild != null)
                {
                    toProcess.Enqueue(current.LeftChild);
                }
                // ak je horná hranica >= akutálnej hodnote, tak ešte môžme ísť vpravo
                if (to[level].CompareTo(current.Keys[level]) >= 0 && current.RightChild != null)
                {
                    toProcess.Enqueue(current.RightChild);
                }
            }
            
            return result;
        }

        private static List<KDTNode<TKey, TValue>> FindMaximum(KDTNode<TKey, TValue> startPoint, Side side)
        {
            var result = new List<KDTNode<TKey, TValue>>();
            var toProcess = new Stack<KDTNode<TKey, TValue>>();
            var inOrderInspection = new Queue<KDTNode<TKey, TValue>>();
            
            var levelOfInteress = startPoint.Level;
            var current = side == Side.Right ? startPoint.RightChild : startPoint.LeftChild;
            var findValue = current.Keys[levelOfInteress];
            
            // naplnenie in order prehliadky prvkov čo nas zaujimaju a ziskanie minimalnej hodnoty
            while (toProcess.Count > 0 || current != null)
            {
                while (current != null)
                {
                    toProcess.Push(current);

                    var res = findValue.CompareTo(current.Keys[levelOfInteress]);
                    findValue = res < 0 ? current.Keys[levelOfInteress] : findValue;
                    
                    // ak hladame predchodcu a sme na urpvni ktora nas zaujima a vlavo su už menšie prvky, tak vľavo nepokračujeme
                    if (current.Level == levelOfInteress &&
                        findValue.CompareTo(current.Keys[levelOfInteress]) > 0)
                    {
                        current = null;
                    }
                    else
                    {
                        current = current.LeftChild;
                    }
                }

                var popped = toProcess.Pop();
                inOrderInspection.Enqueue(popped);
                current = popped.RightChild;
            }
            // vyber prvkov s najmenšou hodnotou hladaneho kluča z prehliadky
            foreach (var node in inOrderInspection)
            {
                if (node.Keys[levelOfInteress].CompareTo(findValue) == 0)
                {
                    result.Add(node);
                }
            }
            return result;
        }

        private bool IsBetween(IReadOnlyList<TKey> min, IReadOnlyList<TKey> max, IReadOnlyList<TKey> value)
        {
            for (var i = 0; i < KeyCount; i++)
            {
                if (min[i].CompareTo(value[i]) > 0 || max[i].CompareTo(value[i]) < 0)
                    return false;
            }

            return true;
        }

        private void AssignNodes(ref KDTNode<TKey, TValue> nodeToBeAssigned,ref  KDTNode<TKey, TValue> toThisNode)
        {
            nodeToBeAssigned.PrimaryKey = toThisNode.PrimaryKey;
            nodeToBeAssigned.Keys = toThisNode.Keys;
            nodeToBeAssigned.Data = toThisNode.Data;
        }
        public new IEnumerator<KDTNode<TKey, TValue>> GetEnumerator()
        {
            return new KDTEnumerator<TKey, TValue>(Root, Inspection.LevelOrder);
        }
    }
}