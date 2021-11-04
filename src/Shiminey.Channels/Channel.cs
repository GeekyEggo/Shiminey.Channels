namespace Shiminey.Channels
{
    public static class Channel
    {
        public static UnboundedOrderableChannel<T> CreateUnboundedOrderableChannel<T>()
            => new UnboundedOrderableChannel<T>();
    }
}
