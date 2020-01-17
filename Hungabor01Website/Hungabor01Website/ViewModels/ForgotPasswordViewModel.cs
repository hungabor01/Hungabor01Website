using System.ComponentModel.DataAnnotations;

namespace Hungabor01Website.ViewModels
{
  /// <summary>
  /// Viewmodel to transfer data from ForgotPassword action to the view
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