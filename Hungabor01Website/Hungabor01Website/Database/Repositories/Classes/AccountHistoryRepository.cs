using Hungabor01Website.Database.Core.Entities;
using Hungabor01Website.Database.Repositories.Interfaces;
using Hungabor01Website.BusinessLogic.Enums;
using System;
using System.Threading.Tasks;
using Hungabor01Website.Database.Core;
using Hungabor01Website.BusinessLogic;

namespace Hungabor01Website.Database.Repositories.Classes
{
    public class AccountHistoryRepository : Repository<AccountHistory>, IAccountHistoryRepository
    {
        public async Task LogUserActionToDatabaseAsync(ApplicationUser user, UserActionType type, string description)
        {
            user.ThrowExceptionIfNull(nameof(user));
            
            var action = new AccountHistory
            {
                UserId = user.Id,
                DateTime = DateTime.Now,
                Type = type.ToString(),
                Description = description
            };

            await AddAsync(action);
        }
    }
}
