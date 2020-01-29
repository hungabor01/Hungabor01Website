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

    public AllowedExtensionsAttribute(params string[] extensions)
    {
      this.extensions = extensions.Select(x => x.ToLower()).ToArray();
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
      if (value != null)
      {
        var file = value as IFormFile;
        
        var extension = Path.GetExtension(file?.FileName);

        if (string.IsNullOrWhiteSpace(extension) || !extensions.Contains(extension.ToLower()))
        {
          return new ValidationResult(Strings.FileExtensionIsNotValid);
        }
      }

      return ValidationResult.Success;
    }
  }
}
