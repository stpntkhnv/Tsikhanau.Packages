namespace Tsikhanau.Railway;

public static partial class OptionalExtensions
{
    public static async Task<Optional<T>> TapAsync<T>(
        this Task<Optional<T>> optionalTask,
        Action<T> action)
    {
        var optional = await optionalTask;
        return optional.Tap(action);
    }

    public static async Task<Optional<T>> TapAsync<T>(
        this Task<Optional<T>> optionalTask,
        Func<T, Task> actionAsync)
    {
        var optional = await optionalTask;
        if (optional.HasValue)
        {
            await actionAsync(optional.Value);
        }
        return optional;
    }

    public static async Task<Optional<T>> TapAsync<T>(
        this Optional<T> optional,
        Func<T, Task> actionAsync)
    {
        if (optional.HasValue)
        {
            await actionAsync(optional.Value);
        }
        return optional;
    }
}
