namespace DomainServices.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Security.Claims;
    using Abstractions;
    using ICloneable = Abstractions.ICloneable;

    /// <summary>
    ///     In-memory implementation of a grouped JSON repository. To be used in for example unit tests.
    ///     All entities must belong to a group.
    ///     The grouping is one level only (not hierarchical).
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public class FakeGroupedJsonRepository<TEntity> :
        IRepository<TEntity, string>,
        IDiscreteRepository<TEntity, string>,
        IGroupedRepository<TEntity>,
        IUpdatableRepository<TEntity, string> where TEntity : IGroupedEntity<string>, ICloneable
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="FakeGroupedJsonRepository{TEntity}" /> class.
        /// </summary>
        public FakeGroupedJsonRepository()
        {
            Entities = new Dictionary<string, Dictionary<string, TEntity>>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FakeGroupedJsonRepository{TEntity}"/> class.
        /// </summary>
        /// <param name="entities">A collection of entities for priming the repository.</param>
        public FakeGroupedJsonRepository(IEnumerable<TEntity> entities)
            : this()
        {
            foreach (var entity in entities)
            {
                if (entity is ITraceableEntity<string> e)
                {
                    e.Added = DateTime.UtcNow;
                    e.Updated = null;
                }
                Add(entity);
            }
        }

        private Dictionary<string, Dictionary<string, TEntity>> Entities { get; }

        /// <summary>
        ///     The total number of entities.
        /// </summary>
        /// <param name="user">The user.</param>
        public int Count(ClaimsPrincipal? user = null)
        {
            var count = 0;
            foreach (var group in Entities)
            {
                count += group.Value.Count;
            }

            return count;
        }

        /// <summary>
        ///     Determines whether an entity with the specified fullname identifier exists.
        /// </summary>
        /// <param name="id">The fullname identifier.</param>
        /// <param name="user">The user.</param>
        /// <returns><c>true</c> if entity with the specified fullname identifier exists, <c>false</c> otherwise.</returns>
        public bool Contains(string id, ClaimsPrincipal? user = null)
        {
            var fullname = GetFullName(id);
            return Entities.ContainsKey(fullname.Group!) && Entities[fullname.Group!].ContainsKey(fullname.Name);
        }

        /// <summary>
        ///     Gets all entities.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>IEnumerable&lt;TEntity&gt;.</returns>
        public virtual IEnumerable<TEntity> GetAll(ClaimsPrincipal? user = null)
        {
            foreach (var group in Entities)
            {
                foreach (var entity in group.Value.Values)
                {
                    yield return entity.Clone<TEntity>();
                }
            }
        }

        /// <summary>
        ///     Gets all entity identifiers.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>IEnumerable&lt;string&gt;.</returns>
        public IEnumerable<string> GetIds(ClaimsPrincipal? user = null)
        {
            var ids = new List<string>();
            foreach (var group in Entities)
            {
                foreach (var entity in group.Value.Values)
                {
                    ids.Add(entity.Id);
                }
            }

            return ids;
        }

        /// <summary>
        ///     Determines whether the repository contains the specified group.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <param name="user">The user.</param>
        /// <returns><c>true</c> if the repository contains the specified group; otherwise, <c>false</c>.</returns>
        public bool ContainsGroup(string group, ClaimsPrincipal? user = null)
        {
            return Entities.ContainsKey(group);
        }

        /// <summary>
        ///     Gets entities by group.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <param name="user">The user.</param>
        /// <returns>IEnumerable&lt;TEntity&gt;.</returns>
        public IEnumerable<TEntity> GetByGroup(string group, ClaimsPrincipal? user = null)
        {
            foreach (var entity in Entities[group].Values)
            {
                yield return entity.Clone<TEntity>();
            }
        }

        /// <summary>
        ///     Gets the full names by group.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <param name="user">The user.</param>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        public IEnumerable<string> GetFullNames(string group, ClaimsPrincipal? user = null)
        {
            return Entities[group].Values.Select(e => e.FullName).ToArray();
        }

        /// <summary>
        ///     Gets the full names.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        public IEnumerable<string> GetFullNames(ClaimsPrincipal? user = null)
        {
            var fullNames = new List<string>();
            foreach (var group in Entities)
            {
                foreach (var entity in group.Value.Values)
                {
                    fullNames.Add(entity.FullName);
                }
            }

            return fullNames;
        }

        /// <summary>
        ///     Gets the entity with the specified fullname identifier.
        /// </summary>
        /// <param name="id">The fullname identifier.</param>
        /// <param name="user">The user.</param>
        /// <returns>Maybe&lt;TEntity&gt;.</returns>
        public Maybe<TEntity> Get(string id, ClaimsPrincipal? user = null)
        {
            var fullName = GetFullName(id);
            if (!Entities.Any())
            {
                return Maybe.Empty<TEntity>();
            }

            Entities.TryGetValue(fullName.Group!, out var group);
            if (group is null)
            {
                return Maybe.Empty<TEntity>();
            }

            group.TryGetValue(fullName.Name, out var entity);
            return entity == null || entity.Equals(default(TEntity)) ? Maybe.Empty<TEntity>() : (entity.Clone<TEntity>()).ToMaybe();
        }

        /// <summary>
        ///     Adds the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="user">The user.</param>
        public void Add(TEntity entity, ClaimsPrincipal? user = null)
        {
            if (entity.Group is null)
            {
                throw new ArgumentException($"The entity '{entity}' does not belong to a group.", nameof(entity));
            }

            if (!Entities.ContainsKey(entity.Group))
            {
                Entities.Add(entity.Group, new Dictionary<string, TEntity>());
            }

            Entities[entity.Group].Add(entity.Name, entity);
        }

        /// <summary>
        ///     Removes the entity with the specified fullname identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="user">The user.</param>
        public void Remove(string id, ClaimsPrincipal? user = null)
        {
            var fullName = GetFullName(id);
            Entities.TryGetValue(fullName.Group!, out var group);
            if (group is null)
            {
                return;
            }

            if (!group.Remove(fullName.Name))
            {
                return;
            }

            if (group.Count == 0)
            {
                Entities.Remove(fullName.Group!);
            }
        }

        /// <summary>
        ///     Updates the specified entity.
        /// </summary>
        /// <param name="updatedEntity">The updated entity.</param>
        /// <param name="user">The user.</param>
        public void Update(TEntity updatedEntity, ClaimsPrincipal? user = null)
        {
            if (updatedEntity.Group is null)
            {
                throw new ArgumentException($"The entity '{updatedEntity}' does not belong to a group.", nameof(updatedEntity));
            }

            var group = Entities[updatedEntity.Group];
            if (updatedEntity is ITraceableEntity<string> entity)
            {
                entity.Added = group[updatedEntity.Name].Added;
            }

            group[updatedEntity.Name] = updatedEntity;
        }

        /// <summary>
        ///     Gets the entities fulfilling the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="user">The user.</param>
        /// <returns>IEnumerable&lt;TEntity&gt;.</returns>
        public virtual IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> predicate, ClaimsPrincipal? user = null)
        {
            foreach (var group in Entities)
            {
                foreach (var entity in group.Value.Values.AsQueryable().Where(predicate))
                {
                    yield return entity.Clone<TEntity>();
                }
            }
        }

        /// <summary>
        ///     Removes the entities fulfilling the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="user">The user.</param>
        public void Remove(Expression<Func<TEntity, bool>> predicate, ClaimsPrincipal? user = null)
        {
            var toRemove = Get(predicate).ToList();
            foreach (var entity in toRemove)
            {
                Remove(entity.FullName);
            }
        }

        private static FullName GetFullName(string id)
        {
            var fullname = FullName.Parse(id);
            if (fullname.Group is null)
            {
                throw new ArgumentException($"Invalid ID '{id}'. The ID of a grouped entity must be a string with following format: {{group}}/{{name}}.", nameof(id));
            }

            return fullname;
        }
    }
}