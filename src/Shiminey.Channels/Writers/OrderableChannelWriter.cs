namespace Shiminey.Channels.Writers
{
    using System.Threading;
    using System.Threading.Channels;
    using System.Threading.Tasks;
    using Shiminey.Channels.Collections;

    public class OrderableChannelWriter<T> : ChannelWriter<T>
    {
        internal OrderableChannelWriter(ConcurrentOrderableQueue<T> items)
            => this.Items = items;

        private ConcurrentOrderableQueue<T> Items { get; }

        public override bool TryWrite(T item)
            => this.TryWriteOrderable(item, out _);

        public bool TryWriteOrderable(T item, out IOrderableChannelItem orderable)
        {
            orderable = this.Items.Enqueue(item);
            return true;
        }

        public override ValueTask<bool> WaitToWriteAsync(CancellationToken cancellationToken = default)
            => new ValueTask<bool>(true);
    }
}
