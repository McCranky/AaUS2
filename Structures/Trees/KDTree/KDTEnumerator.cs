using System;
using System.Collections;
using System.Collections.Generic;
using Structures.Trees.Tree;

namespace Structures.Trees.KDTree
{
    public class KDTEnumerator<TKey, TValue> : IEnumerator<KDTNode<TKey, TValue>> where TKey : IComparable
    {
        private Queue<KDTNode<TKey, TValue>> _path;
        private KDTNode<TKey, TValue> _root;
        private KDTNode<TKey, TValue> _current;
        
        public KDTEnumerator(KDTNode<TKey, TValue> root)
        {
            _root = root;
            _path = new Queue<KDTNode<TKey, TValue>>();
            PopulatePath();
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
            PopulatePath();
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

        private void PopulatePath()
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
    }
}