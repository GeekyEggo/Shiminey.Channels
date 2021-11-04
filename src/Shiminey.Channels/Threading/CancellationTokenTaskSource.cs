namespace Shiminey.Channels.Threading
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public sealed class CancellationTokenTaskSource : IDisposable
    {
        public CancellationTokenTaskSource(CancellationToken cancellationToken = default)
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

        public Task Task { get; }
        private IDisposable Registration { get; }

        public void Dispose()
            => this.Registration?.Dispose();
    }
}
