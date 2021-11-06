namespace Shiminey.Channels.Collections
{
    using System;
    using System.Collections.Generic;
    using Shiminey.Channels.Extensions;

    /// <summary>
    /// Provides methods for controlling the position of an item within a <see cref="ConcurrentOrderableQueue{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of elements in the <see cref="ConcurrentOrderableQueue{T}"/>.</typeparam>
    internal class ConcurrentOrderableQueueItemController<T> : IOrderableChannelItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrentOrderableQueueItemController{T}"/> class.
        /// </summary>
        /// <param name="parent">The parent <see cref="ConcurrentOrderableQueue{T}"/>.</param>
        /// <param name="node">The node that represents the element in the <see cref="ConcurrentOrderableQueue{T}"/>.</param>
        internal ConcurrentOrderableQueueItemController(ConcurrentOrderableQueue<T> parent, LinkedListNode<T> node)
        {
            this.Node = node;
            this.Parent = parent;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is present in the parent queue.
        /// </summary>
        public bool IsPresent
        {
            get
            {
                lock (this.Parent.SyncRoot)
                {
                    return this.Node.List != null;
                }
            }
        }

        /// <summary>
        /// Gets or sets the node that represents the element in the <see cref="ConcurrentOrderableQueue{T}"/>
        /// </summary>
        private LinkedListNode<T> Node { get; set; }

        /// <summary>
        /// Gets the parent <see cref="ConcurrentOrderableQueue{T}"/>.
        /// </summary>
        private ConcurrentOrderableQueue<T> Parent { get; }

        /// <inheritdoc/>
        public bool TryMoveBackward()
            => TryMove(() => !this.Node.IsLast(), list => list.AddAfter(this.Node.Next, this.Node.Value));

        /// <inheritdoc/>
        public bool TryMoveFirst()
            => TryMove(() => !this.Node.IsFirst(), list => list.AddFirst(this.Node.Value));

        /// <inheritdoc/>
        public bool TryMoveForward()
            => TryMove(() => !this.Node.IsFirst(), list => list.AddBefore(this.Node.Previous, this.Node.Value));

        /// <inheritdoc/>
        public bool TryMoveLast()
            => TryMove(() => !this.Node.IsLast(), list => list.AddLast(this.Node.Value));

        /// <summary>
        /// Attempts to move the item within the parent <see cref="ConcurrentOrderableQueue{T}"/>
        /// </summary>
        /// <param name="predicate">The predicate to be fulfilled prior to moving the element.</param>
        /// <param name="addTo">The delegate responsible for repositioning the element.</param>
        /// <returns><c>true</c> when the element was relocated; otherwise <c>false</c>.</returns>
        private bool TryMove(Func<bool> predicate, Func<LinkedList<T>, LinkedListNode<T>> addTo)
        {
            lock (this.Parent.SyncRoot)
            {
                if (this.Node.List == null
                    || !predicate())
                {
                    return false;
                }

                var oldNode = this.Node;
                this.Node = addTo(oldNode.List);

                oldNode.List.Remove(oldNode);
                return true;
            }
        }
    }
}
