using Hungabor01Website.Database.Repositories;
using System;

namespace Hungabor01Website.Database
{  
  /// <summary>
  /// The main database connection component. It represents a connection to the database.
  /// Add the repository interfaces here
  /// </summary>
  public interface IUnitOfWork : IDisposable
  {
    public ITestEntityRepository TestEntities { get; }

    /// <summary>
    /// Commits the changes to the database
    /// </summary>
    /// <returns>Number of the modified entries</returns>
    int Complete();
  }
}
