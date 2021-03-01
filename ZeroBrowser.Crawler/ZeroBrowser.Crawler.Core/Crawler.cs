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
        private static Uri seedUri;
        private readonly IBackgroundUrlQueue _backgroundUrlQueue;
        private readonly CrawlerOptions _crawlerOptions;
        private List<string> _blackList = new List<string> { "ws", "mailto" };

        public Crawler(ILogger<Crawler> logger,
                       IHeadlessBrowserService headlessBrowserService,
                       IOptions<CrawlerOptions> crawlerOptions,
                       IBackgroundUrlQueue backgroundUrlQueue)
        {
            _logger = logger;
            _headlessBrowserService = headlessBrowserService;
            _crawlerOptions = crawlerOptions.Value;
            _backgroundUrlQueue = backgroundUrlQueue;
        }


        public async Task Crawl(CrawlerContext crawlerContext, int index)
        {
            var url = crawlerContext.CurrentUrl;

            //very first time lets populate the seed host name and cache it (static)
            //TODO need to set this once somehow.
            if (crawlerContext.IsSeed)
            {
                seedUri = new Uri(url);
                seedHostName = seedUri.Host.Replace("www.", string.Empty);
            }

            await Task.Delay(_crawlerOptions.PolitenessDelay);

            var urls = await _headlessBrowserService.GetUrls(url, index);

            if (urls == null)
                return;

            foreach (var newUrl in urls)
            {
                _logger.LogInformation($"***** new URL found {newUrl}.{Environment.NewLine}");

                var cleanedUrl = cleanUrl(newUrl.Url);

                if (!isUrlAllowed(cleanedUrl))
                    continue;

                _backgroundUrlQueue.EnqueteUrlItem(cleanedUrl);
            }
        }

        private string cleanUrl(string url)
        {
            //clean up and remove fragments 
            url = url.Remove(url.Length - seedUri.Fragment.Length, seedUri.Fragment.Length);

            if (url.EndsWith("/"))
                url = url.Remove(url.Length - 1, 1);

            //local url if starts with ./
            if (url.StartsWith("./"))
            {
                url = $"{seedUri.Scheme}://{seedHostName}{url.Substring(1, url.Length - 1)}";
            }
            else if (url.StartsWith("/"))
            {
                url = $"{seedUri.Scheme}://{seedHostName}{url}";
            }
            else
            {

            }

            return url;
        }

        private bool isUrlAllowed(string url)
        {
            //lets not crawl if the site is outside seed URL (main site)
            if (!url.Contains(seedHostName))
                return false;

            //lets remove not so interesting protocols
            if (_blackList.Any(badKeyWord => url.Substring(0, badKeyWord.Length) == badKeyWord))
                return false;

            return true;
        }
    }
}
