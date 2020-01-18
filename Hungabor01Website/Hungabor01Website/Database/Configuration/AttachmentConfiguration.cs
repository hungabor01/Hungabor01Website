using Hungabor01Website.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hungabor01Website.Database.Configuration
{
  /// <summary>
  /// Configuration for the Attachment table
  /// </summary>
  public class AttachmentConfiguration : IEntityTypeConfiguration<Attachment>
  {
    public void Configure(EntityTypeBuilder<Attachment> builder)
    {

      builder.Property(x => x.Filename)
        .IsRequired();
      
      builder.Property(x => x.Extension)
        .IsRequired();

      builder.Property(x => x.Data)
        .IsRequired();
    }
  }
}
