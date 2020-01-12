using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Hungabor01Website.ViewModels
{
  /// <summary>
  /// Viewmodel to transfer data from Register action to the view
  /// </summary>
  public class RegisterViewModel
  {
    /// <summary>
    /// Username of the user (unique)
    /// </summary>
    [Required]
    [Remote(action: "IsUsernameInUse", controller: "Account")]
    public string Username { get; set; }

    /// <summary>
    /// Email of the user (unique)
    /// </summary>
    [Required]
    [EmailAddress]
    [Remote(action: "IsEmailInUse", controller: "Account")]
    public string Email { get; set; }

    /// <summary>
    /// Password of the user
    /// </summary>
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    /// <summary>
    /// Password confirmation
    /// </summary>
    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    [Compare("Password", ErrorMessage = "Password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; }
    
  }
}
