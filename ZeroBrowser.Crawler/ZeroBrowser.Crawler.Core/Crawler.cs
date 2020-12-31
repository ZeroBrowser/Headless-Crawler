using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ZeroBrowser.Crawler.Common.Interfaces;
using ZeroBrowser.Crawler.Common.Models;
using ZeroBrowser.Crawler.Core.Interfaces;
using ZeroBrowser.Crawler.Core.Models;

namespace ZeroBrowser.Crawler.Core
{
    public class Crawler : ICrawler
    {
        private readonly string[] _seedUrls;
        private readonly string _headlessBrowserUrl;
        private readonly Parameters _parameters;
        private readonly IHeadlessBrowserService _headlessBrowserService;

        public Crawler(string[] seedUrls, string headlessBrowserUrl, IHeadlessBrowserService headlessBrowserService)
        {
            _seedUrls = seedUrls;
            _headlessBrowserUrl = headlessBrowserUrl;
            _headlessBrowserService = headlessBrowserService;
            _parameters = new Parameters(_seedUrls, _headlessBrowserUrl);
        }


        public async Task<IAsyncEnumerable<WebPage>> Crawl()
        {
            ValidateArgument();

            //1. lets get page information.

            //_headlessBrowserService.

            return null;
        }


        private void ValidateArgument()
        {
            //Validate _seedUrls
            var errorResults = new List<ValidationResult>();
            var context = new ValidationContext(_parameters, serviceProvider: null, items: null);
            if (!Validator.TryValidateObject(_parameters, context, errorResults, true))
            {
                var firstError = errorResults.FirstOrDefault();

                if (firstError != null)
                    throw new ArgumentException(firstError.ErrorMessage, firstError.MemberNames.First());
            }
        }

    }
}
