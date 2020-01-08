using Hungabor01Website.Database.Configuration;
using Hungabor01Website.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hungabor01Website.Database
{
  /// <summary>
  /// The database object.
  /// </summary>
  public class WebsiteDbContext : DbContext
  {
    /// <summary>
    /// Initializes the database component with the given connection string.
    /// </summary>
    public WebsiteDbContext(DbContextOptions<WebsiteDbContext> options) : base(options)
    {

    }

    /// <summary>
    /// AccountLoginInfo table with the basic credentials of the account.
    /// </summary>
    public DbSet<TestEntity> TestEntities { get; set; }

    /// <summary>
    /// Add the configuration files here.
    /// </summary>
    /// <param name="modelBuilder"></param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);
      modelBuilder.ApplyConfiguration(new TestEntityConfiguration());
    }
  }
}
