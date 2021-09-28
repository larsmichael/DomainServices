namespace DomainServices.Abstractions
{
    using System;
    using System.Security.Claims;

    public interface IGroupedUpdatableService
    {
        /// <summary>
        ///     Removes all entities within the group with the specified identifier.
        /// </summary>
        /// <param name="group">The group identifier.</param>
        /// <param name="user">The user.</param>
        void RemoveByGroup(string group, ClaimsPrincipal? user = null);

        /// <summary>
        ///     Occurs when a group of entities was deleted.
        /// </summary>
        event EventHandler<EventArgs<string>> DeletedGroup;

        /// <summary>
        ///     Occurs when deleting a group of entities.
        /// </summary>
        event EventHandler<CancelEventArgs<string>> DeletingGroup;
    }
}