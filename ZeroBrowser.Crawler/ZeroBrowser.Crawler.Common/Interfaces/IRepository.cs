using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ZeroBrowser.Crawler.Common.Models;

namespace ZeroBrowser.Crawler.Common.Interfaces
{
    public interface IRepository
    {
        Task<bool> Exist(string url);

        Task AddPage(CrawlerContext crawlerContext);

        Task UpdateHttpStatusCode(string url, HttpStatusCode statusCode);

        Task<T> GetCrawledRecord<T>(Expression<Func<T, bool>> source) where T :class;
    }
}
