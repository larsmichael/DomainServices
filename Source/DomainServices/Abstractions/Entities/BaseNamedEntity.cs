﻿namespace DomainServices.Abstractions;

using Authorization;
using System;
using System.Collections.Generic;
using Ardalis.GuardClauses;

/// <summary>
/// Abstract base class for a named entity.
/// </summary>
/// <typeparam name="TId">The type of the entity identifier.</typeparam>
public abstract class BaseNamedEntity<TId> : BaseEntity<TId>, INamedEntity<TId> where TId : notnull
{
    private string _name;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseNamedEntity{TId}"/> class.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="name">The name.</param>
    /// <param name="metadata">Metadata.</param>
    /// <param name="permissions">Permissions.</param>
    /// <exception cref="ArgumentNullException">name</exception>
    protected BaseNamedEntity(TId id, string name, IDictionary<string, object>? metadata = null, IList<Permission>? permissions = null)
        : base(id, metadata, permissions)
    {
        Guard.Against.NullOrEmpty(name, nameof(name));
        _name = name;
    }

    /// <summary>
    /// Gets the name.
    /// </summary>
    /// <value>The name.</value>
    public virtual string Name
    {
        get => _name;

        protected set => _name = value;
    }

    /// <summary>
    /// Returns a <see cref="string" /> that represents this instance.
    /// </summary>
    /// <returns>A <see cref="string" /> that represents this instance.</returns>
    public override string ToString()
    {
        return Name;
    }
}