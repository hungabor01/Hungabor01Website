using Hungabor01Website.Database.Repositories;

namespace Hungabor01Website.Database
{
  /// <summary>
  /// The main component of the database connection. It represents a connection to the database. Add the repository interfaces here.
  /// </summary>
  public class UnitOfWork : IUnitOfWork
  {
    private readonly WebsiteDbContext context;

    public ITestEntityRepository TestEntities { get; }

    public UnitOfWork(
      WebsiteDbContext context,
      ITestEntityRepository exercises)
    {
      this.context = context;
      TestEntities = exercises;

      //Must initailize with the context to work.
      TestEntities.InitRepository(context);
    }

    public int Complete()
    {
      return context.SaveChanges();
    }

    /// <summary>
    /// Releases the context object.
    /// </summary>
    public void Dispose()
    {
      context.Dispose();
    }
  }
}
