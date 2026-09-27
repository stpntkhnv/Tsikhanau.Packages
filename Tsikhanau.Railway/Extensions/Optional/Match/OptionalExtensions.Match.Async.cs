using Tsikhanau.Monads.Optional;

namespace Tsikhanau.RailwayExtensions.Optional.Match;

public static partial class OptionalExtensions
{
    public static async Task<TResult> MatchAsync<T, TResult>(
        this Task<Optional<T>> optionalTask,
        Func<T, TResult> onSome,
        Func<TResult> onNone)
    {
        var optional = await optionalTask;
        return optional.Match(onSome, onNone);
    }

    public static async Task<TResult> MatchAsync<T, TResult>(
        this Task<Optional<T>> optionalTask,
        Func<T, Task<TResult>> onSomeAsync,
        Func<Task<TResult>> onNoneAsync)
    {
        var optional = await optionalTask;
        return optional.HasValue
            ? await onSomeAsync(optional.Value)
            : await onNoneAsync();
    }

    public static async Task<TResult> MatchAsync<T, TResult>(
        this Optional<T> optional,
        Func<T, Task<TResult>> onSomeAsync,
        Func<Task<TResult>> onNoneAsync)
    {
        return optional.HasValue
            ? await onSomeAsync(optional.Value)
            : await onNoneAsync();
    }
}
