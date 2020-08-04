using Common.Enums;
using Database.Core;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace BusinessLogic.ControllerManagers.Interfaces
{
    public interface IAccountControllersManager
    {
        public Task LogUserActionToDatabaseAsync(ApplicationUser user, UserActionType actionType, string description = null);

        public Task<bool> SendConfirmationEmailAsync(ApplicationUser user, string confirmationLink);

        public Task<bool> SendForgotPasswordEmailAsync(ApplicationUser user, string passwordResetLink);

        public Task UploadProfilePictureAsync(ApplicationUser user, IFormFile file);

        public Task<(byte[] Data, string Extension)?> GetProfilePictureAsync(ApplicationUser user);

        public Task<bool> DeleteProfilePictureAsync(ApplicationUser user);
    }
}
