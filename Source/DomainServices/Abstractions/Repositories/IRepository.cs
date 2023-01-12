namespace DomainServices.Abstractions;

using System.Security.Claims;

/// <summary>
///     Interface IRepository
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
/// <typeparam name="TEntityId">The type of the entity identifier.</typeparam>
public interface IRepository<TEntity, in TEntityId>
    where TEntityId : notnull
    where TEntity : IEntity<TEntityId>
{
    /// <summary>
    ///     Gets the entity with the specified identifier.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="user">The user.</param>
    /// <returns>Maybe&lt;TEntity&gt;.</returns>
    Maybe<TEntity> Get(TEntityId id, ClaimsPrincipal? user = null);
}