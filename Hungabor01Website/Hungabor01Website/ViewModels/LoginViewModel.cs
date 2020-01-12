using System.ComponentModel.DataAnnotations;

namespace Hungabor01Website.ViewModels
{
  /// <summary>
  /// Viewmodel to transfer data from Login action to the view
  /// </summary>
  public class LoginViewModel
  {
    /// <summary>
    /// Username or email of the user, both works for login 
    /// </summary>
    [Required]
    [Display(Name = "Username or Email")]
    public string UsernameOrEmail { get; set; }

    /// <summary>
    /// Password of the user
    /// </summary>
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    /// <summary>
    /// Remembers the user with permanent cookies
    /// </summary>
    [Display(Name = "Remember me")]
    public bool RememberMe { get; set; }

    /// <summary>
    /// The original url when an unauthorized page is requested
    /// </summary>
    public string ReturnUrl { get; set; }
  }
}
