using BusinessLogic.ControllerManagers.Interfaces;
using BusinessLogic.Services.Interfaces;
using Common.Enums;
using Common.Strings;
using DataAccess.Managers.Interfaces;
using Database.Core;
using Microsoft.AspNetCore.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace BusinessLogic.ControllerManagers.Classes
{
    public class AccountControllersManager : IAccountControllersManager
    {
        private readonly IAccountManager _accountManager;
        private readonly IEmailSender _emailSender;

        public AccountControllersManager(IAccountManager accountManager, IEmailSender emailSender)
        {
            _accountManager = accountManager;
            _emailSender = emailSender;
        }

        public async Task LogUserActionToDatabaseAsync(ApplicationUser user, UserActionType actionType, string description = null)
        {
            await _accountManager.LogUserActionToDatabaseAsync(user, actionType, description);
        }

        public async Task<bool> SendConfirmationEmailAsync(ApplicationUser user, string confirmationLink)
        {     
            var emailBody = string.Format(RegistrationStrings.ConfirmationEmailBody, confirmationLink);
            var subject = string.Format(RegistrationStrings.ConfirmationEmailSubject, Assembly.GetEntryAssembly().GetName().Name);

            return await _emailSender.SendEmailAsync(user.Email, subject, emailBody);
        }

        public async Task<bool> SendForgotPasswordEmailAsync(ApplicationUser user, string passwordResetLink)
        {
            var emailBody = string.Format(ProfileStrings.PasswordResetEmailBody, passwordResetLink);
            var subject = string.Format(ProfileStrings.PasswordResetEmailSubject, Assembly.GetEntryAssembly().GetName().Name);

            return await _emailSender.SendEmailAsync(user.Email, subject, emailBody);
        }

        public async Task UploadProfilePictureAsync(ApplicationUser user, IFormFile file)
        {
            await _accountManager.UploadProfilePictureAsync(user, file);
        }

        public async Task<(byte[] Data, string Extension)?> GetProfilePictureAsync(ApplicationUser user)
        {
            return await _accountManager.GetProfilePictureAsync(user);
        }

        public async Task<bool> DeleteProfilePictureAsync(ApplicationUser user)
        {
            return await _accountManager.DeleteProfilePictureAsync(user);
        }
    }
}
