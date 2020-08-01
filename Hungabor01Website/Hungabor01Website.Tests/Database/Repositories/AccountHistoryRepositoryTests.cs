using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;
using Hungabor01Website.Tests.Helpers;
using System.Linq;
using System.Threading.Tasks;
using Common.Enums;
using Database.Repositories.Classes;
using Database.UnitOfWork;

namespace Hungabor01Website.Tests.Database.Repositories
{
    public class AccountHistoryRepositoryTests : IDisposable
    {
        private readonly ConfigurationHelper _configurationHelper;
        private readonly ServiceProviderHelper _serviceProviderHelper;
        private readonly DatabaseHelper _databaseHelper;
        private readonly IdentityHelper _identityHelper;

        public AccountHistoryRepositoryTests()
        {
            _configurationHelper = new ConfigurationHelper();
            _serviceProviderHelper = new ServiceProviderHelper(_configurationHelper.Configuration);
            _databaseHelper = new DatabaseHelper(_configurationHelper.Configuration);
            _identityHelper = new IdentityHelper("AccountHistoryRepoTest");
            _identityHelper.AddTestUser(_databaseHelper.Context);
        }

        [Fact]
        public async Task LogUserActionToDatabaseAsync_UserIsNull_ThrowArgumentNullException()
        {
            try
            {
                var repo = new AccountHistoryRepository();

                await repo.LogUserActionToDatabaseAsync(null, UserActionType.None, string.Empty);

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
        public async Task LogUserActionToDatabaseAsync_UserIsValid_ActionIsLoggedToDatabase()
        {
            var originalCount = 0;
            using (var unitOfWork = _serviceProviderHelper.ServiceProvider.GetService<IUnitOfWork>())
            {
                originalCount = unitOfWork.AccountHistoryRepository.GetAll().ToList().Count;

                await unitOfWork.AccountHistoryRepository.LogUserActionToDatabaseAsync(_identityHelper.TestUser.Id, UserActionType.None, string.Empty);
                unitOfWork.Complete();
            }

            using (var unitOfWork = _serviceProviderHelper.ServiceProvider.GetService<IUnitOfWork>())
            {
                var newCount = unitOfWork.AccountHistoryRepository.GetAll().ToList().Count;
                Assert.Equal(originalCount + 1, newCount);

                var records = unitOfWork.AccountHistoryRepository.Find(ah => ah.UserId == _identityHelper.TestUser.Id && ah.ActionType == UserActionType.None.ToString());
                Assert.Single(records);
            }

            using (var unitOfWork = _serviceProviderHelper.ServiceProvider.GetService<IUnitOfWork>())
            {
                var record = await unitOfWork.AccountHistoryRepository.SingleOrDefaultAsync(ah => ah.UserId == _identityHelper.TestUser.Id && ah.ActionType == UserActionType.None.ToString());
                unitOfWork.AccountHistoryRepository.Remove(record);
                unitOfWork.Complete();
            }
        }

        public void Dispose()
        {
            _identityHelper.RemoveTestUser(_databaseHelper.Context);
            _databaseHelper.Dispose();
        }
    }
}