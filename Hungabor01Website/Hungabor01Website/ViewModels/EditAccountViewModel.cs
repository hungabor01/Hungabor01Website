using Hungabor01Website.ViewModels.CustomAttributes;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Hungabor01Website.ViewModels
{
    public class EditAccountViewModel
    {
        [MaxFileSize(5 * 1024 * 1024)]
        [AllowedExtensions(".jpg", ".png")]
        [Display(Name = "Profile Picture")]
        public IFormFile ProfilePicture { get; set; }
    }
}
