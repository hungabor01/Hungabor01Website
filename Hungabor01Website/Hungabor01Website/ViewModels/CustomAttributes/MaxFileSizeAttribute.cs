using Hungabor01Website.Constants.Strings;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Hungabor01Website.ViewModels.CustomAttributes
{
    public class MaxFileSizeAttribute : ValidationAttribute
    {
        private readonly int _maxFileSizeInKb;

        public MaxFileSizeAttribute(int maxFileSizeInKb)
        {
            _maxFileSizeInKb = maxFileSizeInKb;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                var file = value as IFormFile;

                if (file == null || file.Length > _maxFileSizeInKb)
                {
                    return new ValidationResult(string.Format(Strings.MaxFileSizeError, _maxFileSizeInKb.ToString()));
                }
            }

            return ValidationResult.Success;
        }
    }
}
