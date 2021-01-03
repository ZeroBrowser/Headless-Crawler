using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZeroBrowser.Crawler.Common.Interfaces;
using ZeroBrowser.Crawler.Common.Models;

namespace ZeroBrowser.Crawler.Puppeteer
{
    public class HeadlessBrowserService : IHeadlessBrowserService
    {
        private Dictionary<string, List<WebPage>> _data = new Dictionary<string, List<WebPage>>();

        public HeadlessBrowserService()
        {
            _data.Add("http://www.url1.com", new List<WebPage> {
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
        }

        public async Task<IEnumerable<WebPage>> GetUrls(string url)
        {
            return _data.ContainsKey(url) ? _data[url] : new List<WebPage>();
        }

        //TODO: delete
        public async Task<WebPage> GetWebPage(string seedUrl, string headlessBrowserUrl)
        {
            var options = new ConnectOptions()
            {
                BrowserWSEndpoint = headlessBrowserUrl,
            };

            using (var browser = await PuppeteerSharp.Puppeteer.ConnectAsync(options))
            {
                using (var page = await browser.NewPageAsync())
                {
                    //var cdpSessopm = await page.Target.CreateCDPSessionAsync();
                    //cdpSessopm.MessageReceived += (o, m) =>
                    //{

                    //};

                    var response = await page.GoToAsync(seedUrl);
                    var rurl = page.Url;
                    //var html = page.

                    await page.ScreenshotAsync("capture.jpg");
                    await page.CloseAsync();
                }
            }

            return new WebPage();
        }

    }
}
