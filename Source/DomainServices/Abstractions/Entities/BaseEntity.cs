namespace DomainServices.Abstractions;

using System.Text.Json.Serialization;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Ardalis.GuardClauses;
using Authorization;

/// <summary>
///     Abstract base class for an entity
/// </summary>
/// <typeparam name="TId">The type of the entity identifier.</typeparam>
public abstract class BaseEntity<TId> : ISecuredEntity<TId>, ICloneable where TId : notnull
{
    private TId _id;

    /// <summary>
    ///     Initializes a new instance of the <see cref="BaseEntity{TId}" /> class.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="metadata">Metadata.</param>
    /// <param name="permissions">Permissions.</param>
    /// <exception cref="ArgumentNullException">id</exception>
    protected BaseEntity(TId id, IDictionary<string, object>? metadata = null, IList<Permission>? permissions = null) 
    {
        Guard.Against.Null(id, nameof(id));
        _id = id;
        Metadata = metadata ?? new Dictionary<string, object>();
        Permissions = permissions ?? new List<Permission>();
    }

    /// <summary>
    ///     Clones this instance.
    /// </summary>
    public T Clone<T>()
    {
        var writeOptions = new JsonSerializerOptions();
        writeOptions.Converters.Add(new JsonStringEnumConverter());
        var json = JsonSerializer.Serialize(this, typeof(T), writeOptions);
        var readOptions = new JsonSerializerOptions();
        readOptions.Converters.Add(new JsonStringEnumConverter());
        readOptions.Converters.Add(new ObjectToInferredTypeConverter());
        return JsonSerializer.Deserialize<T>(json, readOptions)!;
    }

    /// <summary>
    ///     Gets the permissions.
    /// </summary>
    public virtual IList<Permission> Permissions { get; }

    /// <summary>
    ///     Gets the identifier.
    /// </summary>
    /// <value>The identifier.</value>
    public virtual TId Id
    {
        get => _id;

        protected set => _id = value;
    }

    /// <summary>
    ///     Gets the datetime the entity was added to the repository.
    /// </summary>
    public virtual DateTime? Added { get; set; }

    /// <summary>
    ///     Gets the most recent time the entity was updated in the repository.
    /// </summary>
    public virtual DateTime? Updated { get; set; }

    /// <summary>
    ///     Gets the metadata.
    /// </summary>
    public virtual IDictionary<string, object> Metadata { get; }

    /// <summary>
    ///     Determines whether the specified principals are allowed to perform the specified operation.
    /// </summary>
    /// <param name="principals">The principals.</param>
    /// <param name="operation">The operation.</param>
    /// <returns>
    ///     <c>true</c> if the specified principals are allowed to perform the specified operation; otherwise,
    ///     <c>false</c>.
    /// </returns>
    public bool IsAllowed(HashSet<string> principals, string operation)
    {
        if (Permissions.Any(p => p.Principals.Intersect(principals).Any() &&
                                 p.Operation == operation &&
                                 p.Type == PermissionType.Denied))
        {
            return false;
        }

        return Permissions.Any(p => p.Principals.Intersect(principals).Any() &&
                                    p.Operation == operation &&
                                    p.Type == PermissionType.Allowed);
    }

    /// <summary>
    ///     Determines whether the specified principals are allowed to perform the specified operation.
    /// </summary>
    /// <param name="principals">The principals.</param>
    /// <param name="operation">The operation.</param>
    /// <returns>
    ///     <c>true</c> if the specified principals are allowed to perform the specified operation; otherwise,
    ///     <c>false</c>.
    /// </returns>
    public bool IsAllowed(IEnumerable<string> principals, string operation)
    {
        return IsAllowed(new HashSet<string>(principals), operation);
    }

    /// <summary>
    ///     Determines whether the specified user is allowed to perform the specified operation                        .
    /// </summary>
    /// <param name="user">The user.</param>
    /// <param name="operation">The operation.</param>
    /// <returns><c>true</c> if the specified user is allowed to perform the specified operation; otherwise, <c>false</c>.</returns>
    public bool IsAllowed(ClaimsPrincipal user, string operation)
    {
        return IsAllowed(user.GetPrincipals(), operation);
    }

    /// <summary>
    ///     Adds a permission.
    /// </summary>
    /// <param name="principals">The principals.</param>
    /// <param name="operation">The operation.</param>
    /// <param name="permissionType">Type of the permission.</param>
    public void AddPermission(IEnumerable<string> principals, string operation, PermissionType permissionType = PermissionType.Allowed)
    {
        Permissions.Add(new Permission(principals, operation, permissionType));
    }

    /// <summary>
    ///     Adds multiple permissions.
    /// </summary>
    /// <param name="principals">The principals.</param>
    /// <param name="operations">The operations.</param>
    /// <param name="permissionType">Type of the permission.</param>
    public void AddPermissions(IEnumerable<string> principals, IEnumerable<string> operations, PermissionType permissionType = PermissionType.Allowed)
    {
        principals = principals.ToArray();
        foreach (var operation in operations)
        {
            Permissions.Add(new Permission(principals, operation, permissionType));
        }
    }
}