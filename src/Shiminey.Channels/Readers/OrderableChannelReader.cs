namespace Shiminey.Channels.Readers
{
    using System.Threading;
    using System.Threading.Channels;
    using System.Threading.Tasks;
    using Shiminey.Channels.Collections;

    public class OrderableChannelReader<T> : ChannelReader<T>
    {
        internal OrderableChannelReader(ConcurrentOrderableQueue<T> items)
            => this.Items = items;

        private ConcurrentOrderableQueue<T> Items { get; }

        public override bool TryRead(out T item)
            => this.Items.TryDequeue(out item);

        public override async ValueTask<bool> WaitToReadAsync(CancellationToken cancellationToken = default)
        {
            await this.Items.WaitPeekAsync(cancellationToken);
            return true;
        }
    }
}
