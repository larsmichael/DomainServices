﻿namespace DomainServices.Abstractions;

using System;
using System.Collections.Generic;
using System.Security.Claims;
using Logging;

/// <summary>
///     Abstract base class for an updatable service.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
/// <typeparam name="TEntityId">The type of the entity identifier.</typeparam>
public abstract class BaseUpdatableService<TEntity, TEntityId> : BaseService<TEntity, TEntityId>, IUpdatableService<TEntity, TEntityId>
    where TEntityId : notnull
    where TEntity : IEntity<TEntityId>
{
    private readonly IUpdatableRepository<TEntity, TEntityId> _repository;

    /// <summary>
    ///     Initializes a new instance of the <see cref="BaseUpdatableService{TEntity, TEntityId}" /> class.
    /// </summary>
    /// <param name="repository">The repository.</param>
    protected BaseUpdatableService(IUpdatableRepository<TEntity, TEntityId> repository)
        : base((IRepository<TEntity, TEntityId>)repository)
    {
        _repository = repository;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="BaseUpdatableService{TEntity, TEntityId}" /> class.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="logger">An optional logger.</param>
    protected BaseUpdatableService(IUpdatableRepository<TEntity, TEntityId> repository, ILogger logger)
        : base((IRepository<TEntity, TEntityId>)repository, logger)
    {
        _repository = repository;
    }

    /// <summary>
    ///     Adds the specified entity.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <param name="user">The user.</param>
    /// <exception cref="ArgumentException"></exception>
    public virtual void Add(TEntity entity, ClaimsPrincipal? user = null)
    {
        if (!((IDiscreteRepository<TEntity, TEntityId>)_repository).Contains(entity.Id, user))
        {
            var cancelEventArgs = new CancelEventArgs<TEntity>(entity);
            OnAdding(cancelEventArgs);
            if (!cancelEventArgs.Cancel)
            {
                if (entity is ITraceableEntity<TEntityId> e)
                {
                    e.Added = DateTime.UtcNow;
                    e.Updated = null;
                }
                _repository.Add(entity, user);
                OnAdded(entity);
            }
        }
        else
        {
            throw new ArgumentException($"'{typeof(TEntity)}' with id '{entity.Id}' already exists.");
        }
    }

    /// <summary>
    ///     Adds or updates the specified entity.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <param name="user">The user.</param>
    public virtual void AddOrUpdate(TEntity entity, ClaimsPrincipal? user = null)
    {
        if (!((IDiscreteRepository<TEntity, TEntityId>)_repository).Contains(entity.Id, user))
        {
            var cancelEventArgs = new CancelEventArgs<TEntity>(entity);
            OnAdding(cancelEventArgs);
            if (!cancelEventArgs.Cancel)
            {
                if (entity is ITraceableEntity<TEntityId> e)
                {
                    e.Added = DateTime.UtcNow;
                    e.Updated = null;
                }
                _repository.Add(entity, user);
                OnAdded(entity);
            }
        }
        else
        {
            var cancelEventArgs = new CancelEventArgs<TEntity>(entity);
            OnUpdating(cancelEventArgs);
            if (!cancelEventArgs.Cancel)
            {
                if (entity is ITraceableEntity<TEntityId> e)
                {
                    e.Updated = DateTime.UtcNow;
                }
                _repository.Update(entity, user);
                OnUpdated(entity);
            }
        }
    }

    /// <summary>
    ///     Try adding the specified entity without existence check.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <param name="user">The user.</param>
    /// <returns><c>true</c> if entity was successfully added, <c>false</c> otherwise.</returns>
    public virtual bool TryAdd(TEntity entity, ClaimsPrincipal? user = null)
    {
        try
        {
            var cancelEventArgs = new CancelEventArgs<TEntity>(entity);
            OnAdding(cancelEventArgs);
            if (cancelEventArgs.Cancel)
            {
                return false;
            }

            if (entity is ITraceableEntity<TEntityId> e)
            {
                e.Added = DateTime.UtcNow;
                e.Updated = null;
            }
            _repository.Add(entity, user);
            OnAdded(entity);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    ///     Try updating the specified entity without existence check.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <param name="user">The user.</param>
    /// <returns><c>true</c> if entity was successfully updated, <c>false</c> otherwise.</returns>
    public virtual bool TryUpdate(TEntity entity, ClaimsPrincipal? user = null)
    {
        try
        {
            var cancelEventArgs = new CancelEventArgs<TEntity>(entity);
            OnUpdating(cancelEventArgs);
            if (cancelEventArgs.Cancel)
            {
                return false;
            }

            if (entity is ITraceableEntity<TEntityId> e)
            {
                e.Updated = DateTime.UtcNow;
            }
            _repository.Update(entity, user);
            OnUpdated(entity);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    ///     Removes the entity with the specified identifier.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="user">The user.</param>
    /// <exception cref="KeyNotFoundException"></exception>
    public virtual void Remove(TEntityId id, ClaimsPrincipal? user = null)
    {
        if (((IDiscreteRepository<TEntity, TEntityId>)_repository).Contains(id, user))
        {
            var cancelEventArgs = new CancelEventArgs<TEntityId>(id);
            OnDeleting(cancelEventArgs);
            if (!cancelEventArgs.Cancel)
            {
                _repository.Remove(id, user);
                OnDeleted(id);
            }
        }
        else
        {
            throw new KeyNotFoundException($"'{typeof(TEntity)}' with id '{id}' was not found.");
        }
    }

    /// <summary>
    ///     Updates the specified entity.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <param name="user">The user.</param>
    /// <exception cref="KeyNotFoundException"></exception>
    public virtual void Update(TEntity entity, ClaimsPrincipal? user = null)
    {
        if (((IDiscreteRepository<TEntity, TEntityId>)_repository).Contains(entity.Id, user))
        {
            var cancelEventArgs = new CancelEventArgs<TEntity>(entity);
            OnUpdating(cancelEventArgs);
            if (!cancelEventArgs.Cancel)
            {
                if (entity is ITraceableEntity<TEntityId> e)
                {
                    e.Updated = DateTime.UtcNow;
                }
                _repository.Update(entity, user);
                OnUpdated(entity);
            }
        }
        else
        {
            throw new KeyNotFoundException($"'{typeof(TEntity)}' with id '{entity.Id}' was not found.");
        }
    }

    /// <summary>
    ///     Occurs when an entity was added.
    /// </summary>
    public event EventHandler<EventArgs<TEntity>>? Added;

    /// <summary>
    ///     Occurs when adding an entity.
    /// </summary>
    public event EventHandler<CancelEventArgs<TEntity>>? Adding;

    /// <summary>
    ///     Occurs when an entity was deleted.
    /// </summary>
    public event EventHandler<EventArgs<TEntityId>>? Deleted;

    /// <summary>
    ///     Occurs when deleting an entity.
    /// </summary>
    public event EventHandler<CancelEventArgs<TEntityId>>? Deleting;

    /// <summary>
    ///     Occurs when an entity was updated.
    /// </summary>
    public event EventHandler<EventArgs<TEntity>>? Updated;

    /// <summary>
    ///     Occurs when updating an entity.
    /// </summary>
    public event EventHandler<CancelEventArgs<TEntity>>? Updating;

    /// <summary>
    ///     Called when an entity was added.
    /// </summary>
    /// <param name="entity">The entity.</param>
    protected virtual void OnAdded(TEntity entity)
    {
        Added?.Invoke(this, new EventArgs<TEntity>(entity));
    }

    /// <summary>
    ///     Called when adding an entity.
    /// </summary>
    /// <param name="e">The event argument.</param>
    protected virtual void OnAdding(CancelEventArgs<TEntity> e)
    {
        Adding?.Invoke(this, e);
    }

    /// <summary>
    ///     Called when an entity was deleted.
    /// </summary>
    /// <param name="entityId">The entity identifier.</param>
    protected virtual void OnDeleted(TEntityId entityId)
    {
        Deleted?.Invoke(this, new EventArgs<TEntityId>(entityId));
    }

    /// <summary>
    ///     Called when deleting an entity.
    /// </summary>
    /// <param name="e">The event argument.</param>
    protected virtual void OnDeleting(CancelEventArgs<TEntityId> e)
    {
        Deleting?.Invoke(this, e);
    }

    /// <summary>
    ///     Called when a entity was updated.
    /// </summary>
    /// <param name="entity">The entity.</param>
    protected virtual void OnUpdated(TEntity entity)
    {
        Updated?.Invoke(this, new EventArgs<TEntity>(entity));
    }

    /// <summary>
    ///     Called when updating an entity.
    /// </summary>
    /// <param name="e">The event argument.</param>
    protected virtual void OnUpdating(CancelEventArgs<TEntity> e)
    {
        Updating?.Invoke(this, e);
    }
}