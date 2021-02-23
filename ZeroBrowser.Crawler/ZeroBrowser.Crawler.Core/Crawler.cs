using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZeroBrowser.Crawler.Common.Interfaces;
using ZeroBrowser.Crawler.Common.Models;

namespace ZeroBrowser.Crawler.Core
{
    public class Crawler : ICrawler
    {
        private readonly ILogger<Crawler> _logger;
        private readonly IHeadlessBrowserService _headlessBrowserService;
        private static string seedHostName = string.Empty;
        private readonly IBackgroundUrlQueue _backgroundUrlQueue;
        private readonly CrawlerOptions _crawlerOptions;
        private readonly IRepository _repository;
        private List<string> _blackList = new List<string> { "ws", "mailto" };

        public Crawler(ILogger<Crawler> logger,
                       IHeadlessBrowserService headlessBrowserService,
                       IOptions<CrawlerOptions> crawlerOptions,
                       IRepository repository,
                       IBackgroundUrlQueue backgroundUrlQueue)
        {
            _logger = logger;
            _headlessBrowserService = headlessBrowserService;
            _crawlerOptions = crawlerOptions.Value;
            _repository = repository;
            _backgroundUrlQueue = backgroundUrlQueue;
        }


        public async Task Crawl(CrawlerContext crawlerContext, int index)
        {
            var url = crawlerContext.CurrentUrl;

            //very first time lets populate the seed host name and cache it (static)
            //TODO need to set this once somehow.
            if (crawlerContext.IsSeed)
                seedHostName = new Uri(url).Host.Replace("www.", string.Empty);

            Task.Delay(_crawlerOptions.PolitenessDelay).Wait();

            var urls = await _headlessBrowserService.GetUrls(url, index);

            if (urls == null)
                return;

            foreach (var newUrl in urls)
            {
                _logger.LogInformation($"***** new url found {url}.{Environment.NewLine}");

                if (!isUrlAllowed(newUrl.Url))
                    continue;

                _backgroundUrlQueue.EnqueteUrlItem(newUrl.Url);
            }
        }

        private bool isUrlAllowed(string url)
        {
            //lets not crawl if the site is outside seed url (main site)
            if (!url.Contains(seedHostName))
                return false;

            //lets remove not so intresting protocls
            if (_blackList.Any(badKeyWord => url.Substring(0, badKeyWord.Length) == badKeyWord))
                return false;




            return true;
        }
    }
}
