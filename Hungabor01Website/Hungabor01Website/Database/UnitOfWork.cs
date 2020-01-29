using Hungabor01Website.Database.Repositories;

namespace Hungabor01Website.Database
{
  /// <summary>
  /// The main component of work on the database
  /// It represents a single work unit on the database
  /// Add the repository interfaces here
  /// </summary>
  public class UnitOfWork : IUnitOfWork
  {
    private readonly WebsiteDbContext context;

    public IAttachmentRepository AttachmentRepository { get; }

    public IAccountHistoryRepository AccountHistoryRepository { get; }

    public UnitOfWork(
      WebsiteDbContext context,
      IAttachmentRepository attachments,
      IAccountHistoryRepository accountHistories)
    {
      this.context = context;
      AttachmentRepository = attachments;
      AccountHistoryRepository = accountHistories;

      //Must initailize the repositories with the context to work
      AttachmentRepository.InitRepository(context);
      AccountHistoryRepository.InitRepository(context);
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
      //No need for the proper dispose pattern,
      //since the DbContext base class takes care of it
      context.Dispose();
    }
  }
}
