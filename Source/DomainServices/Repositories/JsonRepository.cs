namespace DomainServices.Repositories;

using System;
using System.Collections.Generic;
using System.Security.Claims;
using Abstractions;
using System.Text.Json.Serialization;

/// <summary>
///     Generic Json Repository.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
/// <typeparam name="TEntityId">The type of the entity identifier.</typeparam>
/// <seealso cref="IRepository{TEntity,TEntityId}" />
/// <seealso cref="IDiscreteRepository{TEntity,TEntityId}" />
/// <seealso cref="IUpdatableRepository{TEntity,TEntityId}" />
public class JsonRepository<TEntity, TEntityId> : ImmutableJsonRepository<TEntity, TEntityId>,
    IUpdatableRepository<TEntity, TEntityId>
    where TEntityId : notnull
    where TEntity : IEntity<TEntityId>
{
    private static readonly object _syncObject = new();

    /// <summary>
    ///     Initializes a new instance of the <see cref="JsonRepository{TEntity, TEntityId}" /> class.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    /// <param name="converters">Optional converters.</param>
    /// <param name="comparer">Equality comparer</param>
    /// <exception cref="ArgumentNullException">filePath</exception>
    public JsonRepository(string filePath, IEnumerable<JsonConverter>? converters = null, IEqualityComparer<TEntityId>? comparer = null)
        : base(filePath, converters, comparer)
    {
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

    /// <summary>
    ///     Updates the specified entity.
    /// </summary>
    /// <param name="updatedEntity">The updated entity.</param>
    /// <param name="user">The user.</param>
    public void Update(TEntity updatedEntity, ClaimsPrincipal? user = null)
    {
        lock (_syncObject)
        {
            if (!Entities.ContainsKey(updatedEntity.Id))
            {
                throw new KeyNotFoundException($"'{typeof(TEntity)}' with id '{updatedEntity.Id}' was not found.");
            }

            if (updatedEntity is ITraceableEntity<TEntityId> entity)
            {
                entity.Added = ((ITraceableEntity<TEntityId>)Entities[entity.Id]).Added;
            }

            Entities[updatedEntity.Id] = updatedEntity;
            Serialize();
        }
    }
}