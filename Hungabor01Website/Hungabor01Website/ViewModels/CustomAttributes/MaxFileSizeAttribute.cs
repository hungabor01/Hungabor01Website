using Hungabor01Website.Constants;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Hungabor01Website.ViewModels.CustomAttributes
{
  /// <summary>
  /// Attribute to allow files only with less than a certain size
  /// </summary>
  public class MaxFileSizeAttribute : ValidationAttribute
  {
    private readonly int maxFileSize;

    public MaxFileSizeAttribute(int maxFileSize)
    {
      this.maxFileSize = maxFileSize;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
      if (value != null)
      {
        var file = value as IFormFile;
       
        if (file?.Length > maxFileSize)
        {
          return new ValidationResult(string.Format(Strings.MaxFileSizeError, maxFileSize.ToString()));
        }       
      }

      return ValidationResult.Success;
    }
  }
}
