using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ZeroBrowser.Crawler.Common.Interfaces
{
    public interface IRepository
    {
        Task<bool> Exist(string url);

        Task AddPages(string parentUrl, List<string> pagesToCrawl);

        Task UpdateHttpStatusCode(string url, HttpStatusCode statusCode);

        Task<T> GetCrawledRecord<T>(Expression<Func<T, bool>> source) where T :class;
    }
}
