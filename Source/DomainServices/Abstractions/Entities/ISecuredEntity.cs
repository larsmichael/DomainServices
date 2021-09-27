namespace DomainServices.Abstractions
{
    using System.Collections.Generic;
    using System.Security.Claims;
    using Authorization;

    /// <summary>
    ///     Interface ISecuredEntity
    /// </summary>
    /// <typeparam name="TId">The type of the identifier.</typeparam>
    public interface ISecuredEntity<out TId> : ITraceableEntity<TId>
    {
        /// <summary>
        ///     Gets the permissions.
        /// </summary>
        IList<Permission> Permissions { get; }

        /// <summary>
        ///     Determines whether the specified principals are allowed to perform the specified operation.
        /// </summary>
        /// <param name="principals">The principals.</param>
        /// <param name="operation">The operation.</param>
        /// <returns>
        ///     <c>true</c> if the specified principals are allowed to perform the specified operation; otherwise,
        ///     <c>false</c>.
        /// </returns>
        bool IsAllowed(HashSet<string> principals, string operation);

        /// <summary>
        ///     Determines whether the specified principals are allowed to perform the specified operation.
        /// </summary>
        /// <param name="principals">The principals.</param>
        /// <param name="operation">The operation.</param>
        /// <returns>
        ///     <c>true</c> if the specified principals are allowed to perform the specified operation; otherwise,
        ///     <c>false</c>.
        /// </returns>
        bool IsAllowed(IEnumerable<string> principals, string operation);

        /// <summary>
        ///     Determines whether the specified user is allowed to perform the specified operation                        .
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="operation">The operation.</param>
        /// <returns><c>true</c> if the specified user is allowed to perform the specified operation; otherwise, <c>false</c>.</returns>
        bool IsAllowed(ClaimsPrincipal user, string operation);

        /// <summary>
        ///     Adds a permission.
        /// </summary>
        /// <param name="principals">The principals.</param>
        /// <param name="operation">The operation.</param>
        /// <param name="permissionType">Type of the permission.</param>
        void AddPermission(IEnumerable<string> principals, string operation, PermissionType permissionType = PermissionType.Allowed);

        /// <summary>
        ///     Adds multiple permissions.
        /// </summary>
        /// <param name="principals">The principals.</param>
        /// <param name="operations">The operations.</param>
        /// <param name="permissionType">Type of the permission.</param>
        void AddPermissions(IEnumerable<string> principals, IEnumerable<string> operations, PermissionType permissionType = PermissionType.Allowed);
    }
}