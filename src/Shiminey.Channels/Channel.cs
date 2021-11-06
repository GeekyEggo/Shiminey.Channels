namespace Shiminey.Channels
{
    /// <summary>
    /// Provides static methods for creating channels.
    /// </summary>
    public static class Channel
    {
        /// <summary>
        /// Creates an unbounded channel usable by any number of readers and writers concurrently, with a mutable item order.
        /// </summary>
        /// <typeparam name="T">Specifies the type of data in the channel.</typeparam>
        /// <returns>The <see cref="UnboundedOrderableChannel{T}"/>.</returns>
        public static UnboundedOrderableChannel<T> CreateUnboundedOrderableChannel<T>()
            => new UnboundedOrderableChannel<T>();
    }
}
