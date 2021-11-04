namespace Shiminey.Channels
{
    using System.Threading.Channels;
    using Shiminey.Channels.Collections;
    using Shiminey.Channels.Readers;
    using Shiminey.Channels.Writers;

    public class UnboundedOrderableChannel<T>
    {
        internal UnboundedOrderableChannel()
        {
            var items = new ConcurrentOrderableQueue<T>();

            this.Reader = new OrderableChannelReader<T>(items);
            this.Writer = new OrderableChannelWriter<T>(items);
        }

        public ChannelReader<T> Reader { get; protected set; }
        public OrderableChannelWriter<T> Writer { get; protected set; }
    }
}
