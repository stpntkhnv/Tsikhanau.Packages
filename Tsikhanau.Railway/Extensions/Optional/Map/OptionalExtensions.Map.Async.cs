namespace Tsikhanau.Railway;

public static partial class OptionalExtensions
{
    public static async Task<Optional<TResult>> MapAsync<T, TResult>(
        this Task<Optional<T>> optionalTask,
        Func<T, TResult> mapper)
    {
        var optional = await optionalTask;
        return optional.Map(mapper);
    }

    public static async Task<Optional<TResult>> MapAsync<T, TResult>(
        this Task<Optional<T>> optionalTask,
        Func<T, Task<TResult>> mapperAsync)
    {
        var optional = await optionalTask;
        return optional.HasValue
            ? Optional<TResult>.Some(await mapperAsync(optional.Value))
            : Optional<TResult>.None();
    }

    public static async Task<Optional<TResult>> MapAsync<T, TResult>(
        this Optional<T> optional,
        Func<T, Task<TResult>> mapperAsync)
    {
        return optional.HasValue
            ? Optional<TResult>.Some(await mapperAsync(optional.Value))
            : Optional<TResult>.None();
    }
}
