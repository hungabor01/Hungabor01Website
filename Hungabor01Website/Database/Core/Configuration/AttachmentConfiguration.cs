using Database.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Core.Configuration
{
    public class AttachmentConfiguration : IEntityTypeConfiguration<Attachment>
    {
        public void Configure(EntityTypeBuilder<Attachment> builder)
        {
            builder.Property(a => a.Filename)
                .IsRequired();

            builder.Property(a => a.Extension)
                .IsRequired();

            builder.Property(a => a.Data)
                .IsRequired();

            builder.HasOne(a => a.User)
                .WithMany(user => user.Attachments)
                .HasForeignKey(a => a.UserId);
        }
    }
}
