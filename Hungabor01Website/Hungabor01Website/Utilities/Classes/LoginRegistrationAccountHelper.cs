using Hungabor01Website.Database;
using Hungabor01Website.Database.Entities;
using System;
using Microsoft.Extensions.DependencyInjection;
using Hungabor01Website.Utilities.Interfaces;

namespace Hungabor01Website.Utilities.Classes
{
  /// <summary>
  /// Helper class for the account related controllers
  /// </summary>
  public class LoginRegistrationAccountHelper : ILoginRegistrationAccountHelper
  {
    private readonly IServiceProvider serviceProvider;

    public LoginRegistrationAccountHelper(IServiceProvider serviceProvider)
    {
      this.serviceProvider = serviceProvider;
    }

    public void LogUserActionToDatabase(ApplicationUser user, UserActionType type, string description = null)
    {
      using (var unitOfWork = serviceProvider.GetService<IUnitOfWork>())
      {
        unitOfWork.AccountHistoryRepository.LogUserActionToDatabase(
          user, type, description);

        unitOfWork.Complete();
      }
    }
  }
}
