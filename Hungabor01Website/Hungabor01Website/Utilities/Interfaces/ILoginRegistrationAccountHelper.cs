using Hungabor01Website.Database.Entities;

namespace Hungabor01Website.Utilities.Interfaces
{
  /// <summary>
  /// Helper interface for the account related controllers
  /// </summary>
  public interface ILoginRegistrationAccountHelper
  {
    /// <summary>
    /// Logs the action of the user to the database
    /// </summary>
    /// <param name="user">The user</param>
    /// <param name="type">The type of the action</param>
    /// <param name="description">The description of the action, if necessary</param>
    void LogUserActionToDatabase(ApplicationUser user, UserActionType type, string description = null);
  }
}