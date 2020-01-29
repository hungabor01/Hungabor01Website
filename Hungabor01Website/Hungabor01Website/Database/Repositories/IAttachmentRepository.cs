using Hungabor01Website.Database.Entities;
using Microsoft.AspNetCore.Http;

namespace Hungabor01Website.Database.Repositories
{
  /// <summary>
  /// The repository interface for the custom methods of Attachments table
  /// </summary>
  public interface IAttachmentRepository : IRepository<Attachment>
  {
    /// <summary>
    /// Loads the profile picture of the user
    /// </summary>
    /// <param name="userId">The id of the user</param>
    /// <returns>The data and the extension of the imageas a tuple</returns>
    public (byte[] Data, string Extension)? GetProfilePicture(ApplicationUser user);

    /// <summary>
    /// Changes the profile picture of the user
    /// </summary>
    /// <param name="user">The user, whose image is getting changed</param>
    /// <param name="file">The new image</param>
    public void ChangeProfilePicture(ApplicationUser user, IFormFile file);

    /// <summary>
    /// Deletes the profile picture of the user
    /// </summary>
    /// <param name="userId">The user, whose picture is getting deleted</param>
    /// <returns>Whether it was successfull or not</returns>
    public bool DeleteProfilePicture(ApplicationUser user);
  }
}
