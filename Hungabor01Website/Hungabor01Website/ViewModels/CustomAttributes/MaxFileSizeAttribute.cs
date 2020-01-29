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
    private readonly int maxFileSizeInKb;

    public MaxFileSizeAttribute(int maxFileSizeInKb)
    {
      this.maxFileSizeInKb = maxFileSizeInKb;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
      if (value != null)
      {
        var file = value as IFormFile;
       
        if (file == null || file.Length > maxFileSizeInKb)
        {
          return new ValidationResult(string.Format(Strings.MaxFileSizeError, maxFileSizeInKb.ToString()));
        }       
      }

      return ValidationResult.Success;
    }
  }
}
