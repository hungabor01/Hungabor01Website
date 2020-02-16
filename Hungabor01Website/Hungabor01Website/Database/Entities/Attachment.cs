namespace Hungabor01Website.Database.Entities
{
  /// <summary>
  /// Enum for the type of the attachments
  /// </summary>
  public enum AttachmentType
  {
    None = 0,
    ProfilePicture = 1
  }

  /// <summary>
  /// Entity for the Attachments table
  /// </summary>
  public class Attachment
  {
    /// <summary>
    /// Primary key for the table
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Foreign key for the related user
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// Navigation property for the related user
    /// </summary>
    public ApplicationUser User { get; set; }

    /// <summary>
    /// Type of the attachment
    /// </summary>
    public AttachmentType Type { get; set; }

    /// <summary>
    /// Name of the file
    /// </summary>
    public string Filename { get; set; }

    /// <summary>
    /// Extension of the file
    /// </summary>
    public string Extension { get; set; }

    /// <summary>
    /// Data of the file
    /// </summary>
    public byte[] Data { get; set; }
  }
}
