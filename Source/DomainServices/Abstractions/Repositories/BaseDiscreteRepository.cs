namespace DomainServices.Abstractions
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;

    /// <summary>
    ///     Abstract base class for a discrete repository.
    ///     A discrete repository is a repository with a finite number of (discrete) entities - each identified by a unique ID.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TEntityId">The type of the entity identifier.</typeparam>
    public abstract class BaseDiscreteRepository<TEntity, TEntityId> : IRepository<TEntity, TEntityId>, IDiscreteRepository<TEntity, TEntityId>
        where TEntityId : notnull
        where TEntity : IEntity<TEntityId>
    {
        /// <summary>
        /// Counts the number of entities.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>System.Int32.</returns>
        public virtual int Count(ClaimsPrincipal? user = null)
        {
            return GetAll(user).Count();
        }

        /// <summary>
        ///     Determines whether an entity with the specified identifier exists.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="user">The user.</param>
        /// <returns><c>true</c> if entity with the specified identifier exists, <c>false</c> otherwise.</returns>
        public virtual bool Contains(TEntityId id, ClaimsPrincipal? user = null)
        {
            return Get(id, user).HasValue;
        }

        /// <summary>
        ///     Gets all entities.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>IEnumerable&lt;TEntity&gt;.</returns>
        public abstract IEnumerable<TEntity> GetAll(ClaimsPrincipal? user = null);

        /// <summary>
        ///     Gets all entity identifiers.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>IEnumerable&lt;TEntityId&gt;.</returns>
        public virtual IEnumerable<TEntityId> GetIds(ClaimsPrincipal? user = null)
        {
            return GetAll(user).Select(e => e.Id).ToArray();
        }

        /// <summary>
        ///     Gets the entity with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="user">The user.</param>
        /// <returns>Maybe&lt;TEntity&gt;.</returns>
        public virtual Maybe<TEntity> Get(TEntityId id, ClaimsPrincipal? user = null)
        {
            var entity = GetAll(user).FirstOrDefault(e => e.Id.Equals(id));
            return entity == null || entity.Equals(default(TEntity)) ? Maybe.Empty<TEntity>() : entity.ToMaybe();
        }
    }
}