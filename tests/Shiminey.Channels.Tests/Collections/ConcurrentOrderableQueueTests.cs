namespace Shiminey.Channels.Tests.Collections
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Shiminey.Channels.Collections;

    /// <summary>
    /// Provides tests for <see cref="ConcurrentOrderableQueue{T}"/>.
    /// </summary>
    [TestFixture]
    public class ConcurrentOrderableQueueTests
    {
        /// <summary>
        /// Tests <see cref="ConcurrentOrderableQueue{T}.Clear"/>.
        /// </summary>
        [Test]
        public void Clear()
        {
            // Given.
            var queue = new ConcurrentOrderableQueue<string>();
            queue.Enqueue("One");
            queue.Enqueue("Two");
            queue.Enqueue("Three");
            Assert.AreEqual(3, queue.Count);

            // When, then.
            queue.Clear();
            Assert.AreEqual(0, queue.Count);
        }
        /// <summary>
        /// Tests <see cref="ConcurrentOrderableQueue{T}.Enqueue(T)"/>, <see cref="ConcurrentOrderableQueue{T}.Count"/>, and <see cref="ConcurrentOrderableQueue{T}.TryDequeue(out T)"/>.
        /// </summary>
        [Test]
        public void EnqueueAndTryDequeue()
        {
            // Given.
            var queue = new ConcurrentOrderableQueue<string>();
            queue.Enqueue("One");
            queue.Enqueue("Two");
            queue.Enqueue("Three");

            // When, then.
            Assert.AreEqual(3, queue.Count);

            Assert.IsTrue(queue.TryDequeue(out var item));
            Assert.AreEqual("One", item);

            Assert.IsTrue(queue.TryDequeue(out item));
            Assert.AreEqual("Two", item);

            Assert.IsTrue(queue.TryDequeue(out item));
            Assert.AreEqual("Three", item);

            Assert.IsFalse(queue.TryDequeue(out item));
            Assert.IsNull(item);
        }

        /// <summary>
        /// Tests <see cref="ConcurrentOrderableQueue{T}.WaitToReadAsync(CancellationToken)"/>.
        /// </summary>
        [Test]
        public async Task WaitToReadAsync()
        {
            // Given.
            var queue = new ConcurrentOrderableQueue<string>();
            var enqueueRunning = new TaskCompletionSource();
            _ = Task.Run(async () =>
            {
                enqueueRunning.TrySetResult();
                await Task.Delay(250);
                queue.Enqueue("One");

            });

            await enqueueRunning.Task;

            // When, then.
            await queue.WaitToReadAsync();
            Assert.AreEqual(1, queue.Count);
        }

        /// <summary>
        /// Tests <see cref="ConcurrentOrderableQueue{T}.WaitToReadAsync(CancellationToken)"/> throws <see cref="OperationCanceledException"/> when cancelled.
        /// </summary>
        [Test]
        public void WaitToReadAsync_EmptyWithCancellation()
        {
            // Given.
            Assert.ThrowsAsync<OperationCanceledException>(async () =>
            {
                var queue = new ConcurrentOrderableQueue<string>();
                using (var cts = new CancellationTokenSource())
                {
                    // When, then.
                    cts.Cancel();
                    await queue.WaitToReadAsync(cts.Token);
                }
            });
        }

        /// <summary>
        /// Tests <see cref="ConcurrentOrderableQueue{T}.WaitToReadAsync(CancellationToken)"/> does not throw when the queue is not empty, despite being cancelled.
        /// </summary>
        [Test]
        public void WaitToReadAsync_NonEmptyWithCancellation()
        {
            // Given.
            Assert.DoesNotThrowAsync(async () =>
            {
                var queue = new ConcurrentOrderableQueue<string>();
                queue.Enqueue("One");

                using (var cts = new CancellationTokenSource())
                {
                    // When, then.
                    cts.Cancel();
                    await queue.WaitToReadAsync(cts.Token);
                }
            });
        }
    }
}
