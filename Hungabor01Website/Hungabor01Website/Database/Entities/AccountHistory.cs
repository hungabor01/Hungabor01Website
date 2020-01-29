using Microsoft.AspNetCore.Identity;
using System;

namespace Hungabor01Website.Database.Entities
{
  /// <summary>
  /// Enum to identify the type of the action, the user has taken
  /// </summary>
  public enum UserActionType
  {
    None = 0,
    Login = 1,
    Registration = 2,
    Logout = 3,
    ProfilePictureChanged = 4,
    ProfilePictureDeleted = 5,
    PasswordChangeEmailSent = 6,
    PasswordChanged = 7,
    ConfirmationEmailSent = 8,
    EmailConfirmed = 9,
    ForogotPasswordEmailSent = 10
  }

  /// <summary>
  /// Entity for the Accounthistories table
  /// </summary>
  public class AccountHistory
  {
    /// <summary>
    /// Primary key for the table
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Foreign key for the user
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// Navigation property for the user
    /// </summary>
    public ApplicationUser User { get; set; }

    /// <summary>
    /// Date and time of the action
    /// </summary>
    public DateTime DateTime { get; set; }

    /// <summary>
    /// Type of the action
    /// </summary>
    public UserActionType Type { get; set; }

    /// <summary>
    /// Free text description about the action
    /// </summary>
    public string Description { get; set; }
  }
}
