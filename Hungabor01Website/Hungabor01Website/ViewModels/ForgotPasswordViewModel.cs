using System.ComponentModel.DataAnnotations;

namespace Hungabor01Website.ViewModels
{
  /// <summary>
  /// ViewModel for ForgotPassword view
  /// </summary>
  public class ForgotPasswordViewModel
  {
    /// <summary>
    /// The email address of the user, who forgot the password
    /// </summary>
    [Required]
    [EmailAddress]
    public string Email { get; set; }
  }
}