using Hungabor01Website.Database.Configuration;
using Hungabor01Website.Database.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Hungabor01Website.Database
{
  /// <summary>
  /// The database component
  /// </summary>
  public class AppDbContext : IdentityDbContext<ApplicationUser>
  {
    /// <summary>
    /// Initializes the database component with the given connection options
    /// </summary>
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Attachments table
    /// </summary>
    public DbSet<Attachment> Attachments { get; set; }

    /// <summary>
    /// AccountHistories table
    /// </summary>
    public DbSet<AccountHistory> AccountHistories { get; set; }

    /// <summary>
    /// Add the configuration files of the tables here
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      modelBuilder.ApplyConfiguration(new AttachmentConfiguration());
      modelBuilder.ApplyConfiguration(new AccountHistoryConfiguration());
      modelBuilder.ApplyConfiguration(new ApplicationUserConfiguration());
    }
  }
}
