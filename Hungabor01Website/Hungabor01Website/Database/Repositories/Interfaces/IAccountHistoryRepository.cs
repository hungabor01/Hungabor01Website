using Hungabor01Website.Database.Entities;

namespace Hungabor01Website.Database.Repositories.Interfaces
{
  /// <summary>
  /// The repository interface for the custom methods of AccountHistories table
  /// </summary>
  public interface IAccountHistoryRepository : IRepository<AccountHistory>
  {
    /// <summary>
    /// Adds the action of the user to the AccountHistories table
    /// </summary>
    /// <param name="user">The user, who has taken the action</param>
    /// <param name="type">Type of the action</param>
    /// <param name="description">Free text description about the action</param>
    public void LogUserActionToDatabase(ApplicationUser user, UserActionType type, string description);
  }
}
