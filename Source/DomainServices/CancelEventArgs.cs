namespace DomainServices;

using System.ComponentModel;

/// <summary>
/// Generic CancelEventArgs exposing an item.
/// </summary>
/// <typeparam name="TItem">The type of the item.</typeparam>
public class CancelEventArgs<TItem> : CancelEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CancelEventArgs{TItem}"/> class.
    /// </summary>
    /// <param name="item">The item.</param>
    public CancelEventArgs(TItem item)
    {
        Item = item;
    }

    /// <summary>
    /// Gets the item.
    /// </summary>
    /// <value>The item.</value>
    public TItem Item { get; }
}