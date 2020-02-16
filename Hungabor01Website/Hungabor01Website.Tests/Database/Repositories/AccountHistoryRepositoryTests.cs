using Hungabor01Website.Database;
using Hungabor01Website.Database.Entities;
using Hungabor01Website.Database.Repositories.Classes;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;
using System.Collections.Generic;
using Hungabor01Website.Tests.Helpers;
using System.Linq;

namespace Hungabor01Website.Tests.Database.Repositories
{
  public class AccountHistoryRepositoryTests : IDisposable
  {
    private readonly ConfigurationHelper configurationHelper;
    private readonly ServiceProviderHelper serviceProviderHelper;
    private readonly DatabaseHelper databaseHelper;
    private readonly IdentityHelper identityHelper;

    public AccountHistoryRepositoryTests()
    {
      configurationHelper = new ConfigurationHelper();
      serviceProviderHelper = new ServiceProviderHelper(configurationHelper.Configuration);
      databaseHelper = new DatabaseHelper(configurationHelper.Configuration);
      identityHelper = new IdentityHelper("AccountHistoryRepoTest");
      identityHelper.AddTestUser(databaseHelper.Context);
    }

    [Fact]
    public void LogUserActionToDatabase_UserIsNull_ThrowArgumentNullException()
    {
      try
      {
        var repo = new AccountHistoryRepository();

        repo.LogUserActionToDatabase(null, UserActionType.None, string.Empty);           

        Assert.True(false, "No exception was thrown.");
      }
      catch (ArgumentNullException ex)
      {
        Assert.Equal("user", ex.ParamName);
      }
      catch (Exception)
      {
        Assert.True(false, "Wrong type of exception was thrown.");
      }
    }

    [Fact]
    public void LogUserActionToDatabase_UserIsValid_ActionIsLoggedToDatabase()
    {      
      var originalCount = 0;
      using (var unitOfWork = serviceProviderHelper.ServiceProvider.GetService<IUnitOfWork>())
      {
        originalCount = unitOfWork.AccountHistoryRepository.GetAll().ToList().Count;
      }

      using (var unitOfWork = serviceProviderHelper.ServiceProvider.GetService<IUnitOfWork>())
      {
        unitOfWork.AccountHistoryRepository.LogUserActionToDatabase(identityHelper.TestUser, UserActionType.None, string.Empty);
        unitOfWork.Complete();        
      }

      var newCount = 0;
      using (var unitOfWork = serviceProviderHelper.ServiceProvider.GetService<IUnitOfWork>())
      {
        newCount = unitOfWork.AccountHistoryRepository.GetAll().ToList().Count;

        var records = unitOfWork.AccountHistoryRepository.Find(x => x.UserId == identityHelper.TestUser.Id && x.Type == UserActionType.None);
        Assert.Single(records);
      }

      Assert.Equal(originalCount + 1, newCount);

      using (var unitOfWork = serviceProviderHelper.ServiceProvider.GetService<IUnitOfWork>())
      {
        var record = unitOfWork.AccountHistoryRepository.SingleOrDefault(x => x.UserId == identityHelper.TestUser.Id && x.Type == UserActionType.None);
        unitOfWork.AccountHistoryRepository.Remove(record);
        unitOfWork.Complete();
      }
    }

    public void Dispose()
    {
      identityHelper.RemoveTestUser(databaseHelper.Context);
      databaseHelper.Dispose();
    }
  }
}