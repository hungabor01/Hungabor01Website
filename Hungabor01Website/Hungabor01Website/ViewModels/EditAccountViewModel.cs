using Hungabor01Website.ViewModels.CustomAttributes;
using Microsoft.AspNetCore.Http;

namespace Hungabor01Website.ViewModels
{
    public class EditAccountViewModel
    {
        [MaxFileSize(5 * 1024 * 1024)]
        [AllowedExtensions(".jpg", ".png")]
        public IFormFile ProfilePicture { get; set; }
    }
}
