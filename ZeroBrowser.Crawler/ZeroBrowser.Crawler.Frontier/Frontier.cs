using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using ZeroBrowser.Crawler.Common.Interfaces;
using ZeroBrowser.Crawler.Common.Models;

namespace ZeroBrowser.Crawler.Frontier
{
    public class Frontier : IFrontier
    {
        private readonly IUrlChannel _urlChannel;
        private readonly ILogger<Frontier> _logger;
        private FrontierState _frontierState;

        public Frontier(IUrlChannel urlProducer, ILogger<Frontier> logger, FrontierState frontierState)
        {
            _urlChannel = urlProducer;
            _logger = logger;
            _frontierState = frontierState;
        }

        /// <summary>
        /// It does what a Frontier does, add URL to queue for crawling
        /// </summary>
        /// <param name="crawlerContext">context</param>
        /// <returns>true if new URL found, false if existing</returns>
        public async Task<bool> Process(CrawlerContext crawlerContext)
        {
            if (crawlerContext.IsSeed)
                _frontierState.Reset();

            var url = crawlerContext.CurrentUrl;

            if (url == null)
                return false;

            if (Uri.TryCreate(url, UriKind.Absolute, out Uri result))
            {
                url = cleanUrl(url, result);

                if (_frontierState.CrawledUrls.ContainsKey(url))
                {
                    //existing URL
                    _frontierState.CrawledUrls[url]++;
                }
                else
                {
                    //add to queue
                    _logger.LogInformation($"New URL: {url}{Environment.NewLine}");

                    _frontierState.CrawledUrls.TryAdd(url, 1);
                    await _urlChannel.Insert(crawlerContext);

                    return true;
                }
            }

            return false;
        }

        private string cleanUrl(string url, Uri uri)
        {
            //clean up and remove fragments 
            url = url.Remove(url.Length - uri.Fragment.Length, uri.Fragment.Length);

            if (url.EndsWith("/"))
                url = url.Remove(url.Length - 1, 1);

            return url;
        }
    }
}
