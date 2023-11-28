namespace System.Threading.Tasks;

public static class TaskMixins
{
    public static async void FireAndForget(
        this ValueTask valueTask,
        Action<Exception>? onException = null,
        bool configureAwait = false,
        bool shouldRethrow = false)
    {
        try
        {
            await valueTask.ConfigureAwait(configureAwait);
        }
        catch (Exception ex) when (onException is not null)
        {
            onException(ex);
            if (shouldRethrow) throw;
        }
    }

    public static async void FireAndForget(
        this Task task,
        Action<Exception>? onException = null,
        bool configureAwait = false,
        bool shouldRethrow = false)
    {
        try
        {
            await task.ConfigureAwait(configureAwait);
        }
        catch (Exception ex) when (onException is not null)
        {
            onException(ex);
            if (shouldRethrow) throw;
        }
    }
}
