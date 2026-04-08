using Tsikhanau.Monads.Optional;

namespace Tsikhanau.RailwayExtensions.Optional.Bind;

public static partial class OptionalExtensions
{
    public static async Task<Optional<TResult>> BindAsync<T, TResult>(
        this Task<Optional<T>> optionalTask,
        Func<T, Optional<TResult>> binder)
    {
        var optional = await optionalTask;
        return optional.Bind(binder);
    }

    public static async Task<Optional<TResult>> BindAsync<T, TResult>(
        this Task<Optional<T>> optionalTask,
        Func<T, Task<Optional<TResult>>> binderAsync)
    {
        var optional = await optionalTask;
        return optional.HasValue
            ? await binderAsync(optional.Value)
            : Optional<TResult>.None();
    }

    public static async Task<Optional<TResult>> BindAsync<T, TResult>(
        this Optional<T> optional,
        Func<T, Task<Optional<TResult>>> binderAsync)
    {
        return optional.HasValue
            ? await binderAsync(optional.Value)
            : Optional<TResult>.None();
    }
}
