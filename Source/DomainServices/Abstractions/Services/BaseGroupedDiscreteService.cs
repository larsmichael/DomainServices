namespace DomainServices.Abstractions
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using Ardalis.GuardClauses;
    using Logging;

    /// <summary>
    ///     Abstract base class for a grouped, discrete service.
    ///     It handles entities in a repository with a finite number of (discrete) entities - each identified by a unique ID.
    ///     The entities are organized in a hierarchical (grouped) structure.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TEntityId">The type of the entity identifier.</typeparam>
    public abstract class BaseGroupedDiscreteService<TEntity, TEntityId> : BaseDiscreteService<TEntity, TEntityId>, IGroupedService<TEntity>
        where TEntityId : notnull
        where TEntity : IEntity<TEntityId>
    {
        private readonly IGroupedRepository<TEntity> _repository;

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseGroupedDiscreteService{TEntity, TEntityId}" /> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        protected BaseGroupedDiscreteService(IGroupedRepository<TEntity> repository)
            : base((IDiscreteRepository<TEntity, TEntityId>)repository)
        {
            _repository = repository;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseGroupedDiscreteService{TEntity, TEntityId}" /> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="logger">An optional logger.</param>
        protected BaseGroupedDiscreteService(IGroupedRepository<TEntity> repository, ILogger logger)
            : base((IDiscreteRepository<TEntity, TEntityId>)repository, logger)
        {
            _repository = repository;
        }

        /// <summary>
        ///     Gets the entities by group.
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
        ///     Gets all the full names.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        public virtual IEnumerable<string> GetFullNames(ClaimsPrincipal? user = null)
        {
            return _repository.GetFullNames(user);
        }
    }
}