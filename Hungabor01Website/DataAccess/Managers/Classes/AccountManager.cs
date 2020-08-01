using Common;
using Common.Enums;
using DataAccess.Managers.Interfaces;
using Database.Core;
using Database.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DataAccess.Managers.Classes
{
    public class AccountManager : IAccountManager
    {
        private readonly IServiceProvider _serviceProvider;

        public AccountManager(IServiceProvider serviceProvider) 
        {
            _serviceProvider = serviceProvider;
        }

        public async Task LogUserActionToDatabaseAsync(ApplicationUser user, UserActionType actionType, string description = null)
        {
            using (var unitOfWork = _serviceProvider.GetService<IUnitOfWork>())
            {
                await unitOfWork.AccountHistoryRepository.LogUserActionToDatabaseAsync(user?.Id, actionType, description);
                await unitOfWork.CompleteAsync();
            }
        }

        public async Task UploadProfilePictureAsync(ApplicationUser user, IFormFile file)
        {
            file.ThrowExceptionIfNull(nameof(file));

            using (var unitOfWork = _serviceProvider.GetService<IUnitOfWork>())
            {
                var fileData = ConvertFileToBytes(file);
                await unitOfWork.AttachmentRepository.UploadProfilePictureAsync(user?.Id, file.FileName, fileData);
                await unitOfWork.CompleteAsync();
            }
        }

        private byte[] ConvertFileToBytes(IFormFile file)
        {
            byte[] fileBytes = null;

            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                fileBytes = ms.ToArray();
            }

            return fileBytes;
        }

        public async Task<(byte[] Data, string Extension)?> GetProfilePictureAsync(ApplicationUser user)
        {
            using (var unitOfWork = _serviceProvider.GetService<IUnitOfWork>())
            {
                return await unitOfWork.AttachmentRepository.GetProfilePictureAsync(user?.Id);
            }
        }

        public async Task<bool> DeleteProfilePictureAsync(ApplicationUser user)
        {
            var result = false;

            using (var unitOfWork = _serviceProvider.GetService<IUnitOfWork>())
            {
                result = await unitOfWork.AttachmentRepository.DeleteProfilePictureAsync(user?.Id);
                await unitOfWork.CompleteAsync();
            }

            return result;            
        }
    }
}
