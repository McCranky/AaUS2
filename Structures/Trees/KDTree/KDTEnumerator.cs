using System;
using System.Collections;
using System.Collections.Generic;

namespace Structures.Trees.KDTree
{
    public enum Inspection
    {
        InOrder,
        LevelOrder
    }
    public class KDTEnumerator<TKey, TValue> : IEnumerator<KDTNode<TKey, TValue>> where TKey : IComparable
    {
        private Queue<KDTNode<TKey, TValue>> _path;
        private KDTNode<TKey, TValue> _root;
        private KDTNode<TKey, TValue> _current;
        private readonly Inspection _type;
        
        public KDTEnumerator(KDTNode<TKey, TValue> root, Inspection type)
        {
            _root = root;
            _path = new Queue<KDTNode<TKey, TValue>>();
            _type = type;
            switch (_type)
            {
                case Inspection.InOrder:
                    InOrderInspection();
                    break;
                case Inspection.LevelOrder:
                    LevelOrderInspection();
                    break;
                default:
                    InOrderInspection();
                    break;
            }
        }

        public bool MoveNext()
        {
            var hasNext = _path.Count > 0;
            if (hasNext)
            {
                _current = _path.Dequeue();
            }

            return hasNext;
        }

        public void Reset()
        {
            switch (_type)
            {
                case Inspection.InOrder:
                    InOrderInspection();
                    break;
                case Inspection.LevelOrder:
                    LevelOrderInspection();
                    break;
                default:
                    InOrderInspection();
                    break;
            }
        }

        public KDTNode<TKey, TValue> Current
        {
            get
            {
                return _current;
            }
        }

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            _path.Clear();
        }

        private void InOrderInspection()
        {
            var toProcess = new Stack<KDTNode<TKey, TValue>>();
            var current = _root;

            while (toProcess.Count > 0 || current != null)
            {
                while (current != null)
                {
                    toProcess.Push(current);
                    current = current.LeftChild;
                }

                var popped = toProcess.Pop();
                _path.Enqueue(popped);
                current = popped.RightChild;
            }
        }

        private void LevelOrderInspection()
        {
            var toProcess = new Queue<KDTNode<TKey, TValue>>();
            toProcess.Enqueue(_root);

            while (toProcess.Count > 0)
            {
                var current = toProcess.Dequeue();
                if (current != null)
                {
                    _path.Enqueue(current);
                    toProcess.Enqueue(current.LeftChild);
                    toProcess.Enqueue(current.RightChild);
                }
            }
        }
    }
}