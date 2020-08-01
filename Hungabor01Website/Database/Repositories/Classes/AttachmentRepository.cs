using Database.Core.Entities;
using System.IO;
using Database.Repositories.Interfaces;
using System.Threading.Tasks;
using Common;
using Common.Enums;

namespace Database.Repositories.Classes
{
    public class AttachmentRepository : Repository<Attachment>, IAttachmentRepository
    {
        public async Task<(byte[] Data, string Extension)?> GetProfilePictureAsync(string userId)
        {
            userId.ThrowExceptionIfNullOrWhiteSpace(nameof(userId));

            var profilePicture = await GetProfilePictureForUser(userId);

            if (profilePicture == null)
            {
                return null;
            }

            return (profilePicture.Data, profilePicture.Extension);            
        }

        public async Task UploadProfilePictureAsync(string userId, string filename, byte[] fileData)
        {
            userId.ThrowExceptionIfNullOrWhiteSpace(nameof(userId));
            filename.ThrowExceptionIfNullOrWhiteSpace(nameof(filename));
            fileData.ThrowExceptionIfNull(nameof(fileData));

            var profilePicture = await GetProfilePictureForUser(userId);

            if (profilePicture == null)
            {
                profilePicture = new Attachment()
                {
                    UserId = userId,
                    Type = AttachmentType.ProfilePicture.ToString(),
                    Filename = Path.GetFileNameWithoutExtension(filename),
                    Extension = Path.GetExtension(filename),
                    Data = fileData
                };

                await AddAsync(profilePicture);
            }
            else
            {
                profilePicture.Filename = Path.GetFileNameWithoutExtension(filename);
                profilePicture.Extension = Path.GetExtension(filename);
                profilePicture.Data = fileData;
            }
        }

        public async Task<bool> DeleteProfilePictureAsync(string userId)
        {
            userId.ThrowExceptionIfNullOrWhiteSpace(nameof(userId));

            var profilePicture = await GetProfilePictureForUser(userId);

            if (profilePicture == null)
            {
                return false;
            }

            Remove(profilePicture);
            return true;
        }

        private async Task<Attachment> GetProfilePictureForUser(string userId)
        {
            return await SingleOrDefaultAsync(a => a.User.Id == userId && a.Type == AttachmentType.ProfilePicture.ToString());
        }
    }
}
