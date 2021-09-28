namespace DomainServices.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Security.Claims;
    using Abstractions;

    /// <summary>
    ///     In-memory implementation of a discrete and updatable repository. To be used in for example unit tests.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TEntityId">The type of the entity identifier.</typeparam>
    public class FakeRepository<TEntity, TEntityId> : IRepository<TEntity, TEntityId>, IDiscreteRepository<TEntity, TEntityId>, IUpdatableRepository<TEntity, TEntityId> where TEntity : IEntity<TEntityId>, ICloneable
    {
        protected readonly Dictionary<TEntityId, TEntity> _entities;

        /// <summary>
        ///     Initializes a new instance of the <see cref="FakeRepository{TEntity, TEntityId}" /> class.
        /// </summary>
        public FakeRepository()
        {
            _entities = new Dictionary<TEntityId, TEntity>();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="FakeRepository{TEntity, TEntityId}" /> class.
        /// </summary>
        /// <param name="entities">A collection of entities for priming the repository.</param>
        public FakeRepository(IEnumerable<TEntity> entities)
            : this()
        {
            foreach (var entity in entities)
            {
                _entities.Add(entity.Id, entity);
            }
        }

        /// <summary>
        ///     Gets the entities.
        /// </summary>
        /// <value>The entities.</value>
        protected Dictionary<TEntityId, TEntity> Entities => _entities;

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

        /// <summary>
        ///     Adds the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="user">The user.</param>
        public void Add(TEntity entity, ClaimsPrincipal? user = null)
        {
            _entities.Add(entity.Id, entity);
        }

        /// <summary>
        ///     Removes the entity with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="user">The user.</param>
        public void Remove(TEntityId id, ClaimsPrincipal? user = null)
        {
            _entities.Remove(id);
        }

        /// <summary>
        ///     Updates the specified entity.
        /// </summary>
        /// <param name="updatedEntity">The updated entity.</param>
        /// <param name="user">The user.</param>
        public void Update(TEntity updatedEntity, ClaimsPrincipal? user = null)
        {
            if (!_entities.ContainsKey(updatedEntity.Id))
            {
                throw new KeyNotFoundException($"'{typeof(TEntity)}' with id '{updatedEntity.Id}' was not found.");
            }

            if (updatedEntity is ITraceableEntity<TEntityId> entity)
            {
                entity.Added = ((ITraceableEntity<TEntityId>)_entities[entity.Id]).Added;
            }

            _entities[updatedEntity.Id] = updatedEntity;
        }

        /// <summary>
        ///     Gets entities matching the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>IEnumerable&lt;TEntity&gt;.</returns>
        public IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> predicate)
        {
            foreach (var entity in _entities.Values.AsQueryable().Where(predicate))
            {
                yield return (TEntity)entity.Clone();
            }
        }

        /// <summary>
        ///     Gets entities matching the specified query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>IEnumerable&lt;TEntity&gt;.</returns>
        public IEnumerable<TEntity> Get(IEnumerable<QueryCondition> query)
        {
            var predicate = ExpressionBuilder.Build<TEntity>(query);
            foreach (var entity in Get(predicate))
            {
                yield return (TEntity)entity.Clone();
            }
        }

        /// <summary>
        ///     Removes all entities matching the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        public void Remove(Expression<Func<TEntity, bool>> predicate)
        {
            var toRemove = Get(predicate).ToArray();
            foreach (var entity in toRemove)
            {
                _entities.Remove(entity.Id);
            }
        }
    }
}