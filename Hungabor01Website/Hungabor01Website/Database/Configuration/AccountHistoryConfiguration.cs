using Hungabor01Website.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hungabor01Website.Database.Configuration
{
  /// <summary>
  /// Configuration for the AccountHistories table
  /// </summary>
  public class AccountHistoryConfiguration : IEntityTypeConfiguration<AccountHistory>
  {
    public void Configure(EntityTypeBuilder<AccountHistory> builder)
    {
      builder.HasOne(ah => ah.User)
        .WithMany(user => user.AccountHistories)
        .HasForeignKey(ah => ah.UserId);      
    }
  }
}
