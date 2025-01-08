namespace MattEland.MadDataScience.Helpers;

public static class ThreadHelpers
{
    public static Task ContinueOnUIThread<T>(this Task<T> task,
        Action<T>? onSuccess,
        Action<Exception>? onError,
        Action? onFinally)
    {
        return task.ContinueWith(t =>
        {
            if (t.IsFaulted)
            {
                onError?.Invoke(t.Exception?.InnerException ?? t.Exception ?? new Exception("An unknown exception occurred"));
            }
            else
            {
                onSuccess?.Invoke(t.Result);
            }

            onFinally?.Invoke();
        }, TaskScheduler.FromCurrentSynchronizationContext());
    }
    
    public static Task ContinueOnUIThread(this Task task,
        Action? onSuccess,
        Action<Exception>? onError,
        Action? onFinally)
    {
        return task.ContinueWith(t =>
        {
            if (t.IsFaulted)
            {
                onError?.Invoke(t.Exception?.InnerException ?? t.Exception ?? new Exception("An unknown exception occurred"));
            }
            else
            {
                onSuccess?.Invoke();
            }

            onFinally?.Invoke();
        }, TaskScheduler.FromCurrentSynchronizationContext());
    }
}