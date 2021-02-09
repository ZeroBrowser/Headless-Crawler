using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ZeroBrowser.Crawler.Common.CustomValidations
{
    public class SeedUrlsValidation : UrlValidation
    {
        public SeedUrlsValidation() : base(new HashSet<string> { "http", "https" })
        {

        }
    }
}
