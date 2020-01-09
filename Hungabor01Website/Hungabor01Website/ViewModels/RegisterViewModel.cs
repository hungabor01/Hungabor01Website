using System.ComponentModel.DataAnnotations;

namespace Hungabor01Website.ViewModels
{
  public class RegisterViewModel
  {
    /// <summary>
    /// Username of the user (unique)
    /// </summary>
    [Required]
    public string Username { get; set; }

    /// <summary>
    /// Email of the user
    /// </summary>
    [Required]
    [EmailAddress]
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
