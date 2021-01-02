using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZeroBrowser.Crawler.Common.Interfaces;

namespace ZeroBrowser.Crawler.Core
{
    public class SQLiteRepository : IRepository
    {
        public async Task<bool> Exist(Uri url)
        {
            return false;
        }
    }
}
