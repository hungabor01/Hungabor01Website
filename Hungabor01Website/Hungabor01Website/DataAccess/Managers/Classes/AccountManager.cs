using Hungabor01Website.DataAccess.Managers.Interfaces;
using Hungabor01Website.Database.Core;
using Hungabor01Website.Database.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Hungabor01Website.DataAccess.Managers.Classes
{
    public class AccountManager : RegistrationManager, IAccountManager
    {
        public AccountManager(IServiceProvider serviceProvider) 
            : base(serviceProvider)
        {
        }

        public async Task UploadProfilePictureAsync(ApplicationUser user, IFormFile file)
        {
            using (var unitOfWork = ServiceProvider.GetService<IUnitOfWork>())
            {
                await unitOfWork.AttachmentRepository.UploadProfilePictureAsync(user, file);
                await unitOfWork.CompleteAsync();
            }
        }

        public async Task<(byte[] Data, string Extension)?> GetProfilePictureAsync(ApplicationUser user)
        {
            using (var unitOfWork = ServiceProvider.GetService<IUnitOfWork>())
            {
                return await unitOfWork.AttachmentRepository.GetProfilePictureAsync(user);
            }
        }

        public async Task<bool> DeleteProfilePictureAsync(ApplicationUser user)
        {
            var result = false;

            using (var unitOfWork = ServiceProvider.GetService<IUnitOfWork>())
            {
                result = await unitOfWork.AttachmentRepository.DeleteProfilePictureAsync(user);
                await unitOfWork.CompleteAsync();
            }

            return result;            
        }
    }
}
