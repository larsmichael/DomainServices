namespace DomainServices.Abstractions
{
    using System.Collections.Generic;
    using System.Security.Claims;

    /// <summary>
    ///     Interface IDiscreteRepository
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TEntityId">The type of the entity identifier.</typeparam>
    public interface IDiscreteRepository<out TEntity, TEntityId>
        where TEntityId : notnull
        where TEntity : IEntity<TEntityId>
    {
        /// <summary>
        /// Counts the number of entities.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>System.Int32.</returns>
        int Count(ClaimsPrincipal? user = null);

        /// <summary>
        ///     Determines whether an entity with the specified identifier exists.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="user">The user.</param>
        /// <returns><c>true</c> if entity with the specified identifier exists, <c>false</c> otherwise.</returns>
        bool Contains(TEntityId id, ClaimsPrincipal? user = null);

        /// <summary>
        ///     Gets all entities.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>IEnumerable&lt;TEntity&gt;.</returns>
        IEnumerable<TEntity> GetAll(ClaimsPrincipal? user = null);

        /// <summary>
        ///     Gets all entity identifiers.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>IEnumerable&lt;TEntityId&gt;.</returns>
        IEnumerable<TEntityId> GetIds(ClaimsPrincipal? user = null);
    }
}