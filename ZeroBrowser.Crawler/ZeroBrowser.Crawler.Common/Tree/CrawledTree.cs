using System;
using System.Collections.Generic;
using System.Text;
using ZeroBrowser.Crawler.Common.Interfaces;
using ZeroBrowser.Crawler.Common.Models;

namespace ZeroBrowser.Crawler.Common.Tree
{
    public class CrawledTree<T> : ICrawledTree<T> where T : ICrawledPageInfo
    {
        public TreeNode<T> Root { get; set; }

        public TreeNode<T> Add(T value, TreeNode<T> parent)
        {
            if (Root == null && parent == null)
            {
                Root = new TreeNode<T>(value);
                return Root;
            }
            else
            {
                var newNode = new TreeNode<T>(value) { Parent = parent.Value };
                if (parent != null)
                {
                    parent.Children.AddLast(newNode);
                }
                return newNode;
            }
        }

        public TreeNode<T> Find(T value)
        {
            var queue = new Queue<TreeNode<T>>();
            queue.Enqueue(Root);

            while (Root != null && queue.Count > 0)
            {
                var currentNode = queue.Dequeue();

                if (currentNode != null && currentNode.Value.Url == value.Url)
                {
                    return currentNode;
                }
                foreach (var child in currentNode.Children)
                {
                    queue.Enqueue(child);
                }
            }

            return null;
        }

        public override string ToString()
        {



            return base.ToString();
        }
    }
}
