using Common;
using Common.Enums;
using Database.Core.Entities;
using Database.Repositories.Interfaces;
using System;
using System.Threading.Tasks;

namespace Database.Repositories.Classes
{
    public class AccountHistoryRepository : Repository<AccountHistory>, IAccountHistoryRepository
    {
        public async Task LogUserActionToDatabaseAsync(string userId, UserActionType actionType, string description)
        {
            userId.ThrowExceptionIfNullOrWhiteSpace(nameof(userId));
            
            var action = new AccountHistory
            {
                UserId = userId,
                DateTime = DateTime.Now,
                ActionType = actionType.ToString(),
                Description = description
            };

            await AddAsync(action);
        }
    }
}
