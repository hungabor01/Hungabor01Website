using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Hungabor01Website.ViewModels
{
    public class RegistrationViewModel
    {
        [Required]
        [Remote(action: "IsUsernameInUse", controller: "Registration")]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        [Remote(action: "IsEmailInUse", controller: "Registration")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "Password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
