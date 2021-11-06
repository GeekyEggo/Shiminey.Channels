namespace Shiminey.Channels.Tests.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Shiminey.Channels.Collections;
    using Shiminey.Channels.Tests.Helpers;

    /// <summary>
    /// Provides tests for <see cref="ConcurrentOrderableQueue{T}"/>.
    /// </summary>
    [TestFixture]
    public class ConcurrentOrderableQueueTests
    {
        /// <summary>
        /// Tests <see cref="ConcurrentOrderableQueue{T}.ConcurrentOrderableQueue(T[])"/>.
        /// </summary>
        [Test]
        public void Constructor()
        {
            // Given, when, then.
            var queue = new ConcurrentOrderableQueue<string>("One", "Two", "Three");
            QueueAssert.AreEqual(new[] { "One", "Two", "Three" }, queue);
        }

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
        /// Tests <see cref="ConcurrentOrderableQueue{T}.Enqueue(T)"/>.
        /// </summary>
        [Test]
        public void Enqueue()
        {
            // Given.
            var queue = new ConcurrentOrderableQueue<string>();
            queue.Enqueue("One");
            queue.Enqueue("Two");
            queue.Enqueue("Three");

            // When, then.
            QueueAssert.AreEqual(new[] { "One", "Two", "Three" }, queue);
        }

        /// <summary>
        /// Tests <see cref="IEnumerable{T}.GetEnumerator"/>.
        /// </summary>
        [Test]
        public void Enumerator_Generic()
        {
            // Given.
            var items = new[] { "One", "Two", "Three" };
            var queue = new ConcurrentOrderableQueue<string>(items);

            // When, then.
            var count = 0;
            foreach (var item in (IEnumerable<string>)queue)
            {
                Assert.AreEqual(items[count], item);
                count++;
            }

            Assert.AreEqual(3, count);
        }

        /// <summary>
        /// Tests <see cref="IEnumerable.GetEnumerator"/>.
        /// </summary>
        [Test]
        public void Enumerator_NonGeneric()
        {
            // Given.
            var items = new[] { "One", "Two", "Three" };
            var queue = new ConcurrentOrderableQueue<string>(items);

            // When, then.
            var count = 0;
            foreach (var item in (IEnumerable)queue)
            {
                Assert.AreEqual(items[count], item);
                count++;
            }

            Assert.AreEqual(3, count);
        }

        /// <summary>
        /// Tests <see cref="ConcurrentOrderableQueue{T}.TryDequeue(out T)"/>.
        /// </summary>
        [Test]
        public void TryDequeue()
        {
            // Given.
            var queue = new ConcurrentOrderableQueue<string>("One", "Two", "Three");

            // When.
            var dequeueCount = 0;
            for (var i = queue.Count - 1; i >= 0; i--)
            {
                Assert.IsTrue(queue.TryDequeue(out var _));
                dequeueCount++;
            }

            // Then.
            Assert.AreEqual(3, dequeueCount);
            Assert.AreEqual(0, queue.Count);
            Assert.IsFalse(queue.TryDequeue(out var _));
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
