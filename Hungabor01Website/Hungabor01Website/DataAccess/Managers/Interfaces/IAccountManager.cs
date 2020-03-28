using Hungabor01Website.Database.Core;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Hungabor01Website.DataAccess.Managers.Interfaces
{
    public interface IAccountManager : IRegistrationManager
    {
        public Task UploadProfilePictureAsync(ApplicationUser user, IFormFile file);

        public Task<(byte[] Data, string Extension)?> GetProfilePictureAsync(ApplicationUser user);

        public Task<bool> DeleteProfilePictureAsync(ApplicationUser user);
    }
}
