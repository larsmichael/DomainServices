namespace DomainServices.Abstractions;

using System.Collections.Generic;
using System.Security.Claims;

/// <summary>
///     Interface IGroupedService
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
public interface IGroupedService<out TEntity>
{
    /// <summary>
    ///     Determines whether group exists.
    /// </summary>
    /// <param name="group">The group.</param>
    /// <param name="user">The user.</param>
    /// <returns><c>true</c> if group exists, <c>false</c> otherwise.</returns>
    public bool GroupExists(string group, ClaimsPrincipal? user = null);

    /// <summary>
    ///     Gets entities by group.
    /// </summary>
    /// <param name="group">The group.</param>
    /// <param name="user">The user.</param>
    /// <returns>IEnumerable&lt;TEntity&gt;.</returns>
    IEnumerable<TEntity> GetByGroup(string group, ClaimsPrincipal? user = null);

    /// <summary>
    ///     Gets the entities in each group.
    /// </summary>
    /// <param name="groups">The list of groups</param>
    /// <param name="user">The user.</param>
    /// <returns>IEnumerable&lt;TEntity&gt;.</returns>
    IEnumerable<TEntity> GetByGroups(IEnumerable<string> groups, ClaimsPrincipal? user = null);

    /// <summary>
    ///     Gets the full names by group.
    /// </summary>
    /// <param name="group">The group.</param>
    /// <param name="user">The user.</param>
    /// <returns>IEnumerable&lt;System.String&gt;.</returns>
    IEnumerable<string> GetFullNames(string group, ClaimsPrincipal? user = null);

    /// <summary>
    ///     Gets the full names.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <returns>IEnumerable&lt;System.String&gt;.</returns>
    IEnumerable<string> GetFullNames(ClaimsPrincipal? user = null);
}