using System.Threading;
using System.Threading.Tasks;

namespace AndroidSDK.PlatformTools
{
    public static class CancellationTokenExtensions
    {
        public static Task AsTask(this CancellationToken? cancellationToken)
        {
            if (cancellationToken == null)
                return null;
            else
                return AsTask((CancellationToken)cancellationToken);
        }

        public static Task AsTask(this CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return Task.FromCanceled(cancellationToken);
            }
            else
            {
                var taskCompletionSource = new TaskCompletionSource<bool>();
                cancellationToken.Register(() => taskCompletionSource.TrySetCanceled(cancellationToken), useSynchronizationContext: false);
                return taskCompletionSource.Task;
            }
        }
    }
}
