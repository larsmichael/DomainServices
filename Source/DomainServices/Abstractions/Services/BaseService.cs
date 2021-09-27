namespace DomainServices.Abstractions
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using Logging;

    /// <summary>
    ///     Abstract base class for a service.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TEntityId">The type of the entity identifier.</typeparam>
    public abstract class BaseService<TEntity, TEntityId> : IService<TEntity, TEntityId> where TEntity : IEntity<TEntityId>
    {
        private readonly ILogger _logger;
        private readonly IRepository<TEntity, TEntityId> _repository;

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseService{TEntity, TEntityId}" /> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <exception cref="ArgumentNullException">repository</exception>
        protected BaseService(IRepository<TEntity, TEntityId> repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseService{TEntity, TEntityId}" /> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="logger">An optional logger.</param>
        /// <exception cref="ArgumentNullException">repository</exception>
        protected BaseService(IRepository<TEntity, TEntityId> repository, ILogger logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger;
        }

        /// <summary>
        ///     Gets the entity with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="user">The user.</param>
        /// <returns>TEntity.</returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public virtual TEntity Get(TEntityId id, ClaimsPrincipal user = null)
        {
            var maybe = _repository.Get(id, user);
            if (!maybe.HasValue)
            {
                throw new KeyNotFoundException($"'{typeof(TEntity)}' with id '{id}' was not found.");
            }

            return maybe.Value;
        }

        /// <summary>
        ///     Gets a list of entities with the specified identifiers.
        /// </summary>
        /// <remarks>
        ///     If an identifier is not found, this is logged, if a logger is injected from the service constructor.
        /// </remarks>
        /// <param name="ids">The identifiers.</param>
        /// <param name="user">The user.</param>
        /// <returns>IEnumerable&lt;TEntity&gt;.</returns>
        public virtual IEnumerable<TEntity> Get(IEnumerable<TEntityId> ids, ClaimsPrincipal user = null)
        {
            foreach (var id in ids)
            {
                var maybe = _repository.Get(id, user);
                if (maybe.HasValue)
                {
                    yield return maybe.Value;
                }
                else
                {
                    _logger?.Log(new LogEntry(LogLevel.Warning, $"'{typeof(TEntity)}' with id '{id}' was not found.", "Get(ids)"));
                }
            }
        }
    }
}