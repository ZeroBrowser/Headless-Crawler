using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ZeroBrowser.Crawler.Common.Models;

namespace ZeroBrowser.Crawler.Common.Interfaces
{
    public interface IHeadlessBrowserService
    {
        Task<IEnumerable<WebPage>> GetUrls(string url, int jobIndex);

        Task<HttpStatusCode> HealthCheck(string url, int jobIndex);
    }
}
