namespace DomainServices.Abstractions
{
    /// <summary>
    ///     Interface IGroupedEntity
    /// </summary>
    /// <typeparam name="TId">The type of the identifier.</typeparam>
    public interface IGroupedEntity<out TId> : INamedEntity<TId> where TId : notnull
    {
        /// <summary>
        ///     Gets the full name.
        /// </summary>
        /// <value>The full name.</value>
        string FullName { get; }

        /// <summary>
        ///     Gets the group.
        /// </summary>
        /// <value>The group.</value>
        string Group { get; }
    }
}