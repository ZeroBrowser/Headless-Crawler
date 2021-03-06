﻿using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using Polly.Contrib.WaitAndRetry;
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
        private readonly ILogger<HeadlessBrowserService> _logger;

        public HeadlessBrowserService(IManageHeadlessBrowser manageHeadlessBrowser, ILogger<HeadlessBrowserService> logger)
        {
            _manageHeadlessBrowser = manageHeadlessBrowser;
            _logger = logger;
        }

        public async Task<IEnumerable<WebPage>> GetUrls(string url, int jobIndex)
        {
            try
            {
                //lets try a couple of times
                var delay = Backoff.ConstantBackoff(TimeSpan.FromMilliseconds(1000), retryCount: 5);

                var policy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(delay, onRetry: (response, delay, retryCount, context) =>
                {
                    _logger.LogInformation($"******* retry GetUrls #{retryCount}{Environment.NewLine}");
                });

                IEnumerable<WebPage> results = await policy.ExecuteAsync<IEnumerable<WebPage>>(async () =>
                {
                    return await getWebPage(url, jobIndex);
                });

                return results;
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"oops failed after many attempts: {ex.Message}");

                return null;
            }
        }


        private async Task<IEnumerable<WebPage>> getWebPage(string url, int jobIndex)
        {
            var page = await gotoUrl(url, jobIndex);

            var element = await page.EvaluateFunctionAsync(@"() => {                   
                    var urls = Array.from(
                        document.querySelectorAll('a[href]'),
                        a => a.getAttribute('href')
                      );

                    return JSON.stringify(urls);
                }");

            await _manageHeadlessBrowser.ClosePage(jobIndex);

            var json = element.ToString();

            if (!string.IsNullOrEmpty(json))
            {
                var results = JsonConvert.DeserializeObject<string[]>(json);

                //lets remove duplicates
                return results.Distinct<string>().Select(l => new WebPage { Url = l });
            }

            return new List<WebPage>();
        }

        public async Task<HttpStatusCode> HealthCheck(string url, int jobIndex)
        {
            var page = await _manageHeadlessBrowser.GetPage<Page>(jobIndex);

            var response = await page.GoToAsync(url);

            await _manageHeadlessBrowser.ClosePage(jobIndex);

            return response == null ? HttpStatusCode.NotFound : response.Status;
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
