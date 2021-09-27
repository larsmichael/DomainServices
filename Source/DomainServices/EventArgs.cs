namespace DomainServices
{
    using System;

    /// <summary>
    /// Generic EventArgs exposing an item.
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    public class EventArgs<TItem> : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventArgs{TItem}"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        public EventArgs(TItem item)
        {
            Item = item;
        }

        /// <summary>
        /// Gets the item.
        /// </summary>
        /// <value>The item.</value>
        public TItem Item { get; }
    }
}