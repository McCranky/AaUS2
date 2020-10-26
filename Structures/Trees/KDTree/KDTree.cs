﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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

        public override void Add(IEnumerable<TKey> keys, TValue value)
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

        }

        public bool TryFindKdtNodes(IEnumerable<TKey> keys, out List<KDTNode<TKey, TValue>> values)
        {
            values = new List<KDTNode<TKey, TValue>>();
            var lastNode = Root;
            var founded = false;
            
            if (lastNode == null)
            {
                return false;
            }

            var keyList = keys.ToList();
            
            while (!founded)
            {
                if (lastNode.Keys.SequenceEqual(keyList))
                {
                    values.Add(lastNode);
                    founded = true;
                    var matches = new List<KDTNode<TKey, TValue>>();
                    matches.Add(lastNode);

                    if (lastNode.LeftChild == null ||
                        keyList[lastNode.Level].CompareTo(lastNode.LeftChild.Keys[lastNode.Level]) != 0)
                    {
                        break; // prišli sme po list alebo ľavý syn má už inú hodnotu kľúča
                    }
                    
                    do
                    {
                        var level = lastNode.Level;
                        var result = keyList[level].CompareTo(lastNode.Keys[level]);
                        lastNode = lastNode[result];
                        if (lastNode == null)
                        {
                            break;
                        }
                        //TODO tu to pada || kvoli rovnakym hodnotam vpravo???
                        if (lastNode.Keys.SequenceEqual(keyList))
                        {
                            values.Add(lastNode);
                        }
                    } while (!lastNode.IsLeaf);
                }
                else
                {
                    var level = lastNode.Level;
                    var result = keyList[level].CompareTo(lastNode.Keys[level]);
                    lastNode = lastNode[result];
                    if (lastNode == null)
                    {
                        break;
                    }
                }
            }
            
            return founded;
        }

        private List<KDTNode<TKey, TValue>> FindMaximum(KDTNode<TKey, TValue> startPoint, Side side)
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
        
        public override void Remove(IEnumerable<TKey> keys)
        {
            if (Root == null) return;
            if (!TryFindKdtNodes(keys, out var candidates))
            {
                Console.WriteLine("Node with given keys doesn't exist.");
                return;
            }

            var index = 0;
            // vyber konkretnej nody pre mazanie
            if (candidates.Count > 1)
            {
                // TODO for user decision purposes
                // Console.WriteLine("Multiple matches found. Choose one to delete:");
                // for (int i = 0; i < candidates.Count; i++)
                // {
                //     Console.WriteLine($"Option[{i}] ID:{candidates[i].PrimaryKey} Data: {candidates[i].Data.ToString()}");
                // }
                // Console.Write($"Delete: ");
                // index = int.Parse(Console.ReadLine() ?? "0");
            }
            
            var nodeToReplace = candidates[index];
            // var toReadd = new Stack<KDTNode<TKey, TValue>>();
            // var toDelete = new Stack<KDTNode<TKey, TValue>>();
            
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
                        // TODO LIFO ich odstraniť a znovu pridať
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

            // foreach (var node in toReadd)
            // {
            //     Add(node.Keys, node.Data);
            // }
            // Console.WriteLine($"Nodes successfully replaced");
        }

        private void AssignNodes(ref KDTNode<TKey, TValue> nodeToBeAssigned,ref  KDTNode<TKey, TValue> toThisNode)
        {
            nodeToBeAssigned.PrimaryKey = toThisNode.PrimaryKey;
            nodeToBeAssigned.Keys = toThisNode.Keys;
            nodeToBeAssigned.Data = toThisNode.Data;
            // nodeToBeAssigned.Level = toThisNode.Level;
        }
        public IEnumerator<KDTNode<TKey, TValue>> GetEnumerator()
        {
            return new KDTEnumerator<TKey, TValue>(Root, Inspection.LevelOrder);
        }
    }
}