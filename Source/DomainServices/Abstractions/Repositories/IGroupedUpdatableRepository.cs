namespace DomainServices.Abstractions
{
    using System.Security.Claims;

    public interface IGroupedUpdatableRepository
    {
        /// <summary>
        ///     Removes all entities within the group with the specified identifier.
        /// </summary>
        /// <param name="group">The group identifier.</param>
        /// <param name="user">The user.</param>
        void RemoveByGroup(string group, ClaimsPrincipal? user = null);
    }
}