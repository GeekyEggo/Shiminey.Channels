namespace Shiminey.Channels.Collections
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Shiminey.Channels.Threading;

    internal class ConcurrentOrderableQueue<T>
    {
        public int Count
        {
            get
            {
                lock (this.SyncRoot)
                {
                    return this.Queue.Count;
                }
            }
        }
        internal object SyncRoot { get; } = new object();
        private TaskCompletionSource<T> ItemEnqueuedTaskCompletionSource { get; set; } = new TaskCompletionSource<T>();
        private LinkedList<T> Queue { get; } = new LinkedList<T>();

        public void Clear()
        {
            lock (this.SyncRoot)
            {
                this.Queue.Clear();
            }
        }

        public OrderableQueueItem<T> Enqueue(T item)
        {
            lock (this.SyncRoot)
            {
                var enqueuedItem = new OrderableQueueItem<T>(this, this.Queue.AddLast(item));
                this.ItemEnqueuedTaskCompletionSource?.TrySetResult(item);

                return enqueuedItem;
            }
        }

        public bool TryDequeue(out T item)
        {
            lock (this.SyncRoot)
            {
                if (this.Queue.Count == 0)
                {
                    item = default;
                    return false;
                }

                item = this.Queue.First.Value;
                this.Queue.RemoveFirst();

                if (this.Queue.Count == 0
                    && this.ItemEnqueuedTaskCompletionSource.Task.IsCompleted)
                {
                    this.ItemEnqueuedTaskCompletionSource = new TaskCompletionSource<T>();
                }

                return true;
            }
        }

        public async Task WaitPeekAsync(CancellationToken cancellationToken = default)
        {
            Task task;
            lock (this.SyncRoot)
            {
                if (this.Queue.Count > 0)
                {
                    return;
                }

                task = this.ItemEnqueuedTaskCompletionSource.Task;
            }

            using var cts = new CancellationTokenTaskSource(cancellationToken);
            {
                await Task.WhenAny(task, cts.Task)
                    .ConfigureAwait(false);
            }
        }
    }
}
