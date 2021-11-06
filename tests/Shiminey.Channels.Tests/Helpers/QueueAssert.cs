namespace Shiminey.Channels.Tests.Helpers
{
    using NUnit.Framework;
    using Shiminey.Channels.Collections;

    /// <summary>
    /// Provides helpers methods for asserting <see cref="ConcurrentOrderableQueue{T}"/>.
    /// </summary>
    internal static class QueueAssert
    {
        /// <summary>
        /// Asserts the <paramref name="actual"/> contains the <paramref name="expected"/> items, in the expected order.
        /// </summary>
        /// <typeparam name="T">The type of elements within the <see cref="ConcurrentOrderableQueue{T}"/>.</typeparam>
        /// <param name="expected">The expected order of elements.</param>
        /// <param name="actual">The queue.</param>
        internal static void AreEqual<T>(T[] expected, ConcurrentOrderableQueue<T> actual)
        {
            var expectedEnumerator = expected.GetEnumerator();
            var actualEnumerator = actual.GetEnumerator();

            while (expectedEnumerator.MoveNext())
            {
                Assert.IsTrue(actualEnumerator.MoveNext());
                Assert.AreEqual(expectedEnumerator.Current, actualEnumerator.Current);
            }

            Assert.IsFalse(actualEnumerator.MoveNext());
        }
    }
}
