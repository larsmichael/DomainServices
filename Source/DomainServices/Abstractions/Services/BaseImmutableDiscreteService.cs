namespace DomainServices.Abstractions
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using Logging;

    /// <summary>
    ///     Abstract base class for a discrete, immutable service.
    ///     It handles entities in a repository with a finite number of (discrete) entities - each identified by a unique ID.
    ///     The entities are immutable (add and remove - but not update).
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TEntityId">The type of the entity identifier.</typeparam>
    public abstract class BaseImmutableDiscreteService<TEntity, TEntityId> : BaseDiscreteService<TEntity, TEntityId>, IImmutableService<TEntity, TEntityId>
        where TEntityId : notnull
        where TEntity : IEntity<TEntityId>
    {
        private readonly IImmutableRepository<TEntity, TEntityId> _repository;

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseImmutableDiscreteService{TEntity}" /> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        protected BaseImmutableDiscreteService(IImmutableRepository<TEntity, TEntityId> repository)
            : base((IDiscreteRepository<TEntity, TEntityId>)repository)
        {
            _repository = repository;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseImmutableDiscreteService{TEntity}" /> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="logger">An optional logger.</param>
        protected BaseImmutableDiscreteService(IImmutableRepository<TEntity, TEntityId> repository, ILogger logger)
            : base((IDiscreteRepository<TEntity, TEntityId>)repository, logger)
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
            var cancelEventArgs = new CancelEventArgs<TEntity>(entity);
            OnAdding(cancelEventArgs);
            if (cancelEventArgs.Cancel)
            {
                return;
            }

            _repository.Add(entity, user);
            OnAdded(entity);
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
        ///     Called when an entity was added.
        /// </summary>
        protected virtual void OnAdded(TEntity entity)
        {
            Added?.Invoke(this, new EventArgs<TEntity>(entity));
        }

        /// <summary>
        ///     Called when adding an entity.
        /// </summary>
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
        protected virtual void OnDeleting(CancelEventArgs<TEntityId> e)
        {
            Deleting?.Invoke(this, e);
        }
    }

    /// <summary>
    ///     Abstract base class for a discrete, immutable service.
    ///     It handles entities in a repository with a finite number of (discrete) entities - each identified by a unique ID.
    ///     The entities are immutable (add and remove - but not update).
    ///     Entity ID is of type Guid.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public abstract class BaseImmutableDiscreteService<TEntity> : BaseImmutableDiscreteService<TEntity, Guid> where TEntity : IEntity<Guid>
    {
        protected BaseImmutableDiscreteService(IImmutableRepository<TEntity, Guid> repository) : base(repository)
        {
        }

        protected BaseImmutableDiscreteService(IImmutableRepository<TEntity, Guid> repository, ILogger logger) : base(repository, logger)
        {
        }
    }
}