using Database.Core.Entities;
using System.Threading.Tasks;

namespace Database.Repositories.Interfaces
{
    public interface IAttachmentRepository : IRepository<Attachment>
    {
        public Task<(byte[] Data, string Extension)?> GetProfilePictureAsync(string userId);

        public Task UploadProfilePictureAsync(string userId, string filename, byte[] fileData);

        public Task<bool> DeleteProfilePictureAsync(string userId);
    }
}
