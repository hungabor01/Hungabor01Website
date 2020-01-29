using Hungabor01Website.ViewModels.CustomAttributes;
using Microsoft.AspNetCore.Http;

namespace Hungabor01Website.ViewModels
{
  /// <summary>
  /// ViewModel for EditAccount view
  /// </summary>
  public class EditAccountViewModel
  {
    /// <summary>
    /// Profile picture of the user
    /// </summary>
    [MaxFileSize(5 * 1024 * 1024)]
    [AllowedExtensions(".jpg", ".png")]
    public IFormFile ProfilePicture { get; set; }
  }
}
