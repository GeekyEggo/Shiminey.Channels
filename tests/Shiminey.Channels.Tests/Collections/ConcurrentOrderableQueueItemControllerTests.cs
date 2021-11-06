namespace Shiminey.Channels.Tests.Collections
{
    using NUnit.Framework;
    using Shiminey.Channels.Collections;
    using Shiminey.Channels.Tests.Helpers;

    /// <summary>
    /// Provides tests for <see cref="ConcurrentOrderableQueueItemController{T}"/>.
    /// </summary>
    [TestFixture]
    public class ConcurrentOrderableQueueItemControllerTests
    {
        /// <summary>
        /// Tests <see cref="ConcurrentOrderableQueueItemController{T}.IsPresent"/>
        /// </summary>
        [Test]
        public void IsPresent()
        {
            // Given, when.
            var queue = new ConcurrentOrderableQueue<string>();
            var item = queue.Enqueue("One");

            // Then.
            Assert.IsTrue(item.IsPresent);
        }

        /// <summary>
        /// Tests <see cref="ConcurrentOrderableQueueItemController{T}.TryMoveBackward"/>.
        /// </summary>
        [Test]
        public void TryMoveBackward()
        {
            var queue = new ConcurrentOrderableQueue<string>();
            var items = new[]
            {
                queue.Enqueue("One"),
                queue.Enqueue("Two"),
                queue.Enqueue("Three")
            };

            Assert.IsTrue(items[0].TryMoveBackward());
            QueueAssert.AreEqual(new[] { "Two", "One", "Three" }, queue);
            Assert.IsTrue(items[0].TryMoveBackward());
            QueueAssert.AreEqual(new[] { "Two", "Three", "One" }, queue);
            Assert.IsFalse(items[0].TryMoveBackward());
            QueueAssert.AreEqual(new[] { "Two", "Three", "One" }, queue);
        }

        /// <summary>
        /// Tests <see cref="ConcurrentOrderableQueueItemController{T}.TryMoveFirst"/>.
        /// </summary>
        [Test]
        public void TryMoveFirst()
        {
            var queue = new ConcurrentOrderableQueue<string>();
            var items = new[]
            {
                queue.Enqueue("One"),
                queue.Enqueue("Two"),
                queue.Enqueue("Three")
            };

            Assert.IsTrue(items[2].TryMoveFirst());
            QueueAssert.AreEqual(new[] { "Three", "One", "Two" }, queue);
            Assert.IsFalse(items[2].TryMoveFirst());
            QueueAssert.AreEqual(new[] { "Three", "One", "Two" }, queue);
        }

        /// <summary>
        /// Tests <see cref="ConcurrentOrderableQueueItemController{T}.TryMoveForward"/>.
        /// </summary>
        [Test]
        public void TryMoveForward()
        {
            var queue = new ConcurrentOrderableQueue<string>();
            var items = new[]
            {
                queue.Enqueue("One"),
                queue.Enqueue("Two"),
                queue.Enqueue("Three")
            };

            Assert.IsTrue(items[2].TryMoveForward());
            QueueAssert.AreEqual(new[] { "One", "Three", "Two" }, queue);
            Assert.IsTrue(items[2].TryMoveForward());
            QueueAssert.AreEqual(new[] { "Three", "One", "Two" }, queue);
            Assert.IsFalse(items[2].TryMoveForward());
            QueueAssert.AreEqual(new[] { "Three", "One", "Two" }, queue);
        }

        /// <summary>
        /// Tests <see cref="ConcurrentOrderableQueueItemController{T}.TryMoveLast"/>.
        /// </summary>
        [Test]
        public void TryMoveLast()
        {
            var queue = new ConcurrentOrderableQueue<string>();
            var items = new[]
            {
                queue.Enqueue("One"),
                queue.Enqueue("Two"),
                queue.Enqueue("Three")
            };

            Assert.IsTrue(items[0].TryMoveLast());
            QueueAssert.AreEqual(new[] { "Two", "Three", "One" }, queue);
            Assert.IsFalse(items[0].TryMoveLast());
            QueueAssert.AreEqual(new[] { "Two", "Three", "One" }, queue);
        }

        /// <summary>
        /// Tests the item is flagged as not present when removed, and cannot be relocated.
        /// </summary>
        [Test]
        public void TryMoveDequeued()
        {
            // Given.
            var queue = new ConcurrentOrderableQueue<string>();
            var item = queue.Enqueue("One");

            // When.
            var dequeued = queue.TryDequeue(out var _);

            // Then.
            Assert.IsTrue(dequeued);
            Assert.IsFalse(item.IsPresent);
            Assert.IsFalse(item.TryMoveBackward());
            Assert.IsFalse(item.TryMoveFirst());
            Assert.IsFalse(item.TryMoveForward());
            Assert.IsFalse(item.TryMoveLast());
        }
    }
}
