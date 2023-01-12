namespace DomainServices.Abstractions;

using System;
using System.Collections.Generic;
using System.Security.Claims;
using Logging;

/// <summary>
///     Abstract base class for a discrete service.
///     A discrete service handles entities in a repository with a finite number of (discrete) entities - each identified
///     by a unique ID.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
/// <typeparam name="TEntityId">The type of the entity identifier.</typeparam>
public abstract class BaseDiscreteService<TEntity, TEntityId> : BaseService<TEntity, TEntityId>, IDiscreteService<TEntity, TEntityId>
    where TEntityId : notnull
    where TEntity : IEntity<TEntityId>
{
    private readonly IDiscreteRepository<TEntity, TEntityId> _repository;

    /// <summary>
    ///     Initializes a new instance of the <see cref="BaseDiscreteService{TEntity, TEntityId}" /> class.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <exception cref="ArgumentNullException">repository</exception>
    protected BaseDiscreteService(IDiscreteRepository<TEntity, TEntityId> repository)
        : base((IRepository<TEntity, TEntityId>)repository)
    {
        _repository = repository;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="BaseDiscreteService{TEntity, TEntityId}" /> class.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="logger">An optional logger.</param>
    /// <exception cref="ArgumentNullException">repository</exception>
    protected BaseDiscreteService(IDiscreteRepository<TEntity, TEntityId> repository, ILogger logger)
        : base((IRepository<TEntity, TEntityId>)repository, logger)
    {
        _repository = repository;
    }

    /// <summary>
    ///     Counts the number of entities.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <returns>System.Int32.</returns>
    public virtual int Count(ClaimsPrincipal? user = null)
    {
        return _repository.Count(user);
    }

    /// <summary>
    ///     Determines whether an entity with the specified identifier exists.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="user">The user.</param>
    /// <returns><c>true</c> if entity with the specified identifier exists, <c>false</c> otherwise.</returns>
    public virtual bool Exists(TEntityId id, ClaimsPrincipal? user = null)
    {
        return _repository.Contains(id, user);
    }

    /// <summary>
    ///     Gets all entities.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <returns>IEnumerable&lt;TEntity&gt;.</returns>
    public virtual IEnumerable<TEntity> GetAll(ClaimsPrincipal? user = null)
    {
        return _repository.GetAll(user);
    }

    /// <summary>
    ///     Gets all entity identifiers.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <returns>IEnumerable&lt;TEntityId&gt;.</returns>
    public virtual IEnumerable<TEntityId> GetIds(ClaimsPrincipal? user = null)
    {
        return _repository.GetIds(user);
    }
}