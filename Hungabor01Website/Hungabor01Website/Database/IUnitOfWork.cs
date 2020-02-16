using Hungabor01Website.Database.Repositories.Interfaces;
using System;

namespace Hungabor01Website.Database
{  
  /// <summary>
  /// The main database connection component
  /// It represents a single work unit on the database
  /// Add the repository interfaces here
  /// </summary>
  public interface IUnitOfWork : IDisposable
  {
    /// <summary>
    /// Repository of the Attachments table
    /// </summary>
    public IAttachmentRepository AttachmentRepository { get; }

    /// <summary>
    /// Repository of the AccountHistories table
    /// </summary>
    public IAccountHistoryRepository AccountHistoryRepository { get; }

    /// <summary>
    /// Commits the changes to the database
    /// </summary>
    /// <returns>Number of the modified entries</returns>
    public int Complete();
  }
}
