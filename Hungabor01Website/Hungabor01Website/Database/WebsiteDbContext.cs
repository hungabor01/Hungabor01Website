using Hungabor01Website.Database.Configuration;
using Hungabor01Website.Database.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Hungabor01Website.Database
{
  /// <summary>
  /// The database object
  /// </summary>
  public class WebsiteDbContext : IdentityDbContext
  {
    /// <summary>
    /// Initializes the database component with the given connection options
    /// </summary>
    public WebsiteDbContext(DbContextOptions<WebsiteDbContext> options) : base(options)
    {

    }

    /// <summary>
    /// Attachments table
    /// </summary>
    public DbSet<Attachment> Attachments { get; set; }

    /// <summary>
    /// Add the configuration files of the tables here
    /// </summary>
    /// <param name="modelBuilder"></param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      modelBuilder.ApplyConfiguration(new AttachmentConfiguration());
    }
  }
}
