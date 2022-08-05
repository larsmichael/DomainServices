namespace DomainServices.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Security.Claims;
    using Abstractions;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    /// <summary>
    ///     Generic Json Repository.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TEntityId">The type of the entity identifier.</typeparam>
    /// <seealso cref="IRepository{TEntity,TEntityId}" />
    /// <seealso cref="IDiscreteRepository{TEntity,TEntityId}" />
    /// <seealso cref="IUpdatableRepository{TEntity,TEntityId}" />
    public class JsonRepository<TEntity, TEntityId> :
        IRepository<TEntity, TEntityId>,
        IDiscreteRepository<TEntity, TEntityId>,
        IUpdatableRepository<TEntity, TEntityId>
        where TEntityId : notnull
        where TEntity : IEntity<TEntityId>
    {
        private static readonly object _syncObject = new();
        private readonly IEqualityComparer<TEntityId>? _comparer;
        private readonly FileInfo _fileInfo;
        private readonly string _filePath;
        private readonly JsonSerializerOptions _options;
        private Dictionary<TEntityId, TEntity> _entities;
        private DateTime _lastModified = DateTime.MinValue;

        /// <summary>
        ///     Initializes a new instance of the <see cref="JsonRepository{TEntity, TEntityId}" /> class.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="converters">Optional converters.</param>
        /// <param name="comparer">Equality comparer</param>
        /// <exception cref="ArgumentNullException">filePath</exception>
        public JsonRepository(string filePath, IEnumerable<JsonConverter>? converters = null, IEqualityComparer<TEntityId>? comparer = null)
        {
            _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
            _comparer = comparer;
            _entities = new Dictionary<TEntityId, TEntity>(_comparer);
            _fileInfo = new FileInfo(_filePath);
            _options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = true
            };
            _options.Converters.Add(new JsonStringEnumConverter());
            _options.Converters.Add(new ObjectToInferredTypesConverter());
            if (converters != null)
            {
                foreach (var converter in converters)
                {
                    _options.Converters.Add(converter);
                }
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="JsonRepository{TEntity, TEntityId}" /> class.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="converters">The converters.</param>
        public JsonRepository(string filePath, IEnumerable<JsonConverter>? converters = null)
            : this(filePath, converters, null)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="JsonRepository{TEntity, TEntityId}" /> class.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        public JsonRepository(string filePath)
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
        public int Count(ClaimsPrincipal? user = null)
        {
            lock (_syncObject)
            {
                return Entities.Count;
            }
        }

        /// <summary>
        ///     Determines whether an entity with the specified identifier exists.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="user">The user.</param>
        /// <returns><c>true</c> if entity with the specified identifier exists, <c>false</c> otherwise.</returns>
        public bool Contains(TEntityId id, ClaimsPrincipal? user = null)
        {
            lock (_syncObject)
            {
                return Entities.ContainsKey(id);
            }
        }

        /// <summary>
        ///     Gets all entities.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>IEnumerable&lt;TEntity&gt;.</returns>
        public virtual IEnumerable<TEntity> GetAll(ClaimsPrincipal? user = null)
        {
            lock (_syncObject)
            {
                Deserialize();
                return _entities.Values;
            }
        }

        /// <summary>
        ///     Gets all entity identifiers.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>IEnumerable&lt;TEntityId&gt;.</returns>
        public IEnumerable<TEntityId> GetIds(ClaimsPrincipal? user = null)
        {
            lock (_syncObject)
            {
                return Entities.Values.Select(e => e.Id);
            }
        }

        /// <summary>
        ///     Gets the entity with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="user">The user.</param>
        /// <returns>Maybe&lt;TEntity&gt;.</returns>
        public Maybe<TEntity> Get(TEntityId id, ClaimsPrincipal? user = null)
        {
            lock (_syncObject)
            {
                Deserialize();
                if (!_entities.Any())
                {
                    return Maybe.Empty<TEntity>();
                }

                _entities.TryGetValue(id, out var entity);
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
        public void Remove(TEntityId id, ClaimsPrincipal? user = null)
        {
            lock (_syncObject)
            {
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
        public void Update(TEntity updatedEntity, ClaimsPrincipal? user = null)
        {
            lock (_syncObject)
            {
                if (!_entities.ContainsKey(updatedEntity.Id))
                {
                    throw new KeyNotFoundException($"'{typeof(TEntity)}' with id '{updatedEntity.Id}' was not found.");
                }

                if (updatedEntity is ITraceableEntity<TEntityId> entity)
                {
                    entity.Added = ((ITraceableEntity<TEntityId>)_entities[entity.Id]).Added;
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
        public virtual IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> predicate, ClaimsPrincipal? user = null)
        {
            lock (_syncObject)
            {
                Deserialize();
                if (!_entities.Any())
                {
                    return new List<TEntity>();
                }

                return _entities.Values.AsQueryable().Where(predicate);
            }
        }

        /// <summary>
        ///     Removes the entities fulfilling the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="user">The user.</param>
        public void Remove(Expression<Func<TEntity, bool>> predicate, ClaimsPrincipal? user = null)
        {
            lock (_syncObject)
            {
                var toRemove = Get(predicate);
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
            var json = JsonSerializer.Serialize(_entities, _options);
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
                _entities = new Dictionary<TEntityId, TEntity>(JsonSerializer.Deserialize<Dictionary<TEntityId, TEntity>>(json, _options)!, _comparer);
            }
            catch (Exception exception)
            {
                throw new Exception($"Cannot deserialize file {_filePath} with message {exception.Message}");
            }
        }
    }
}