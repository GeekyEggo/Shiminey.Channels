namespace Shiminey.Channels.Tests
{
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Shiminey.Channels;
    using Shiminey.Channels.Collections;

    /// <summary>
    /// Provides tests for <see cref="UnboundedOrderableChannel{T}"/>.
    /// </summary>
    [TestFixture]
    public class UnboundedOrderableChannelTests
    {
        /// <summary>
        /// Tests writing, reading, and re-ordering the channel.
        /// </summary>
        [Test]
        public async Task WriteRead()
        {
            var readerWaiter = new TaskCompletionSource<bool>();
            var controllerWaiter = new TaskCompletionSource<IChannelItemController>();

            // Given.
            var expected = new[] { "One", "Three", "Two", "Four" };
            var channel = Channel.CreateUnboundedOrderableChannel<string>();

            var assertWaiter = Task.Factory.StartNew(async () =>
            {
                readerWaiter.TrySetResult(true);

                // Then
                for (var i = 0; i < expected.Length; i++)
                {
                    var item = await channel.Reader.ReadAsync();
                    Assert.AreEqual(expected[i], item);

                    var controller = await controllerWaiter.Task;
                    controller.TryMoveForward();
                }
            }, TaskCreationOptions.RunContinuationsAsynchronously);

            // When.
            await readerWaiter.Task;
            await channel.Writer.WaitToWriteAsync();
            channel.Writer.TryWrite("One");
            channel.Writer.TryWrite("Two");
            channel.Writer.TryWriteOrderable("Three", out var threeController);
            controllerWaiter.TrySetResult(threeController);

            await Task.Delay(250);
            channel.Writer.TryWrite("Four");

            await assertWaiter;
        }

        /// <summary>
        /// Tests the README example.
        /// </summary>
        [Test]
        public async Task Example()
        {
            // Create a channel, and write two items to it.
            var channel = Channel.CreateUnboundedOrderableChannel<string>();
            channel.Writer.TryWrite("One");
            channel.Writer.TryWriteOrderable("Two", out var twoController);

            // The controller allows us to relocate the item within the channel.
            twoController.TryMoveFirst();

            string item;
            item = await channel.Reader.ReadAsync(); // Two
            Assert.AreEqual("Two", item);

            item = await channel.Reader.ReadAsync(); // One
            Assert.AreEqual("One", item);
        }
    }
}
