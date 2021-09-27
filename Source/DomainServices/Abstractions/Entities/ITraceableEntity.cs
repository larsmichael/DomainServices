namespace DomainServices.Abstractions
{
    using System;

    public interface ITraceableEntity<out TId> : IEntity<TId>
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
}