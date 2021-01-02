using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using ZeroBrowser.Crawler.Common.CustomValidations;

namespace ZeroBrowser.Crawler.Common.Models
{
    public class Parameters
    {
        public Parameters(string[] seedUrls, string headlessBrowserUrl)
        {
            SeedUrls = seedUrls;
            HeadlessBrowserUrl = headlessBrowserUrl;
        }

        [Required]
        [SeedUrlsValidation]
        public string[] SeedUrls { get; private set; }

        [Required]
        [HeadlessBrowserUrlValidation]
        public string HeadlessBrowserUrl { get; private set; }

        //TODO: add limit for number of pages to crawl

    }
}
