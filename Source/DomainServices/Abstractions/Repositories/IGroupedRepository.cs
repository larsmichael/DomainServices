namespace DomainServices.Abstractions
{
    using System.Collections.Generic;
    using System.Security.Claims;

    /// <summary>
    ///     Interface IGroupedRepository
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public interface IGroupedRepository<out TEntity>
    {
        /// <summary>
        ///     Determines whether the repository contains group.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <param name="user">The user.</param>
        /// <returns><c>true</c> if the specified group contains group; otherwise, <c>false</c>.</returns>
        bool ContainsGroup(string group, ClaimsPrincipal user = null);

        /// <summary>
        ///     Gets entities by group.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <param name="user">The user.</param>
        /// <returns>IEnumerable&lt;TEntity&gt;.</returns>
        IEnumerable<TEntity> GetByGroup(string group, ClaimsPrincipal user = null);

        /// <summary>
        ///     Gets the full names by group.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <param name="user">The user.</param>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        IEnumerable<string> GetFullNames(string group, ClaimsPrincipal user = null);

        /// <summary>
        ///     Gets the full names.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        IEnumerable<string> GetFullNames(ClaimsPrincipal user = null);
    }
}