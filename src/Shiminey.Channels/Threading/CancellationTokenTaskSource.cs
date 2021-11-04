namespace Shiminey.Channels.Threading
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides an infinite-awaitable task that can only be cancelled via the <see cref="CancellationToken"/>.
    /// </summary>
    public sealed class CancellationTokenTaskSource : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CancellationTokenTaskSource"/> class.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        public CancellationTokenTaskSource(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                this.Task = Task.FromCanceled(cancellationToken);
            }
            else
            {
                var tcs = new TaskCompletionSource<object>();

                this.Registration = cancellationToken.Register(() => tcs.TrySetCanceled(cancellationToken), useSynchronizationContext: false);
                this.Task = tcs.Task;
            }
        }

        /// <summary>
        /// Gets the task to be awaited.
        /// </summary>
        public Task Task { get; }

        /// <summary>
        /// Gets the <see cref="CancellationTokenRegistration"/>.
        /// </summary>
        private IDisposable Registration { get; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
            => this.Registration?.Dispose();
    }
}
