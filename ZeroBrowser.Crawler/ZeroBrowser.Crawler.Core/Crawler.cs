using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ZeroBrowser.Crawler.Core.Interfaces;
using ZeroBrowser.Crawler.Core.Models;

namespace ZeroBrowser.Crawler.Core
{
    public class Crawler : ICrawler
    {
        private readonly string[] _seedUrls;
        private readonly string _headlessBrowserUrl;
        private readonly Parameters _parameters;

        public Crawler(string[] seedUrls, string headlessBrowserUrl)
        {
            _seedUrls = seedUrls;
            _headlessBrowserUrl = headlessBrowserUrl;

            _parameters = new Parameters(_seedUrls, _headlessBrowserUrl);
        }


        public async Task<IAsyncEnumerable<WebPage>> Crawl()
        {
            ValidateArgument();

            throw new NotImplementedException();
        }


        private void ValidateArgument()
        {
            //Validate _seedUrls
            var errorResults = new List<ValidationResult>();
            var context = new ValidationContext(_parameters, serviceProvider: null, items: null);
            if (!Validator.TryValidateObject(_parameters, context, errorResults, true))
            {
                var errors = string.Join(",", errorResults.Select(a => a.ErrorMessage));
                throw new ArgumentException(errors, "_seedUrls");
            }


            //Validate _headlessBrowserUrl

        }

    }
}
