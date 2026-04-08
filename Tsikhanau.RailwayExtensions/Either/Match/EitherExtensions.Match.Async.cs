using Tsikhanau.Monads.Either;

namespace Tsikhanau.RailwayExtensions.Either.Match;

public static partial class EitherExtensions
{
    public static async Task<TResult> MatchAsync<TLeft, TRight, TResult>(
        this Task<Either<TLeft, TRight>> eitherTask,
        Func<TLeft, TResult> onLeft,
        Func<TRight, TResult> onRight)
    {
        var either = await eitherTask;
        return either.Match(onLeft, onRight);
    }

    public static async Task<TResult> MatchAsync<TLeft, TRight, TResult>(
        this Task<Either<TLeft, TRight>> eitherTask,
        Func<TLeft, Task<TResult>> onLeftAsync,
        Func<TRight, Task<TResult>> onRightAsync)
    {
        var either = await eitherTask;
        return either.IsRight
            ? await onRightAsync(either.Right)
            : await onLeftAsync(either.Left);
    }

    public static async Task<TResult> MatchAsync<TLeft, TRight, TResult>(
        this Either<TLeft, TRight> either,
        Func<TLeft, Task<TResult>> onLeftAsync,
        Func<TRight, Task<TResult>> onRightAsync)
    {
        return either.IsRight
            ? await onRightAsync(either.Right)
            : await onLeftAsync(either.Left);
    }
}
