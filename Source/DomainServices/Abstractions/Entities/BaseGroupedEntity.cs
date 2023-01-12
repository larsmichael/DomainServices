namespace DomainServices.Abstractions;

using Authorization;
using System.Collections.Generic;

/// <summary>
/// Abstract base class for a grouped, named entity
/// </summary>
/// <typeparam name="TId">The type of the entity identifier.</typeparam>
public abstract class BaseGroupedEntity<TId> : BaseNamedEntity<TId>, IGroupedEntity<TId> where TId : notnull
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BaseGroupedEntity{TId}"/> class.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="name">The name.</param>
    /// <param name="group">The group.</param>
    /// <param name="metadata">Metadata.</param>
    /// <param name="permissions">Permissions.</param>
    protected BaseGroupedEntity(TId id, string name, string? group, IDictionary<string, object>? metadata = null, IList<Permission>? permissions = null)
        : base(id, name, metadata, permissions)
    {
        Group = group;
    }

    /// <summary>
    /// Gets the full name.
    /// </summary>
    /// <value>The full name.</value>
    public string FullName => new FullName(Group, Name).ToString();

    /// <summary>
    /// Gets the group.
    /// </summary>
    /// <value>The group.</value>
    public string? Group { get; }

    /// <summary>
    /// Returns a <see cref="string" /> that represents this instance.
    /// </summary>
    /// <returns>A <see cref="string" /> that represents this instance.</returns>
    public override string ToString()
    {
        return FullName;
    }
}