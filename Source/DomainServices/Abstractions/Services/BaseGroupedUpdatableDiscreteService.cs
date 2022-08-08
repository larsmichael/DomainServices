namespace DomainServices.Abstractions
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using Ardalis.GuardClauses;
    using Logging;

    /// <summary>
    ///     Abstract base class for a discrete, grouped and updatable service.
    ///     It handles entities in a repository with a finite number of (discrete) entities - each identified by a unique ID.
    ///     The entities are organized in a hierarchical (grouped) structure.
    ///     The entities are updatable (add, update and remove).
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TEntityId">The type of the entity identifier.</typeparam>
    public abstract class BaseGroupedUpdatableDiscreteService<TEntity, TEntityId> : BaseUpdatableDiscreteService<TEntity, TEntityId>, IGroupedService<TEntity>, IGroupedUpdatableService
        where TEntityId : notnull
        where TEntity : IEntity<TEntityId>
    {
        private readonly IGroupedRepository<TEntity> _repository;

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseGroupedUpdatableDiscreteService{TEntity,TEntityId}" /> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <exception cref="ArgumentException"></exception>
        protected BaseGroupedUpdatableDiscreteService(IGroupedRepository<TEntity> repository)
            : base((IUpdatableRepository<TEntity, TEntityId>)repository)
        {
            _repository = repository;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseGroupedUpdatableDiscreteService{TEntity,TEntityId}" /> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="logger">An optional logger.</param>
        /// <exception cref="ArgumentException"></exception>
        protected BaseGroupedUpdatableDiscreteService(IGroupedRepository<TEntity> repository, ILogger logger)
            : base((IUpdatableRepository<TEntity, TEntityId>)repository, logger)
        {
            _repository = repository;
        }

        /// <summary>
        ///     Determines whether group exists.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <param name="user">The user.</param>
        /// <returns><c>true</c> if group exists, <c>false</c> otherwise.</returns>
        public virtual bool GroupExists(string group, ClaimsPrincipal? user = null)
        {
            return _repository.ContainsGroup(group, user);
        }

        /// <summary>
        ///     Gets the by group.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <param name="user">The user.</param>
        /// <returns>IEnumerable&lt;TEntity&gt;.</returns>
        /// <exception cref="ArgumentException">Cannot be null or empty. - group</exception>
        /// <exception cref="KeyNotFoundException"></exception>
        public virtual IEnumerable<TEntity> GetByGroup(string group, ClaimsPrincipal? user = null)
        {
            Guard.Against.NullOrEmpty(group, nameof(group));
            if (!_repository.ContainsGroup(group, user))
            {
                throw new KeyNotFoundException($"{typeof(TEntity)} group '{group}' does not exist.");
            }

            return _repository.GetByGroup(group, user);
        }

        /// <summary>
        ///     Gets the entities in each group.
        /// </summary>
        /// <param name="groups">The list of groups</param>
        /// <param name="user">The user.</param>
        /// <returns>IEnumerable&lt;TEntity&gt;.</returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public virtual IEnumerable<TEntity> GetByGroups(IEnumerable<string> groups, ClaimsPrincipal? user = null)
        {
            var list = new List<TEntity>();
            foreach (var group in groups)
            {
                list.AddRange(GetByGroup(group, user));
            }

            return list;
        }

        /// <summary>
        ///     Gets the full names within the specified group.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <param name="user">The user.</param>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        /// <exception cref="ArgumentException">Cannot be null or empty. - group</exception>
        /// <exception cref="KeyNotFoundException"></exception>
        public virtual IEnumerable<string> GetFullNames(string group, ClaimsPrincipal? user = null)
        {
            Guard.Against.NullOrEmpty(group, nameof(group));
            if (!_repository.ContainsGroup(group, user))
            {
                throw new KeyNotFoundException($"{typeof(TEntity)} group '{group}' does not exist.");
            }

            return _repository.GetFullNames(group, user);
        }

        /// <summary>
        ///     Gets the full names.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        public virtual IEnumerable<string> GetFullNames(ClaimsPrincipal? user = null)
        {
            return _repository.GetFullNames(user);
        }

        /// <summary>
        ///     Removes all entities within the group with the specified identifier.
        /// </summary>
        /// <param name="group">The group identifier.</param>
        /// <param name="user">The user.</param>
        public virtual void RemoveByGroup(string group, ClaimsPrincipal? user = null)
        {
            if (!_repository.ContainsGroup(group, user))
            {
                throw new KeyNotFoundException($"{typeof(TEntity)} group '{group}' does not exist.");
            }

            var cancelEventArgs = new CancelEventArgs<string>(group);
            OnDeletingGroup(cancelEventArgs);
            if (cancelEventArgs.Cancel)
            {
                return;
            }

            if (_repository is IGroupedUpdatableRepository repository)
            {
                repository.RemoveByGroup(group);
            }
            else
            {
                foreach (var entity in _repository.GetByGroup(group, user))
                {
                    Remove(entity.Id);
                }
            }

            OnDeletedGroup(group);
        }

        /// <summary>
        ///     Occurs when a group of entities was deleted.
        /// </summary>
        public event EventHandler<EventArgs<string>>? DeletedGroup;

        /// <summary>
        ///     Occurs when deleting a group of entities.
        /// </summary>
        public event EventHandler<CancelEventArgs<string>>? DeletingGroup;

        /// <summary>
        ///     Called when a group of entities was deleted.
        /// </summary>
        /// <param name="group">The group identifier.</param>
        protected virtual void OnDeletedGroup(string group)
        {
            DeletedGroup?.Invoke(this, new EventArgs<string>(group));
        }

        /// <summary>
        ///     Called when deleting a group of entities.
        /// </summary>
        /// <param name="e">The event argument.</param>
        protected virtual void OnDeletingGroup(CancelEventArgs<string> e)
        {
            DeletingGroup?.Invoke(this, e);
        }
    }
}