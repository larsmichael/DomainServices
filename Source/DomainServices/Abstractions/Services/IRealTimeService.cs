namespace DomainServices.Abstractions
{
    using System;
    using System.Threading.Tasks;

    [Obsolete("Will be removed in a future version.")]
    public interface IRealTimeService
    {
        /// <summary>
        ///     Gets a value indicating whether the SignalR hub is enabled.
        /// </summary>
        /// <value><c>true</c> if SignalR hub is enabled; otherwise, <c>false</c>.</value>
        bool SignalRHubEnabled { get; }

        /// <summary>
        ///     Connects to the SignalR hub if it is enabled and disconnected.
        /// </summary>
        Task<(bool connected, string message)> TryConnectHub();
    }
}