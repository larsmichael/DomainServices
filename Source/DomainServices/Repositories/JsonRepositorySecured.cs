#pragma warning disable CS8767 // Nullability of reference types in type of parameter doesn't match implicitly implemented member (possibly because of nullability attributes).
namespace DomainServices.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Security.Claims;
    using Abstractions;
    using Authorization;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    /// <summary>
    ///     Generic Json Repository supporting permissions on entities.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TEntityId">The type of the entity identifier.</typeparam>
    /// <seealso cref="IRepository{TEntity,TEntityId}" />
    /// <seealso cref="IDiscreteRepository{TEntity,TEntityId}" />
    /// <seealso cref="IUpdatableRepository{TEntity,TEntityId}" />
    public class JsonRepositorySecured<TEntity, TEntityId> :
        IRepository<TEntity, TEntityId>,
        IDiscreteRepository<TEntity, TEntityId>,
        IUpdatableRepository<TEntity, TEntityId>
        where TEntityId : notnull
        where TEntity : ISecuredEntity<TEntityId>
    {
        private static readonly object _syncObject = new();
        private readonly IEqualityComparer<TEntityId>? _comparer;
        private readonly FileInfo _fileInfo;
        private readonly string _filePath;
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private Dictionary<TEntityId, TEntity> _entities;
        private DateTime _lastModified = DateTime.MinValue;

        /// <summary>
        ///     Initializes a new instance of the <see cref="JsonRepository{TEntity, TEntityId}" /> class.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="converters">Optional converters.</param>
        /// <param name="comparer">Equality comparer</param>
        /// <exception cref="ArgumentNullException">filePath</exception>
        public JsonRepositorySecured(string filePath, IEnumerable<JsonConverter>? converters = null, IEqualityComparer<TEntityId>? comparer = null)
        {
            _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
            _comparer = comparer;
            _entities = new Dictionary<TEntityId, TEntity>(_comparer);
            _fileInfo = new FileInfo(_filePath);
            _jsonSerializerOptions = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = true
            };
            if (converters != null)
            {
                foreach (var converter in converters)
                {
                    _jsonSerializerOptions.Converters.Add(converter);
                }
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="JsonRepository{TEntity, TEntityId}" /> class.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="converters">The converters.</param>
        public JsonRepositorySecured(string filePath, IEnumerable<JsonConverter>? converters = null)
            : this(filePath, converters, null)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="JsonRepository{TEntity, TEntityId}" /> class.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        public JsonRepositorySecured(string filePath)
            : this(filePath, null, null)
        {
        }

        private Dictionary<TEntityId, TEntity> Entities
        {
            get
            {
                _fileInfo.Refresh();
                if (_fileInfo.LastWriteTime == _lastModified)
                {
                    return _entities;
                }

                Deserialize();
                _lastModified = _fileInfo.LastWriteTime;
                return _entities;
            }
        }

        /// <summary>
        ///     The total number of entities.
        /// </summary>
        /// <param name="user">The user.</param>

        public int Count(ClaimsPrincipal user)

        {
            lock (_syncObject)
            {
                return Entities.Count(e => e.Value.IsAllowed(user, "read"));
            }
        }

        /// <summary>
        ///     Determines whether an entity with the specified identifier exists.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="user">The user.</param>
        /// <returns><c>true</c> if entity with the specified identifier exists, <c>false</c> otherwise.</returns>
        public bool Contains(TEntityId id, ClaimsPrincipal user)
        {
            lock (_syncObject)
            {
                return Entities.Where(e => e.Value.IsAllowed(user, "read")).ToDictionary(kvp => kvp.Key, kvp => kvp.Value, _comparer).ContainsKey(id);
            }
        }

        /// <summary>
        ///     Gets all entities.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>IEnumerable&lt;TEntity&gt;.</returns>
        public virtual IEnumerable<TEntity> GetAll(ClaimsPrincipal user)
        {
            lock (_syncObject)
            {
                Deserialize();
                return Entities.Where(e => e.Value.IsAllowed(user, "read")).Select(kvp => kvp.Value);
            }
        }

        /// <summary>
        ///     Gets all entity identifiers.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>IEnumerable&lt;TEntityId&gt;.</returns>
        public IEnumerable<TEntityId> GetIds(ClaimsPrincipal user)
        {
            lock (_syncObject)
            {
                return Entities.Where(e => e.Value.IsAllowed(user, "read")).Select(kvp => kvp.Value.Id);
            }
        }

        /// <summary>
        ///     Gets the entity with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="user">The user.</param>
        /// <returns>Maybe&lt;TEntity&gt;.</returns>
        public Maybe<TEntity> Get(TEntityId id, ClaimsPrincipal user)
        {
            lock (_syncObject)
            {
                Deserialize();
                if (!_entities.Any())
                {
                    return Maybe.Empty<TEntity>();
                }

                _entities.Where(e => e.Value.IsAllowed(user, "read")).ToDictionary(kvp => kvp.Key, kvp => kvp.Value, _comparer).TryGetValue(id, out var entity);
                return entity == null || entity.Equals(default(TEntity)) ? Maybe.Empty<TEntity>() : entity.ToMaybe();
            }
        }

        /// <summary>
        ///     Adds the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="user">The user.</param>
        public void Add(TEntity entity, ClaimsPrincipal? user = null)
        {
            if (!entity.Permissions.Any())
            {
                throw new ArgumentException("No permissions were defined for the entity.", nameof(entity));
            }

            if (entity.Permissions.All(p => p.Operation != "read"))
            {
                throw new ArgumentException("No 'read' permissions were defined for the entity.", nameof(entity));
            }

            lock (_syncObject)
            {
                Entities.Add(entity.Id, entity);
                Serialize();
            }
        }

        /// <summary>
        ///     Removes the entity with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="user">The user.</param>
        public void Remove(TEntityId id, ClaimsPrincipal user)
        {
            lock (_syncObject)
            {
                if (!Entities[id].IsAllowed(user, "delete"))
                {
                    throw new ArgumentException($"User '{user.GetUserId()}' does not have permission to remove entity with id '{id}'.", nameof(id));
                }

                if (Entities.Remove(id))
                {
                    Serialize();
                }
            }
        }

        /// <summary>
        ///     Updates the specified entity.
        /// </summary>
        /// <param name="updatedEntity">The updated entity.</param>
        /// <param name="user">The user.</param>
        public void Update(TEntity updatedEntity, ClaimsPrincipal user)
        {
            if (!updatedEntity.Permissions.Any())
            {
                throw new ArgumentException("No permissions were defined for the entity.", nameof(updatedEntity));
            }

            if (updatedEntity.Permissions.All(p => p.Operation != "read"))
            {
                throw new ArgumentException("No 'read' permissions were defined for the entity.", nameof(updatedEntity));
            }

            lock (_syncObject)
            {
                if (!Entities[updatedEntity.Id].IsAllowed(user, "update"))
                {
                    throw new ArgumentException($"User '{user.GetUserId()}' does not have permission to update entity with id '{updatedEntity.Id}'.", nameof(updatedEntity));
                }

                if (updatedEntity is ITraceableEntity<TEntityId> entity)
                {
                    entity.Added = _entities[entity.Id].Added;
                }

                Entities[updatedEntity.Id] = updatedEntity;
                Serialize();
            }
        }

        /// <summary>
        ///     Gets the entities fulfilling the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="user">The user.</param>
        /// <returns>IEnumerable&lt;TEntity&gt;.</returns>
        public virtual IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> predicate, ClaimsPrincipal user)
        {
            lock (_syncObject)
            {
                Deserialize();
                if (!_entities.Any())
                {
                    return new List<TEntity>();
                }

                return _entities.Values.AsQueryable().Where(predicate).Where(e => e.IsAllowed(user, "read"));
            }
        }

        /// <summary>
        ///     Removes the entities fulfilling the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="user">The user.</param>
        public void Remove(Expression<Func<TEntity, bool>> predicate, ClaimsPrincipal user)
        {
            lock (_syncObject)
            {
                var toRemove = Get(predicate, user);
                foreach (var entity in toRemove)
                {
                    Entities.Remove(entity.Id);
                }

                Serialize();
            }
        }

        private void Serialize()
        {
            using var streamWriter = new StreamWriter(_filePath);
            var json = JsonSerializer.Serialize(_entities, _jsonSerializerOptions);
            streamWriter.Write(json);
        }

        private void Deserialize()
        {
            if (!File.Exists(_filePath))
            {
                return;
            }

            try
            {
                using var streamReader = new StreamReader(_filePath);
                var json = streamReader.ReadToEnd();
                _entities = new Dictionary<TEntityId, TEntity>(JsonSerializer.Deserialize<Dictionary<TEntityId, TEntity>>(json, _jsonSerializerOptions)!, _comparer);
            }
            catch (Exception exception)
            {
                throw new Exception($"Cannot deserialize file {_filePath} with message {exception.Message}");
            }
        }
    }
}