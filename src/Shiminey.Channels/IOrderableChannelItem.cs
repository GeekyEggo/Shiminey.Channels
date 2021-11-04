namespace Shiminey.Channels.Collections
{
    public interface IOrderableChannelItem
    {
        void MoveBackward();
        void MoveFirst();
        void MoveForward();
        void MoveLast();
    }
}
