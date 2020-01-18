using Hungabor01Website.Constants;
using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;

namespace Hungabor01Website.ViewModels.CustomAttributes
{
  /// <summary>
  /// Attribute to allow files only with certain extension
  /// </summary>
  public class AllowedExtensionsAttribute : ValidationAttribute
  {
    private readonly string[] extensions;

    public AllowedExtensionsAttribute(string[] extensions)
    {
      this.extensions = extensions;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
      if (value != null)
      {
        var file = value as IFormFile;
        
        var extension = Path.GetExtension(file?.FileName);

        if (extension != null && !extensions.Contains(extension.ToLower()))
        {
          return new ValidationResult(Strings.FileExtensionIsNotValid);
        }
      }

      return ValidationResult.Success;
    }
  }
}
