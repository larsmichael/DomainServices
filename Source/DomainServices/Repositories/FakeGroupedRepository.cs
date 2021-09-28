namespace DomainServices.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using Abstractions;

    /// <summary>
    ///     In-memory implementation of a grouped, discrete and updatable repository. To be used in for example unit tests.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TEntityId">The type of the entity identifier.</typeparam>
    public class FakeGroupedRepository<TEntity, TEntityId> : FakeRepository<TEntity, TEntityId>, IGroupedRepository<TEntity> where TEntity : IEntity<TEntityId>, ICloneable, IGroupedEntity<TEntityId>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="FakeGroupedRepository{TEntity,TEntityId}" /> class.
        /// </summary>
        public FakeGroupedRepository()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="FakeGroupedRepository{TEntity,TEntityId}" /> class.
        /// </summary>
        /// <param name="entities">A collection of entities for priming the repository.</param>
        public FakeGroupedRepository(IEnumerable<TEntity> entities)
            : base(entities)
        {
        }

        /// <summary>
        ///     Determines whether the repository contains the specified group.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <param name="user">The user.</param>
        /// <returns><c>true</c> if the specified group contains group; otherwise, <c>false</c>.</returns>
        public bool ContainsGroup(string group, ClaimsPrincipal? user = null)
        {
            return _entities.Any(e => e.Value.Group == group);
        }

        /// <summary>
        ///     Gets entities by group.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <param name="user">The user.</param>
        /// <returns>IEnumerable&lt;TEntity&gt;.</returns>
        public IEnumerable<TEntity> GetByGroup(string group, ClaimsPrincipal? user = null)
        {
            return _entities.Where(e => e.Value.Group == group).Select(e => (TEntity)e.Value.Clone()).ToList();
        }

        /// <summary>
        ///     Gets the full names by group.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <param name="user">The user.</param>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        public IEnumerable<string> GetFullNames(string group, ClaimsPrincipal? user = null)
        {
            return GetByGroup(group).Select(e => e.FullName).ToList();
        }

        /// <summary>
        ///     Gets the full names.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        public IEnumerable<string> GetFullNames(ClaimsPrincipal? user = null)
        {
            return _entities.Select(e => e.Value.FullName).ToList();
        }
    }
}