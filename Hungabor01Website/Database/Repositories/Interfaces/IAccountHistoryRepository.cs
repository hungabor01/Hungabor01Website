using Common.Enums;
using Database.Core.Entities;
using System.Threading.Tasks;

namespace Database.Repositories.Interfaces
{
    public interface IAccountHistoryRepository : IRepository<AccountHistory>
    {
        public Task LogUserActionToDatabaseAsync(string userId, UserActionType type, string description);
    }
}
