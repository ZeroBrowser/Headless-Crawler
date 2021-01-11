using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Contrib.WaitAndRetry;
using PuppeteerSharp;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using ZeroBrowser.Crawler.Common.Interfaces;
using ZeroBrowser.Crawler.Common.Models;

namespace ZeroBrowser.Crawler.Puppeteer
{
    public class ManageHeadlessBrowser : IManageHeadlessBrowser
    {
        private readonly ILogger<ManageHeadlessBrowser> _logger;
        private Browser _browser;
        public ConcurrentDictionary<int, Page> BrowserLookup = new ConcurrentDictionary<int, Page>();
        private readonly CrawlerOptions _crawlerOptions;

        public ManageHeadlessBrowser(IOptions<CrawlerOptions> crawlerOptions, ILogger<ManageHeadlessBrowser> logger)
        {
            _crawlerOptions = crawlerOptions.Value;
            _logger = logger;
        }

        public async Task Init()
        {
            _logger.LogInformation($"******* init headless browser{Environment.NewLine}");

            await connectToRemoteBrowser();

            if (!_crawlerOptions.LazyInitBrowserPage)
                //lets create pages
                for (int index = 0; index < _crawlerOptions.NumberOfParallelInstances; index++)
                {
                    await GetPage<Page>(index);
                }
        }

        private async Task connectToRemoteBrowser()
        {
            var options = new ConnectOptions()
            {
                BrowserWSEndpoint = _crawlerOptions.HeadlessBrowserUrl,
            };

            //lets try a couple of times
            var delay = Backoff.ConstantBackoff(TimeSpan.FromMilliseconds(1000), retryCount: 5);

            var policy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(delay, onRetry: (response, delay, retryCount, context) =>
            {
                _logger.LogInformation($"******* retry #{retryCount}{Environment.NewLine}");
            });

            await policy.ExecuteAsync(async () =>
            {
                _logger.LogInformation($"******* trying to connect to headless browser.{Environment.NewLine}");
                _browser = await PuppeteerSharp.Puppeteer.ConnectAsync(options);
            });
        }

        /// <summary>
        /// lets create/get browser pages based on index
        /// </summary>
        /// <param name="index">concurrency index</param>
        /// <returns>Puppeteer Page object</returns>
        public async Task<T> GetPage<T>(int index) where T : class
        {
            if (_browser == null || _browser.IsClosed)
                await Init();


            BrowserLookup.TryGetValue(index, out Page page);
            if (page == null)
            {
                page = await _browser.NewPageAsync();

                //lets inject jquery url to any page so we can use it in our queries
                await page.AddScriptTagAsync(_crawlerOptions.JQueryUrl);

                if (BrowserLookup.TryAdd(index, page))
                    return page as T;
            }
            return page as T;
        }

        public async Task ClosePage(int index)
        {
            BrowserLookup.TryGetValue(index, out Page page);
            if (page == null)
            {
                await page.CloseAsync();
                BrowserLookup.TryUpdate(index, null, page);
            }
        }
    }
}
