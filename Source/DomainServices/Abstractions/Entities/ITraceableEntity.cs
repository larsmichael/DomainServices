namespace DomainServices.Abstractions;

using System;

/// <summary>
///     Interface ITraceableEntity
/// </summary>
/// <typeparam name="TId">The type of the identifier.</typeparam>
public interface ITraceableEntity<out TId> : IEntity<TId> where TId : notnull
{
    /// <summary>
    ///     Gets the datetime the entity was added to the repository.
    /// </summary>
    DateTime? Added { get; set; }

    /// <summary>
    ///     Gets the most recent time the entity was updated in the repository.
    /// </summary>
    DateTime? Updated { get; set; }
}