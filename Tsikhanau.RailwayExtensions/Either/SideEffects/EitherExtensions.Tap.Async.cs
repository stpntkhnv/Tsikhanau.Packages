using Tsikhanau.Monads.Either;

namespace Tsikhanau.RailwayExtensions.Either.SideEffects;

public static partial class EitherExtensions
{
    public static async Task<Either<TLeft, TRight>> TapAsync<TLeft, TRight>(
        this Task<Either<TLeft, TRight>> eitherTask,
        Action<TRight> action)
    {
        var either = await eitherTask;
        return either.Tap(action);
    }

    public static async Task<Either<TLeft, TRight>> TapAsync<TLeft, TRight>(
        this Task<Either<TLeft, TRight>> eitherTask,
        Func<TRight, Task> actionAsync)
    {
        var either = await eitherTask;
        if (either.IsRight)
        {
            await actionAsync(either.Right);
        }
        return either;
    }

    public static async Task<Either<TLeft, TRight>> TapAsync<TLeft, TRight>(
        this Either<TLeft, TRight> either,
        Func<TRight, Task> actionAsync)
    {
        if (either.IsRight)
        {
            await actionAsync(either.Right);
        }
        return either;
    }
}
