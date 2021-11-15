using System;
using System.Collections.Generic;
using System.Text;
using ZeroBrowser.Crawler.Common.Tree;

namespace ZeroBrowser.Crawler.Common.Interfaces
{    
    public interface ICrawledTree<T>
    {
        TreeNode<T> Root { get; set; }
        TreeNode<T> Add(T value, TreeNode<T> parent);
        TreeNode<T> Find(T value);
    }
}
