using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ZeroBrowser.Crawler.Core.CustomValidations
{
    public class SeedUrlsValidation : ValidationAttribute
    {        
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var _seedUrls = value as string[];

            foreach (var seedUrl in _seedUrls)
            {
                if (seedUrl == default(string) || seedUrl.Length == 0)
                    return new ValidationResult("Null/Empty");

                if (!Uri.TryCreate(seedUrl, UriKind.Absolute, out Uri result))
                {
                    return new ValidationResult("Invalid uri");
                }
                else if (result.Scheme != "http" || result.Scheme != "https")
                {
                    return new ValidationResult("Invalid scheme");
                }
            }

            return ValidationResult.Success;
        }
    }
}
