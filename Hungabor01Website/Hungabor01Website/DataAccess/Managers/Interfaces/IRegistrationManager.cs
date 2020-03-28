using Hungabor01Website.BusinessLogic.Enums;
using Hungabor01Website.Database.Core;
using System.Threading.Tasks;

namespace Hungabor01Website.DataAccess.Managers.Interfaces
{
    public interface IRegistrationManager
    {
        public Task LogUserActionToDatabaseAsync(ApplicationUser user, UserActionType type, string description = null);

        public Task<bool> SendEmailAsync(string emailAddress, string subject, string emailBody);
    }
}