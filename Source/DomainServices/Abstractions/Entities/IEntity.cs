﻿namespace DomainServices.Abstractions
{
    using System.Collections.Generic;

    /// <summary>
    ///     Interface IEntity
    /// </summary>
    /// <typeparam name="TId">The type of the identifier.</typeparam>
    public interface IEntity<out TId>
    {
        /// <summary>
        ///     Gets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        TId Id { get; }

        /// <summary>
        ///     Gets the metadata.
        /// </summary>
        /// <value>The metadata.</value>
        IDictionary<object, object> Metadata { get; }
    }
}