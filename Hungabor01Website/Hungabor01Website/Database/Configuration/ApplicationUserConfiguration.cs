using Hungabor01Website.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hungabor01Website.Database.Configuration
{
  /// <summary>
  /// Configuration for the user table
  /// </summary>
  public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
  {
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
    }
  }
}
