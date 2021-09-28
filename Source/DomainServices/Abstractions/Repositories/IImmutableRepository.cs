namespace DomainServices.Abstractions
{
    using System;
    using System.Security.Claims;

    /// <summary>
    ///     Interface for an immutable repository.
    ///     Entities can be added and removed - but not updated.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TEntityId">The type of the entity identifier.</typeparam>
    public interface IImmutableRepository<in TEntity, in TEntityId>
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
    }

    /// <summary>
    ///     Immutable repository.
    ///     Entities can be added and removed - but not updated.
    ///     Entity ID is of type Guid
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public interface IImmutableRepository<in TEntity> : IImmutableRepository<TEntity, Guid> where TEntity : IEntity<Guid>
    {
    }
}