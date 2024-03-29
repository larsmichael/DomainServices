﻿namespace DomainServices.Abstractions;

using System;
using System.Security.Claims;

/// <summary>
///     Interface IUpdatableService
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
/// <typeparam name="TEntityId">The type of the entity identifier.</typeparam>
public interface IUpdatableService<TEntity, TEntityId>
    where TEntityId : notnull
    where TEntity : IEntity<TEntityId>
{
    /// <summary>
    ///     Adds the specified entity.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <param name="user">The user.</param>
    void Add(TEntity entity, ClaimsPrincipal? user = null);

    /// <summary>
    ///    Try adding the specified entity without existence check.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <param name="user">The user.</param>
    /// <returns><c>true</c> if entity was successfully added, <c>false</c> otherwise.</returns>
    bool TryAdd(TEntity entity, ClaimsPrincipal? user = null);

    /// <summary>
    ///     Adds or updates the specified entity.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <param name="user">The user.</param>
    void AddOrUpdate(TEntity entity, ClaimsPrincipal? user = null);

    /// <summary>
    ///     Removes the entity with the specified identifier.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <param name="user">The user.</param>
    void Remove(TEntityId id, ClaimsPrincipal? user = null);

    /// <summary>
    ///     Updates the specified entity.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <param name="user">The user.</param>
    void Update(TEntity entity, ClaimsPrincipal? user = null);

    /// <summary>
    ///    Try updating the specified entity without existence check.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <param name="user">The user.</param>
    /// <returns><c>true</c> if entity was successfully updated, <c>false</c> otherwise.</returns>
    bool TryUpdate(TEntity entity, ClaimsPrincipal? user = null);

    /// <summary>
    ///     Occurs when an entity was added.
    /// </summary>
    event EventHandler<EventArgs<TEntity>> Added;

    /// <summary>
    ///     Occurs when adding an entity.
    /// </summary>
    event EventHandler<CancelEventArgs<TEntity>> Adding;

    /// <summary>
    ///     Occurs when an entity was deleted.
    /// </summary>
    event EventHandler<EventArgs<TEntityId>> Deleted;

    /// <summary>
    ///     Occurs when deleting an entity.
    /// </summary>
    event EventHandler<CancelEventArgs<TEntityId>> Deleting;

    /// <summary>
    ///     Occurs when an entity was updated.
    /// </summary>
    event EventHandler<EventArgs<TEntity>> Updated;

    /// <summary>
    ///     Occurs when updating an entity.
    /// </summary>
    event EventHandler<CancelEventArgs<TEntity>> Updating;
}