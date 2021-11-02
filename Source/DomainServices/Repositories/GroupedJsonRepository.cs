namespace DomainServices.Repositories
{
    using System.Text.Json.Serialization;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Security.Claims;
    using Abstractions;
    using System.Text.Json;

    /// <summary>
    ///     Generic grouped JSON repository.
    ///     All entities must belong to a group.
    ///     The grouping is one level only (not hierarchical).
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public class GroupedJsonRepository<TEntity> :
        IRepository<TEntity, string>,
        IDiscreteRepository<TEntity, string>,
        IGroupedRepository<TEntity>,
        IUpdatableRepository<TEntity, string> where TEntity : IGroupedEntity<string>
    {
        private static readonly object _syncObject = new();
        private readonly IEqualityComparer<string>? _comparer;
        private readonly FileInfo _fileInfo;
        private readonly string _filePath;
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private Dictionary<string, Dictionary<string, TEntity>> _entities;
        private DateTime _lastModified = DateTime.MinValue;

        /// <summary>
        ///     Initializes a new instance of the <see cref="GroupedJsonRepository{TEntity}" /> class.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="converters">Optional converters.</param>
        /// <param name="comparer">Optional equality comparer</param>
        /// <exception cref="ArgumentNullException">filePath</exception>
        public GroupedJsonRepository(string filePath, IEnumerable<JsonConverter>? converters = null, IEqualityComparer<string>? comparer = null)
        {
            _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
            _comparer = comparer;
            _entities = new Dictionary<string, Dictionary<string, TEntity>>(_comparer);
            _fileInfo = new FileInfo(_filePath);
            _jsonSerializerOptions = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = true
            };
            _jsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            if (converters != null)
            {
                foreach (var converter in converters)
                {
                    _jsonSerializerOptions.Converters.Add(converter);
                }
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="GroupedJsonRepository{TEntity}" /> class.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="converters">The converters.</param>
        public GroupedJsonRepository(string filePath, IEnumerable<JsonConverter>? converters = null)
            : this(filePath, converters, null)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="GroupedJsonRepository{TEntity}" /> class.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        public GroupedJsonRepository(string filePath)
            : this(filePath, null, null)
        {
        }

        private Dictionary<string, Dictionary<string, TEntity>> Entities
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
                var count = 0;
                foreach (var group in Entities)
                {
                    count += group.Value.Count;
                }

                return count;
            }
        }

        /// <summary>
        ///     Determines whether an entity with the specified fullname identifier exists.
        /// </summary>
        /// <param name="id">The fullname identifier.</param>
        /// <param name="user">The user.</param>
        /// <returns><c>true</c> if entity with the specified fullname identifier exists, <c>false</c> otherwise.</returns>
        public bool Contains(string id, ClaimsPrincipal? user = null)
        {
            lock (_syncObject)
            {
                var fullname = GetFullName(id);
                return Entities.ContainsKey(fullname.Group!) && Entities[fullname.Group!].ContainsKey(fullname.Name);
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
                var entities = new List<TEntity>();
                foreach (var group in _entities)
                {
                    foreach (var entity in group.Value.Values)
                    {
                        entities.Add(entity);
                    }
                }

                return entities;
            }
        }

        /// <summary>
        ///     Gets all entity identifiers.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>IEnumerable&lt;string&gt;.</returns>
        public IEnumerable<string> GetIds(ClaimsPrincipal? user = null)
        {
            lock (_syncObject)
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
        }

        /// <summary>
        ///     Determines whether the repository contains the specified group.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <param name="user">The user.</param>
        /// <returns><c>true</c> if the repository contains the specified group; otherwise, <c>false</c>.</returns>
        public bool ContainsGroup(string group, ClaimsPrincipal? user = null)
        {
            lock (_syncObject)
            {
                return Entities.ContainsKey(group);
            }
        }

        /// <summary>
        ///     Gets entities by group.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <param name="user">The user.</param>
        /// <returns>IEnumerable&lt;TEntity&gt;.</returns>
        public IEnumerable<TEntity> GetByGroup(string group, ClaimsPrincipal? user = null)
        {
            lock (_syncObject)
            {
                Deserialize();
                return _entities[group].Values.ToArray();
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
            lock (_syncObject)
            {
                return Entities[group].Values.Select(e => e.FullName).ToArray();
            }
        }

        /// <summary>
        ///     Gets the full names.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        public IEnumerable<string> GetFullNames(ClaimsPrincipal? user = null)
        {
            lock (_syncObject)
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
        }

        /// <summary>
        ///     Gets the entity with the specified fullname identifier.
        /// </summary>
        /// <param name="id">The fullname identifier.</param>
        /// <param name="user">The user.</param>
        /// <returns>Maybe&lt;TEntity&gt;.</returns>
        public Maybe<TEntity> Get(string id, ClaimsPrincipal? user = null)
        {
            lock (_syncObject)
            {
                var fullName = GetFullName(id);
                Deserialize();
                if (!_entities.Any())
                {
                    return Maybe.Empty<TEntity>();
                }

                _entities.TryGetValue(fullName.Group!, out var group);
                if (group is null)
                {
                    return Maybe.Empty<TEntity>();
                }

                group.TryGetValue(fullName.Name, out var entity);
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
                if (entity.Group is null)
                {
                    throw new ArgumentException($"The entity '{entity}' does not belong to a group.", nameof(entity));
                }

                if (!Entities.ContainsKey(entity.Group))
                {
                    Entities.Add(entity.Group, new Dictionary<string, TEntity>(_comparer));
                }

                Entities[entity.Group].Add(entity.Name, entity);
                Serialize();
            }
        }

        /// <summary>
        ///     Removes the entity with the specified fullname identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="user">The user.</param>
        public void Remove(string id, ClaimsPrincipal? user = null)
        {
            lock (_syncObject)
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

                Serialize();
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
                var entities = new List<TEntity>();
                if (!_entities.Any())
                {
                    return entities;
                }

                foreach (var group in _entities)
                {
                    foreach (var entity in group.Value.Values.AsQueryable().Where(predicate))
                    {
                        entities.Add(entity);
                    }
                }

                return entities;
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
                    Remove(entity.FullName);
                }
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

            using var streamReader = new StreamReader(_filePath);
            var json = streamReader.ReadToEnd();
            var entities = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, TEntity>>>(json, _jsonSerializerOptions);
            _entities = new Dictionary<string, Dictionary<string, TEntity>>(_comparer);
            foreach (var group in entities!)
            {
                _entities.Add(group.Key, new Dictionary<string, TEntity>(group.Value, _comparer));
            }
        }
    }
}