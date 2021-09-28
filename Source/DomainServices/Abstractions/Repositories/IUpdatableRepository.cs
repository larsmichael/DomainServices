namespace DomainServices.Abstractions
{
    using System.Security.Claims;

    /// <summary>
    ///     Interface IUpdatableRepository
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TEntityId">The type of the entity identifier.</typeparam>
    public interface IUpdatableRepository<in TEntity, in TEntityId>
        where TEntityId : notnull
        where TEntity : IEntity<TEntityId>
    {
        /// <summary>
        ///     Adds the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="user">The user.</param>
        void Add(TEntity entity, ClaimsPrincipal? user = null);

        /// <summary>
        ///     Removes the entity with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="user">The user.</param>
        void Remove(TEntityId id, ClaimsPrincipal? user = null);

        /// <summary>
        ///     Updates the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="user">The user.</param>
        void Update(TEntity entity, ClaimsPrincipal? user = null);
    }
}