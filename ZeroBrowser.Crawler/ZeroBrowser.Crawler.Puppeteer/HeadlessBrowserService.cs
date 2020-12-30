using PuppeteerSharp;
using System;
using System.Threading.Tasks;
using ZeroBrowser.Crawler.Common.Interfaces;
using ZeroBrowser.Crawler.Common.Models;

namespace ZeroBrowser.Crawler.Puppeteer
{
    public class HeadlessBrowserService : IHeadlessBrowserService
    {
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
