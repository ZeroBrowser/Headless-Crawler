using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ZeroBrowser.Crawler.Common.Interfaces
{
    public interface IRepository
    {
        Task<bool> Exist(string url);
    }
}
