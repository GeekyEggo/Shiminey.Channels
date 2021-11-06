namespace Shiminey.Channels.Collections
{
    /// <summary>
    /// Provides methods for controlling the position of an item within an orderable channel.
    /// </summary>
    public interface IOrderableChannelItem
    {
        /// <summary>
        /// Attempts to decrease the priority of the item by moving it backwards one place.
        /// </summary>
        /// <returns><c>true</c> when the item was moved; otherwise <c>false</c>.</returns>
        bool TryMoveBackward();

        /// <summary>
        /// Attempts to move the item to the front.
        /// </summary>
        /// <returns><c>true</c> when the item was moved; otherwise <c>false</c>.</returns>
        bool TryMoveFirst();

        /// <summary>
        /// Attempts to increase the priority of the item by bring it forward one place.
        /// </summary>
        /// <returns><c>true</c> when the item was moved; otherwise <c>false</c>.</returns>
        bool TryMoveForward();

        /// <summary>
        /// Attempts to move the item the end.
        /// </summary>
        /// <returns><c>true</c> when the item was moved; otherwise <c>false</c>.</returns>
        bool TryMoveLast();
    }
}
