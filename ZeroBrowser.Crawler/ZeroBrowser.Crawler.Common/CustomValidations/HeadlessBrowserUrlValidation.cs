using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ZeroBrowser.Crawler.Common.CustomValidations
{
    public class HeadlessBrowserUrlValidation : UrlValidation
    {
        public HeadlessBrowserUrlValidation() : base(new HashSet<string> { "ws", "wss" })
        {

        }
    }
}
