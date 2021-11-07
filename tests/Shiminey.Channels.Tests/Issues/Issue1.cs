namespace Shiminey.Channels.Tests.Issues
{
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Shiminey.Channels.Collections;

    /// <summary>
    /// Provides tests for <see aref="https://github.com/GeekyEggo/Shiminey.Channels/issues/1"/>
    /// </summary>
    [TestFixture]
    public class Issue1
    {
        /// <summary>
        /// Tests that <see cref="ConcurrentOrderableQueue{T}.Clear"/> correctly resets the underlying task of <see cref="ConcurrentOrderableQueue{T}.WaitToReadAsync(System.Threading.CancellationToken)"/>.
        /// </summary>
        [Test]
        public async Task Test()
        {
            // Given.
            var queue = new ConcurrentOrderableQueue<string>();
            queue.Enqueue("foo");
            queue.Enqueue("bar");
            Assert.AreEqual(2, queue.Count);

            // When.
            queue.Clear();

            var writerTaskStarted = new TaskCompletionSource();
            var writerTask = Task.Factory.StartNew(async () =>
            {
                writerTaskStarted.TrySetResult();

                await Task.Delay(500);
                queue.Enqueue("foo bar");
            }, TaskCreationOptions.RunContinuationsAsynchronously);

            await writerTaskStarted.Task;

            // Then.
            await queue.WaitToReadAsync();
            Assert.AreEqual(1, queue.Count);

            await writerTask;
        }
    }
}
