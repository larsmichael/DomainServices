namespace DomainServices.Abstractions;

/// <summary>
///     Interface INamedEntity
/// </summary>
/// <typeparam name="TId">The type of the identifier.</typeparam>
public interface INamedEntity<out TId> : ISecuredEntity<TId> where TId : notnull
{
    /// <summary>
    ///     Gets the name.
    /// </summary>
    /// <value>The name.</value>
    string Name { get; }
}