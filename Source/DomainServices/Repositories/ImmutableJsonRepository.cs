namespace DomainServices.Repositories;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using Abstractions;

/// <summary>
///     Immutable Json Repository.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
/// <typeparam name="TEntityId">The type of the entity identifier.</typeparam>
/// <seealso cref="IRepository{TEntity,TEntityId}" />
/// <seealso cref="IDiscreteRepository{TEntity,TEntityId}" />
/// <seealso cref="IImmutableRepository{TEntity}" />
public class ImmutableJsonRepository<TEntity, TEntityId> :
    IRepository<TEntity, TEntityId>,
    IDiscreteRepository<TEntity, TEntityId>,
    IImmutableRepository<TEntity, TEntityId>
    where TEntityId : notnull
    where TEntity : IEntity<TEntityId>
{
    private static readonly object _syncObject = new();
    private readonly IEqualityComparer<TEntityId>? _comparer;
    private readonly FileInfo _fileInfo;
    private readonly string _filePath;
    private readonly JsonSerializerOptions _serializerOptions;
    private readonly JsonSerializerOptions _deserializerOptions;
    private Dictionary<TEntityId, TEntity> _entities;
    private DateTime _lastModified = DateTime.MinValue;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ImmutableJsonRepository{TEntity,TEntityId}" /> class.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    /// <param name="converters">Optional converters.</param>
    /// <param name="comparer">Equality comparer</param>
    /// <exception cref="ArgumentNullException">filePath</exception>
    public ImmutableJsonRepository(string filePath, IEnumerable<JsonConverter>? converters = null, IEqualityComparer<TEntityId>? comparer = null)
    {
        _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        _comparer = comparer;
        _entities = new Dictionary<TEntityId, TEntity>(_comparer);
        _fileInfo = new FileInfo(_filePath);

        // Serialization options
        _serializerOptions = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = true
        };
        _serializerOptions.Converters.Add(new JsonStringEnumConverter());
        if (converters != null)
        {
            foreach (var converter in converters)
            {
                _serializerOptions.Converters.Add(converter);
            }
        }

        // Deserialization options
        _deserializerOptions = new JsonSerializerOptions();
        _deserializerOptions.Converters.Add(new JsonStringEnumConverter());
        _deserializerOptions.Converters.Add(new ObjectToInferredTypeConverter());
        if (converters != null)
        {
            foreach (var converter in converters)
            {
                _deserializerOptions.Converters.Add(converter);
            }
        }
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ImmutableJsonRepository{TEntity, TEntityId}" /> class.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    /// <param name="converters">The converters.</param>
    public ImmutableJsonRepository(string filePath, IEnumerable<JsonConverter>? converters = null)
        : this(filePath, converters, null)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ImmutableJsonRepository{TEntity, TEntityId}" /> class.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    public ImmutableJsonRepository(string filePath)
        : this(filePath, null, null)
    {
    }

    protected Dictionary<TEntityId, TEntity> Entities
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

    protected void Serialize()
    {
        using var streamWriter = new StreamWriter(_filePath);
        var json = JsonSerializer.Serialize(_entities, _serializerOptions);
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
            _entities = new Dictionary<TEntityId, TEntity>(JsonSerializer.Deserialize<Dictionary<TEntityId, TEntity>>(json, _deserializerOptions)!, _comparer);
        }
        catch (Exception exception)
        {
            throw new Exception($"Cannot deserialize file {_filePath} with message {exception.Message}");
        }
    }
}