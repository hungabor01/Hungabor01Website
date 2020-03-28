using Hungabor01Website.Constants.Strings;
using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;

namespace Hungabor01Website.ViewModels.CustomAttributes
{
    public class AllowedExtensionsAttribute : ValidationAttribute
    {
        private readonly string[] _extensions;

        public AllowedExtensionsAttribute(params string[] extensions)
        {
            _extensions = extensions.Select(s => s.ToLower()).ToArray();
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                var file = value as IFormFile;
                var extension = Path.GetExtension(file?.FileName);

                if (string.IsNullOrWhiteSpace(extension) || !_extensions.Contains(extension.ToLower()))
                {
                    return new ValidationResult(Strings.FileExtensionIsNotValid);
                }
            }

            return ValidationResult.Success;
        }
    }
}
