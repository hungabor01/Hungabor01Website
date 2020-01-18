using Hungabor01Website.Database.Repositories;

namespace Hungabor01Website.Database
{
  /// <summary>
  /// The main component of the database connection. It represents a connection to the database. Add the repository interfaces here
  /// </summary>
  public class UnitOfWork : IUnitOfWork
  {
    private readonly WebsiteDbContext context;

    public IAttachmentRepository Attachments { get; }

    public UnitOfWork(
      WebsiteDbContext context,
      IAttachmentRepository testEntities)
    {
      this.context = context;
      Attachments = testEntities;

      //Must initailize the repositories with the context to work
      Attachments.InitRepository(context);
    }

    public int Complete()
    {
      return context.SaveChanges();
    }

    /// <summary>
    /// Releases the context object
    /// </summary>
    public void Dispose()
    {
      context.Dispose();
    }
  }
}
