namespace DomainServices.Abstractions;

/// <summary>
///     Interface ICloneable
/// </summary>
public interface ICloneable
{
    /// <summary>
    ///     Clones this instance.
    /// </summary>
    public T Clone<T>();
}