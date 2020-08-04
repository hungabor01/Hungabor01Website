using Common.Enums;
using Database.Core;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace DataAccess.Managers.Interfaces
{
    public interface IAccountManager
    {
        public Task LogUserActionToDatabaseAsync(ApplicationUser user, UserActionType actionType, string description = null);
        
        public Task UploadProfilePictureAsync(ApplicationUser user, IFormFile file);

        public Task<(byte[] Data, string Extension)?> GetProfilePictureAsync(ApplicationUser user);

        public Task<bool> DeleteProfilePictureAsync(ApplicationUser user);
    }
}
