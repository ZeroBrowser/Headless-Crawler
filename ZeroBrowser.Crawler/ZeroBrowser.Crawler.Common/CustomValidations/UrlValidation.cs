using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ZeroBrowser.Crawler.Common.CustomValidations
{
    public class UrlValidation : ValidationAttribute
    {
        HashSet<string> _validSchemes;

        public UrlValidation(HashSet<string> validSchemes)
        {
            _validSchemes = validSchemes;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var membersName = new[] { validationContext.MemberName };

            if (value.GetType().IsArray)
            {
                var urls = value as string[];

                foreach (var seedUrl in urls)
                {
                    return validate(seedUrl, membersName);
                }
            }
            else
            {
                var url = value as string;
                return validate(url, membersName);
            }

            return ValidationResult.Success;
        }


        private ValidationResult validate(string url, string[] membersName)
        {
            if (url == default(string) || url.Length == 0)
                return new ValidationResult("Null/Empty", membersName);

            if (!Uri.TryCreate(url, UriKind.Absolute, out Uri result))
            {
                return new ValidationResult("Invalid uri", membersName);
            }
            else if (!_validSchemes.Contains(result.Scheme))
            {
                return new ValidationResult("Invalid scheme", membersName);
            }
            else
                return ValidationResult.Success;
        }
    }
}
