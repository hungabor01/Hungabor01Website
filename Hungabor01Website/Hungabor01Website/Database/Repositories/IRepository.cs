using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Hungabor01Website.Database.Repositories
{
  /// <summary>
  /// Generic interface for the repository components, the tables
  /// </summary>
  /// <typeparam name="TEntity">The table of the repository</typeparam>
  public interface IRepository<TEntity> where TEntity : class
  {
    /// <summary>
    /// Initializes the context of the repository
    /// </summary>
    /// <param name="context">The database actual context</param>
    void InitRepository(DbContext context);

    /// <summary>
    /// Retrieves one element by the given id from the table
    /// </summary>
    /// <param name="id">The Id column of the table</param>
    /// <returns>The matching row object</returns>
    TEntity Get(int id);

    /// <summary>
    /// Retrieves all element of the table
    /// </summary>
    /// <returns>All row</returns>
    IEnumerable<TEntity> GetAll();

    /// <summary>
    /// Finds elements based on the given expression
    /// </summary>
    /// <param name="predicate">The expression to find the elements</param>
    /// <returns>The matching elements</returns>
    IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    /// Finds one element based on the given expression
    /// </summary>
    /// <param name="predicate">The expression to find the element</param>
    /// <returns>The matching element or the default value</returns>
    TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    /// Adds one element to the table
    /// </summary>
    /// <param name="entity">The element to add</param>
    void Add(TEntity entity);

    /// <summary>
    /// Adds more elements to the table
    /// </summary>
    /// <param name="entity">The elements to add</param>
    void AddRange(IEnumerable<TEntity> Entities);

    /// <summary>
    /// Removes one element from the table
    /// </summary>
    /// <param name="entity">The element to remove</param>
    void Remove(TEntity entity);

    /// <summary>
    /// Removes more element from the table
    /// </summary>
    /// <param name="entity">The elements to remove</param>
    void RemoveRange(IEnumerable<TEntity> Entities);
  }
}