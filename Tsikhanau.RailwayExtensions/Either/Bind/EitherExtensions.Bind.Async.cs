using Tsikhanau.Monads.Either;

namespace Tsikhanau.RailwayExtensions.Either.Bind;

public static partial class EitherExtensions
{
    public static async Task<Either<TLeft, TResult>> BindAsync<TLeft, TRight, TResult>(
        this Task<Either<TLeft, TRight>> eitherTask,
        Func<TRight, Either<TLeft, TResult>> binder)
    {
        var either = await eitherTask;
        return either.Bind(binder);
    }

    public static async Task<Either<TLeft, TResult>> BindAsync<TLeft, TRight, TResult>(
        this Task<Either<TLeft, TRight>> eitherTask,
        Func<TRight, Task<Either<TLeft, TResult>>> binderAsync)
    {
        var either = await eitherTask;
        return either.IsRight
            ? await binderAsync(either.Right)
            : Either<TLeft, TResult>.FromLeft(either.Left);
    }

    public static async Task<Either<TLeft, TResult>> BindAsync<TLeft, TRight, TResult>(
        this Either<TLeft, TRight> either,
        Func<TRight, Task<Either<TLeft, TResult>>> binderAsync)
    {
        return either.IsRight
            ? await binderAsync(either.Right)
            : Either<TLeft, TResult>.FromLeft(either.Left);
    }
}
