namespace Shiminey.Channels.Collections
{
    using System.Collections.Generic;
    using Shiminey.Channels.Extensions;

    internal class OrderableQueueItem<T> : IOrderableChannelItem
    {
        internal OrderableQueueItem(ConcurrentOrderableQueue<T> parent, LinkedListNode<T> node)
        {
            this.Parent = parent;
            this.Node = node;
        }

        private LinkedListNode<T> Node { get; set; }
        private ConcurrentOrderableQueue<T> Parent { get; }

        public void MoveBackward()
        {
            lock (this.Parent.SyncRoot)
            {
                if (!this.Node.IsLast())
                {
                    var oldNode = this.Node;
                    this.Node = oldNode.List.AddBefore(oldNode.Previous, oldNode.Value);
                    oldNode.List.Remove(oldNode);
                }
            }

        }

        public void MoveFirst()
        {
            lock (this.Parent.SyncRoot)
            {
                if (!this.Node.IsFirst())
                {
                    var oldNode = this.Node;
                    this.Node = oldNode.List.AddFirst(oldNode.Value);
                    oldNode.List.Remove(oldNode);
                }
            }
        }

        public void MoveForward()
        {
            lock (this.Parent.SyncRoot)
            {
                if (!this.Node.IsFirst())
                {
                    var oldNode = this.Node;
                    this.Node = oldNode.List.AddBefore(oldNode.Previous, oldNode.Value);
                    oldNode.List.Remove(oldNode);
                }
            }
        }

        public void MoveLast()
        {
            lock (this.Parent.SyncRoot)
            {
                if (!this.Node.IsLast())
                {
                    var oldNode = this.Node;
                    this.Node = oldNode.List.AddLast(oldNode.Value);
                    oldNode.List.Remove(oldNode);
                }
            }
        }
    }
}
