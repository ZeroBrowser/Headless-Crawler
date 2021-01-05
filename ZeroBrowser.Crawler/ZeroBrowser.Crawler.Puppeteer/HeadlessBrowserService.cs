using Newtonsoft.Json;
using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using ZeroBrowser.Crawler.Common.Interfaces;
using ZeroBrowser.Crawler.Common.Models;

namespace ZeroBrowser.Crawler.Puppeteer
{
    public class HeadlessBrowserService : IHeadlessBrowserService
    {
        private Dictionary<string, List<WebPage>> _data = new Dictionary<string, List<WebPage>>();
        private readonly IManageHeadlessBrowser _manageHeadlessBrowser;

        public HeadlessBrowserService(IManageHeadlessBrowser manageHeadlessBrowser)
        {
            _data.Add("https://www.0browser.com", new List<WebPage> {
                new WebPage { Url = "http://www.url21.com" },
                new WebPage { Url = "http://www.url22.com" } });

            _data.Add("http://www.url21.com", new List<WebPage> {
                new WebPage { Url = "http://www.url31.com" },
                new WebPage { Url = "http://www.url32.com" } });

            _data.Add("http://www.url22.com", new List<WebPage> {
                new WebPage { Url = "http://www.url33.com" },
                new WebPage { Url = "http://www.url34.com" } });


            _data.Add("http://www.url31.com", new List<WebPage> {
                new WebPage { Url = "http://www.url41.com" },
                new WebPage { Url = "http://www.url42.com" } });

            _data.Add("http://www.url32.com", new List<WebPage> {
                new WebPage { Url = "http://www.url43.com" },
                new WebPage { Url = "http://www.url44.com" } });

            _manageHeadlessBrowser = manageHeadlessBrowser;
        }

        public async Task<IEnumerable<WebPage>> GetUrls(string url, int jobIndex)
        {
            var page = await gotoUrl(url, jobIndex);

            var jquerySelector = "$(a[href])";

            var element = await page.EvaluateFunctionAsync(@"(jquerySelector) => {
                    const $ = window.$;
                    var links = eval(jquerySelector).toArray();                    
                    return JSON.stringify(links);
                }", jquerySelector);


            var json = element.ToString();

            if (!string.IsNullOrEmpty(json))
            {
                var results = JsonConvert.DeserializeObject<string[]>(json);

            }

            return _data.ContainsKey(url) ? _data[url] : new List<WebPage>();
        }

        public async Task<HttpStatusCode> HealthCheck(string url, int jobIndex)
        {
            var page = await _manageHeadlessBrowser.GetPage<Page>(jobIndex);

            var response = await page.GoToAsync(url);

            return response.Status;
        }

        private async Task<Page> gotoUrl(string url, int jobIndex)
        {
            var page = await _manageHeadlessBrowser.GetPage<Page>(jobIndex);
            await page.GoToAsync(url);
            await page.WaitForSelectorAsync("body");
            return page;
        }

    }
}
