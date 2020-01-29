using System.ComponentModel.DataAnnotations;

namespace Hungabor01Website.ViewModels
{
  /// <summary>
  /// ViewModel for ResetPassword view
  /// </summary>
  public class ResetPasswordViewModel
  {
    /// <summary>
    /// Email of the user (hidden)
    /// </summary>
    [EmailAddress]
    public string Email { get; set; }

    /// <summary>
    /// New password
    /// </summary>
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    /// <summary>
    /// Password confirmation
    /// </summary>
    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    [Compare("Password", ErrorMessage = "Password and Confirm Password must match")]
    public string ConfirmPassword { get; set; }

    /// <summary>
    /// The token for resetting the password
    /// </summary>
    public string Token { get; set; }
  }
}
