using System.ComponentModel.DataAnnotations;

namespace Hungabor01Website.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}