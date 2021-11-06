namespace Shiminey.Channels
{
    using System.Threading.Channels;
    using Shiminey.Channels.Collections;
    using Shiminey.Channels.Readers;
    using Shiminey.Channels.Writers;

    /// <summary>
    /// An <see cref="UnboundedChannel{T}"/> that contains data whose order can be mutated.
    /// </summary>
    public class UnboundedOrderableChannel<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnboundedOrderableChannel{T}"/> class.
        /// </summary>
        internal UnboundedOrderableChannel()
        {
            var items = new ConcurrentOrderableQueue<T>();

            this.Reader = new OrderableChannelReader<T>(items);
            this.Writer = new OrderableChannelWriter<T>(items);
        }

        /// <summary>
        /// Gets the readable half of this channel.
        /// </summary>
        public ChannelReader<T> Reader { get; }

        /// <summary>
        /// Gets the writable half of this channel.
        /// </summary>
        public OrderableChannelWriter<T> Writer { get; }
    }
}
