using Microsoft.Extensions.Options;
using PuppeteerSharp;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using ZeroBrowser.Crawler.Common.Interfaces;
using ZeroBrowser.Crawler.Common.Models;

namespace ZeroBrowser.Crawler.Puppeteer
{
    public class ManageHeadlessBrowser : IManageHeadlessBrowser
    {
        private Browser _browser;
        public ConcurrentDictionary<int, Page> BrowserLookup = new ConcurrentDictionary<int, Page>();
        private readonly CrawlerOptions _crawlerOptions;

        public ManageHeadlessBrowser(IOptions<CrawlerOptions> crawlerOptions)
        {
            _crawlerOptions = crawlerOptions.Value;
        }

        public async Task Init()
        {
            var options = new ConnectOptions()
            {
                BrowserWSEndpoint = _crawlerOptions.HeadlessBrowserUrl,
            };

            _browser = await PuppeteerSharp.Puppeteer.ConnectAsync(options);

            if (_crawlerOptions.LazyInitBrowserPage)
                //lets create pages
                for (int index = 0; index < _crawlerOptions.NumberOfParallelInstances; index++)
                {
                    await GetPage<PuppeteerSharp.Page>(index);
                }
        }

        /// <summary>
        /// lets create/get browser pages based on index
        /// </summary>
        /// <param name="index">concurrency index</param>
        /// <returns>Puppeteer Page object</returns>
        public async Task<T> GetPage<T>(int index) where T : class
        {
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
    }
}
