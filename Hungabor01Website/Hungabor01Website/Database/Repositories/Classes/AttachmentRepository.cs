using Hungabor01Website.Database.Core.Entities;
using Microsoft.AspNetCore.Http;
using System.IO;
using Hungabor01Website.Database.Repositories.Interfaces;
using System.Threading.Tasks;
using Hungabor01Website.BusinessLogic.Enums;
using Hungabor01Website.Database.Core;
using Hungabor01Website.BusinessLogic;

namespace Hungabor01Website.Database.Repositories.Classes
{
    public class AttachmentRepository : Repository<Attachment>, IAttachmentRepository
    {
        public async Task<(byte[] Data, string Extension)?> GetProfilePictureAsync(ApplicationUser user)
        {
            user.ThrowExceptionIfNull(nameof(user));

            var profilePicture = await GetProfilePictureForUser(user.Id);

            if (profilePicture == null)
            {
                return null;
            }

            return (profilePicture.Data, profilePicture.Extension);            
        }

        public async Task UploadProfilePictureAsync(ApplicationUser user, IFormFile file)
        {
            user.ThrowExceptionIfNull(nameof(user));
            file.ThrowExceptionIfNull(nameof(file));

            var profilePicture = await GetProfilePictureForUser(user.Id);

            if (profilePicture == null)
            {
                profilePicture = new Attachment()
                {
                    UserId = user.Id,
                    Type = AttachmentType.ProfilePicture.ToString(),
                    Filename = Path.GetFileNameWithoutExtension(file.FileName),
                    Extension = Path.GetExtension(file.FileName),
                    Data = ConvertFileToBytes(file)
                };

                await AddAsync(profilePicture);
            }
            else
            {
                profilePicture.Filename = Path.GetFileNameWithoutExtension(file.FileName);
                profilePicture.Extension = Path.GetExtension(file.FileName);
                profilePicture.Data = ConvertFileToBytes(file);
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

        public async Task<bool> DeleteProfilePictureAsync(ApplicationUser user)
        {
            user.ThrowExceptionIfNull(nameof(user));

            var profilePicture = await GetProfilePictureForUser(user.Id);

            if (profilePicture != null)
            {
                Remove(profilePicture);
                return true;
            } 

            return false;
        }

        private async Task<Attachment> GetProfilePictureForUser(string userId)
        {
            return await SingleOrDefaultAsync(a => a.User.Id == userId && a.Type == AttachmentType.ProfilePicture.ToString());
        }
    }
}
