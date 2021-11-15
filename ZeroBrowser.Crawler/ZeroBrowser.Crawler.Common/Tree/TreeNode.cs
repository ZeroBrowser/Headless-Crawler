using System;
using System.Collections.Generic;
using System.Text;

namespace ZeroBrowser.Crawler.Common.Tree
{
    public class TreeNode<T>
    {
        public TreeNode(T value)
        {
            Value = value;
            Children = new LinkedList<TreeNode<T>>();
        }

        public T Parent { get; set; }
        public T Value { get; private set; }
        public LinkedList<TreeNode<T>> Children { get; set; }
    }
}
