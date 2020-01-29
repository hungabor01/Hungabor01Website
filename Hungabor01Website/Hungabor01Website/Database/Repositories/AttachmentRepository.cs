using Hungabor01Website.Database.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.IO;
using Hungabor01Website.Utilities;

namespace Hungabor01Website.Database.Repositories
{
  /// <summary>
  /// The repository class for the custom methods of Attachments table
  /// </summary>
  public class AttachmentRepository : Repository<Attachment>, IAttachmentRepository
  {
    public (byte[] Data, string Extension)? GetProfilePicture(ApplicationUser user)
    {
      user.ThrowExceptionIfNull(nameof(user));

      var attachment = SingleOrDefault(
        x => x.User.Id == user.Id && x.Type == AttachmentType.ProfilePicture);

      if (attachment != null)
      {
        return (attachment.Data, attachment.Extension);
      }

      return null;
    }

    public void ChangeProfilePicture(ApplicationUser user, IFormFile file)
    {
      user.ThrowExceptionIfNull(nameof(user));
      file.ThrowExceptionIfNull(nameof(file));

      var attachment = SingleOrDefault(
        x => x.User.Id == user.Id && x.Type == AttachmentType.ProfilePicture);

      if (attachment == null)
      {
        attachment = new Attachment()
        {
          UserId = user.Id,
          Type = AttachmentType.ProfilePicture,
          Filename = Path.GetFileNameWithoutExtension(file.FileName),
          Extension = Path.GetExtension(file.FileName),
          Data = ConvertFileToBytes(file)
        };

        Add(attachment);
      }
      else
      {
        attachment.Filename = Path.GetFileNameWithoutExtension(file.FileName);
        attachment.Extension = Path.GetExtension(file.FileName);
        attachment.Data = ConvertFileToBytes(file);
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

      var attachment = SingleOrDefault(
        x => x.User.Id == user.Id && x.Type == AttachmentType.ProfilePicture);

      if (attachment != null)
      {
        Remove(attachment);
        return true;
      } 

      return false;
    }
  }
}
