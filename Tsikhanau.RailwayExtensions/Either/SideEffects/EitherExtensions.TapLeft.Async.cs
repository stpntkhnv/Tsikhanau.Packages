using Tsikhanau.Monads.Either;

namespace Tsikhanau.RailwayExtensions.Either.SideEffects;

public static partial class EitherExtensions
{
    public static async Task<Either<TLeft, TRight>> TapLeftAsync<TLeft, TRight>(
        this Task<Either<TLeft, TRight>> eitherTask,
        Action<TLeft> action)
    {
        var either = await eitherTask;
        return either.TapLeft(action);
    }

    public static async Task<Either<TLeft, TRight>> TapLeftAsync<TLeft, TRight>(
        this Task<Either<TLeft, TRight>> eitherTask,
        Func<TLeft, Task> actionAsync)
    {
        var either = await eitherTask;
        if (either.IsLeft)
        {
            await actionAsync(either.Left);
        }
        return either;
    }

    public static async Task<Either<TLeft, TRight>> TapLeftAsync<TLeft, TRight>(
        this Either<TLeft, TRight> either,
        Func<TLeft, Task> actionAsync)
    {
        if (either.IsLeft)
        {
            await actionAsync(either.Left);
        }
        return either;
    }
}
