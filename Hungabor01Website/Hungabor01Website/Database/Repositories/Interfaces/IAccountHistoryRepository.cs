using Hungabor01Website.Database.Core.Entities;
using Hungabor01Website.BusinessLogic.Enums;
using System.Threading.Tasks;
using Hungabor01Website.Database.Core;

namespace Hungabor01Website.Database.Repositories.Interfaces
{
    public interface IAccountHistoryRepository : IRepository<AccountHistory>
    {
        public Task LogUserActionToDatabaseAsync(ApplicationUser user, UserActionType type, string description);
    }
}
