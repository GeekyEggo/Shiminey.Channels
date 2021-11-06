namespace Shiminey.Channels.Writers
{
    using System.Threading;
    using System.Threading.Channels;
    using System.Threading.Tasks;
    using Shiminey.Channels.Collections;

    /// <summary>
    /// Provides a <see cref="ChannelWriter{T}"/> that contains data whose order can be mutated.
    /// </summary>
    /// <typeparam name="T">Specifies the type of data in the writer.</typeparam>
    public class OrderableChannelWriter<T> : ChannelWriter<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderableChannelWriter{T}"/> class.
        /// </summary>
        /// <param name="items">The items that represent the underlying data source of the channel.</param>
        internal OrderableChannelWriter(ConcurrentOrderableQueue<T> items)
            => this.Items = items;

        /// <summary>
        /// Gets the items that represent the underlying data source of the channel.
        /// </summary>
        private ConcurrentOrderableQueue<T> Items { get; }

        /// <inheritdoc/>
        public override bool TryWrite(T item)
            => this.TryWriteOrderable(item, out _);

        /// <summary>
        /// Attempts to write the specified item to the channel.
        /// </summary>
        /// <param name="item">The item to write.</param>
        /// <param name="controller">The controller that allows for the item to be re-located within the channel.</param>
        /// <returns><c>true</c> if the item was written; otherwise <c>false</c>.</returns>
        public bool TryWriteOrderable(T item, out IChannelItemController controller)
        {
            controller = this.Items.Enqueue(item);
            return true;
        }

        /// <inheritdoc/>
        public override ValueTask<bool> WaitToWriteAsync(CancellationToken cancellationToken = default)
            => new ValueTask<bool>(true);
    }
}
