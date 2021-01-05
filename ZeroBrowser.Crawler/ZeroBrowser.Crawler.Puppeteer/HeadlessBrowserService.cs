using Newtonsoft.Json;
using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ZeroBrowser.Crawler.Common.Interfaces;
using ZeroBrowser.Crawler.Common.Models;

namespace ZeroBrowser.Crawler.Puppeteer
{
    public class HeadlessBrowserService : IHeadlessBrowserService
    {
        private readonly IManageHeadlessBrowser _manageHeadlessBrowser;

        public HeadlessBrowserService(IManageHeadlessBrowser manageHeadlessBrowser)
        {           
            _manageHeadlessBrowser = manageHeadlessBrowser;
        }

        public async Task<IEnumerable<WebPage>> GetUrls(string url, int jobIndex)
        {
            var page = await gotoUrl(url, jobIndex);

            var jquerySelector = "$('a[href]')";

            var element = await page.EvaluateFunctionAsync(@"(jquerySelector) => {
                    const $ = window.$;
                    var links = eval(jquerySelector).toArray();

                    var urls = [];
                    $(links).each(function() {
                       urls.push( this.href ); 
                    });

                    return JSON.stringify(urls);
                }", jquerySelector);

            await _manageHeadlessBrowser.ClosePage(jobIndex);

            var json = element.ToString();

            if (!string.IsNullOrEmpty(json))
            {
                var results = JsonConvert.DeserializeObject<string[]>(json);
                return results.ToList().Select(l => new WebPage { Url = l });
            }

            return new List<WebPage>();
        }

        public async Task<HttpStatusCode> HealthCheck(string url, int jobIndex)
        {
            var page = await _manageHeadlessBrowser.GetPage<Page>(jobIndex);

            var response = await page.GoToAsync(url);

            await _manageHeadlessBrowser.ClosePage(jobIndex);

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
