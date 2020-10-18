using Structures.Common;

namespace Structures.Trees.Tree
{
    class TreeNode<T> : DataItem<T>
    {
        public TreeNode<T> Parent { get; private set; }

        public TreeNode(T data) : base(data)
        {
            Parent = null;
        }

        public TreeNode(TreeNode<T> other) : base(other)
        {
            Parent = other.Parent;
        }

        public bool IsRoot => Parent == null;
        public void ResetParent()
        {
            Parent = null;
        }

        public void SetParent(TreeNode<T> parent)
        {
            Parent = parent;
        }

        public virtual TreeNode<T> ShallowCopy() => null;
        public virtual TreeNode<T> DeepCopy() => null;
        public virtual bool IsLeaf() => false;
        public virtual TreeNode<T> GetBrother(int brotherOrder) => null;
        public virtual TreeNode<T> GetSon(int sonOrder) => null;
        public virtual void InsertSon(TreeNode<T> son, int order) { }
        public virtual TreeNode<T> ReplaceSon(TreeNode<T> son, int order) => null;
        public virtual TreeNode<T> RemoveSon(int order) => null;
        public virtual int Degree() => 0;
        public virtual int SizeOfSubtree() => 0;
    }
}