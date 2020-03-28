using Hungabor01Website.Database.Core;
using Hungabor01Website.Database.Core.Entities;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Hungabor01Website.Database.Repositories.Interfaces
{
    public interface IAttachmentRepository : IRepository<Attachment>
    {
        public Task<(byte[] Data, string Extension)?> GetProfilePictureAsync(ApplicationUser user);

        public Task UploadProfilePictureAsync(ApplicationUser user, IFormFile file);

        public Task<bool> DeleteProfilePictureAsync(ApplicationUser user);
    }
}
