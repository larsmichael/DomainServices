namespace DomainServices.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using Abstractions;

    /// <summary>
    ///     In-memory implementation of a discrete repository. To be used in for example unit tests.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TEntityId">The type of the entity identifier.</typeparam>
    public class FakeDiscreteRepository<TEntity, TEntityId> : IRepository<TEntity, TEntityId>, IDiscreteRepository<TEntity, TEntityId>
        where TEntityId : notnull
        where TEntity : IEntity<TEntityId>, ICloneable
    {
        protected Dictionary<TEntityId, TEntity> _entities = new();

        /// <summary>
        ///     Initializes a new instance of the <see cref="FakeDiscreteRepository{TEntity, TEntityId}" /> class.
        /// </summary>
        /// <param name="entities">A collection of entities for priming the repository.</param>
        public FakeDiscreteRepository(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                _entities.Add(entity.Id, entity);
            }
        }

        /// <summary>
        /// Counts the number of entities.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>System.Int32.</returns>
        public int Count(ClaimsPrincipal? user = null)
        {
            return GetAll().Count();
        }

        /// <summary>
        ///     Determines whether an entity with the specified identifier exists.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="user">The user.</param>
        /// <returns><c>true</c> if entity with the specified identifier exists, <c>false</c> otherwise.</returns>
        public bool Contains(TEntityId id, ClaimsPrincipal? user = null)
        {
            return _entities.ContainsKey(id);
        }

        /// <summary>
        ///     Gets all entities.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>IEnumerable&lt;TEntity&gt;.</returns>
        public IEnumerable<TEntity> GetAll(ClaimsPrincipal? user = null)
        {
            foreach (var entity in _entities.Values)
            {
                yield return (TEntity)entity.Clone();
            }
        }

        /// <summary>
        ///     Gets all entity identifiers.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>IEnumerable&lt;TEntityId&gt;.</returns>
        public IEnumerable<TEntityId> GetIds(ClaimsPrincipal? user = null)
        {
            return _entities.Values.Select(e => e.Id).ToArray();
        }

        /// <summary>
        ///     Gets the entity with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="user">The user.</param>
        /// <returns>Maybe&lt;TEntity&gt;.</returns>
        public Maybe<TEntity> Get(TEntityId id, ClaimsPrincipal? user = null)
        {
            _entities.TryGetValue(id, out var entity);
            return entity == null || entity.Equals(default(TEntity)) ? Maybe.Empty<TEntity>() : ((TEntity)entity.Clone()).ToMaybe();
        }
    }
}