using Hungabor01Website.Database.Entities;
using Hungabor01Website.Database.Repositories.Interfaces;
using Hungabor01Website.Utilities.Classes;
using System;

namespace Hungabor01Website.Database.Repositories.Classes
{
  /// <summary>
  /// The repository class for the custom methods of AcountHistories table
  /// </summary>
  public class AccountHistoryRepository : Repository<AccountHistory>, IAccountHistoryRepository
  {
    public void LogUserActionToDatabase(ApplicationUser user, UserActionType type, string description)
    {
      user.ThrowExceptionIfNull(nameof(user));

      var action = new AccountHistory
      {
        UserId = user.Id,
        DateTime = DateTime.Now,
        Type = type,
        Description = description
      };

      Add(action);
    }
  }
}
