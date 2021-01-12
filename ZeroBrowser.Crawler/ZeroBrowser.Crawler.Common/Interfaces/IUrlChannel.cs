using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ZeroBrowser.Crawler.Common.Interfaces
{
    public interface IUrlChannel
    {
        Task Insert(string url);

        Task<IAsyncEnumerable<string>> Read();
    }
}
