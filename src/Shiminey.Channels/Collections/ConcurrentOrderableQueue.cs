namespace Shiminey.Channels.Collections
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Shiminey.Channels.Threading;

    /// <summary>
    /// Represents a first-in, first-out collection of objects whose order can be mutated.
    /// </summary>
    /// <typeparam name="T">Specifies the type of elements in the queue.</typeparam>
    internal class ConcurrentOrderableQueue<T> : IEnumerable<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrentOrderableQueue{T}"/> class.
        /// </summary>
        /// <param name="collection">The initial collection.</param>
        public ConcurrentOrderableQueue(params T[] collection)
        {
            foreach (var item in collection)
            {
                this.Items.AddLast(item);
            }
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="ConcurrentOrderableQueue{T}"/>.
        /// </summary>
        public int Count
        {
            get
            {
                lock (this.SyncRoot)
                {
                    return this.Items.Count;
                }
            }
        }

        /// <summary>
        /// Gets the shared synchronization root.
        /// </summary>
        internal object SyncRoot { get; } = new object();

        /// <summary>
        /// Gets the linked list that represents the underlying queue.
        /// </summary>
        private LinkedList<T> Items { get; } = new LinkedList<T>();

        /// <summary>
        /// Gets or sets the the task completion source that is fulfilled when the <see cref="ConcurrentOrderableQueue{T}"/> is not empty.
        /// </summary>
        private TaskCompletionSource<T> NotEmptyTaskCompletionSource { get; set; } = new TaskCompletionSource<T>();

        /// <summary>
        /// Removes all items from the <see cref="ConcurrentOrderableQueue{T}"/>.
        /// </summary>
        public void Clear()
        {
            lock (this.SyncRoot)
            {
                this.Items.Clear();
            }
        }

        /// <summary>
        /// Adds the specified <paramref name="item"/> to the end of the <see cref="ConcurrentOrderableQueue{T}"/>.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <returns>The orderable item that allows for repositioning of the item in the <see cref="ConcurrentOrderableQueue{T}"/>.</returns>
        public ConcurrentOrderableQueueItemController<T> Enqueue(T item)
        {
            lock (this.SyncRoot)
            {
                var enqueuedItem = new ConcurrentOrderableQueueItemController<T>(this, this.Items.AddLast(item));
                this.NotEmptyTaskCompletionSource?.TrySetResult(item);

                return enqueuedItem;
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            lock (this.SyncRoot)
            {
                var enumerator = this.Items.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    yield return enumerator.Current;
                }
            }
        }

        /// <summary>
        /// Removes the item at the beginning of the <see cref="ConcurrentOrderableQueue{T}"/> and copies it to the result parameter.
        /// </summary>
        /// <param name="result">The removed item.</param>
        /// <returns><c>true</c> if the item was successfully removed; <c>false</c> if <see cref="ConcurrentOrderableQueue{T}"/> is empty.</returns>
        public bool TryDequeue(out T result)
        {
            lock (this.SyncRoot)
            {
                if (this.Items.Count == 0)
                {
                    result = default;
                    return false;
                }

                result = this.Items.First.Value;
                this.Items.RemoveFirst();

                if (this.Items.Count == 0
                    && this.NotEmptyTaskCompletionSource.Task.IsCompleted)
                {
                    this.NotEmptyTaskCompletionSource = new TaskCompletionSource<T>();
                }

                return true;
            }
        }

        /// <summary>
        /// Waits the <see cref="ConcurrentOrderableQueue{T}"/> to contain an item which can be read asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The optional cancellation token.</param>
        public async Task WaitToReadAsync(CancellationToken cancellationToken = default)
        {
            Task task;
            lock (this.SyncRoot)
            {
                if (this.Items.Count > 0)
                {
                    return;
                }

                task = this.NotEmptyTaskCompletionSource.Task;
            }

            using var cts = new CancellationTokenTaskSource(cancellationToken);
            {
                await Task.WhenAny(task, cts.Task).ConfigureAwait(false);
                cancellationToken.ThrowIfCancellationRequested();
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="IEnumerator"></see> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
            => this.GetEnumerator();
    }
}
