namespace DomainServices.Abstractions
{
    using System.Collections.Generic;
    using System.Security.Claims;

    /// <summary>
    ///     Interface IService
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TEntityId">The type of the entity identifier.</typeparam>
    public interface IService<out TEntity, in TEntityId> where TEntity : IEntity<TEntityId>
    {
        /// <summary>
        ///     Gets the entity with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="user">The user.</param>
        /// <returns>TEntity.</returns>
        TEntity Get(TEntityId id, ClaimsPrincipal user = null);

        /// <summary>
        ///     Gets a list of entities with the specified identifiers.
        /// </summary>
        /// <param name="ids">The identifiers.</param>
        /// <param name="user">The user.</param>
        /// <returns>IEnumerable&lt;TEntity&gt;.</returns>
        IEnumerable<TEntity> Get(IEnumerable<TEntityId> ids, ClaimsPrincipal user = null);
    }
}