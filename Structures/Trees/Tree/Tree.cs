using System.Collections;
using System.Collections.Generic;
using Structures.Common;

namespace Structures.Trees.Tree
{
    class Tree<T> : Structure, IEnumerable<T>
    {

        public IEnumerator<T> GetEnumerator()
        {
            
            throw new System.NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}