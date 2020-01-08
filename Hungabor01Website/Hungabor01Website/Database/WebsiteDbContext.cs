using Hungabor01Website.Database.Configuration;
using Hungabor01Website.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hungabor01Website.Database
{
  /// <summary>
  /// The database object
  /// </summary>
  public class WebsiteDbContext : DbContext
  {
    /// <summary>
    /// Initializes the database component with the given connection options
    /// </summary>
    public WebsiteDbContext(DbContextOptions<WebsiteDbContext> options) : base(options)
    {

    }

    /// <summary>
    /// TestEntity table
    /// </summary>
    public DbSet<TestEntity> TestEntities { get; set; }

    /// <summary>
    /// Add the configuration files of the tables here
    /// </summary>
    /// <param name="modelBuilder"></param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      modelBuilder.ApplyConfiguration(new TestEntityConfiguration());
    }
  }
}
