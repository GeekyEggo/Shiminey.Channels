namespace Shiminey.Channels.Readers
{
    using System.Threading;
    using System.Threading.Channels;
    using System.Threading.Tasks;
    using Shiminey.Channels.Collections;

    /// <summary>
    /// Provides a <see cref="ChannelReader{T}"/> that contains data whose order can be mutated.
    /// </summary>
    /// <typeparam name="T">Specifies the type of data in the reader.</typeparam>
    public class OrderableChannelReader<T> : ChannelReader<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderableChannelReader{T}"/> class.
        /// </summary>
        /// <param name="items">The items that represent the underlying data source of the channel.</param>
        internal OrderableChannelReader(ConcurrentOrderableQueue<T> items)
            => this.Items = items;

        /// <summary>
        /// Gets the items that represent the underlying data source of the channel.
        /// </summary>
        private ConcurrentOrderableQueue<T> Items { get; }

        /// <inheritdoc/>
        public override bool TryRead(out T item)
            => this.Items.TryDequeue(out item);

        /// <inheritdoc/>
        public override async ValueTask<bool> WaitToReadAsync(CancellationToken cancellationToken = default)
        {
            await this.Items.WaitToReadAsync(cancellationToken);
            return true;
        }
    }
}
