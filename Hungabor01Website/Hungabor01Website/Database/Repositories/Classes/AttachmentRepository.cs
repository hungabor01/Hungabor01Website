using Hungabor01Website.Database.Entities;
using Microsoft.AspNetCore.Http;
using System.IO;
using Hungabor01Website.Database.Repositories.Interfaces;
using Hungabor01Website.Utilities.Classes;

namespace Hungabor01Website.Database.Repositories.Classes
{
  /// <summary>
  /// The repository class for the custom methods of Attachments table
  /// </summary>
  public class AttachmentRepository : Repository<Attachment>, IAttachmentRepository
  {
    public (byte[] Data, string Extension)? GetProfilePicture(ApplicationUser user)
    {
      user.ThrowExceptionIfNull(nameof(user));

      var profilePicture = SingleOrDefault(
        x => x.User.Id == user.Id && x.Type == AttachmentType.ProfilePicture);

      if (profilePicture != null)
      {
        return (profilePicture.Data, profilePicture.Extension);
      }

      return null;
    }

    public void ChangeProfilePicture(ApplicationUser user, IFormFile file)
    {
      user.ThrowExceptionIfNull(nameof(user));
      file.ThrowExceptionIfNull(nameof(file));

      var profilePicture = SingleOrDefault(
        x => x.User.Id == user.Id && x.Type == AttachmentType.ProfilePicture);

      if (profilePicture == null)
      {
        profilePicture = new Attachment()
        {
          UserId = user.Id,
          Type = AttachmentType.ProfilePicture,
          Filename = Path.GetFileNameWithoutExtension(file.FileName),
          Extension = Path.GetExtension(file.FileName),
          Data = ConvertFileToBytes(file)
        };

        Add(profilePicture);
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
      file.ThrowExceptionIfNull(nameof(file));
      
      byte[] fileBytes = null;

      using (var ms = new MemoryStream())
      {
        file.CopyTo(ms);
        fileBytes = ms.ToArray();
      }

      return fileBytes;
    }

    public bool DeleteProfilePicture(ApplicationUser user)
    {
      user.ThrowExceptionIfNull(nameof(user));

      var profilePicture = SingleOrDefault(
        x => x.User.Id == user.Id && x.Type == AttachmentType.ProfilePicture);

      if (profilePicture != null)
      {
        Remove(profilePicture);
        return true;
      } 

      return false;
    }
  }
}
