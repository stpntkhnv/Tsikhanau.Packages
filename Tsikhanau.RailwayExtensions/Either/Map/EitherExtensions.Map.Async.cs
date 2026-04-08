using Tsikhanau.Monads.Either;

namespace Tsikhanau.RailwayExtensions.Either.Map;

public static partial class EitherExtensions
{
    public static async Task<Either<TLeft, TResult>> MapAsync<TLeft, TRight, TResult>(
        this Task<Either<TLeft, TRight>> eitherTask,
        Func<TRight, TResult> mapper)
    {
        var either = await eitherTask;
        return either.Map(mapper);
    }

    public static async Task<Either<TLeft, TResult>> MapAsync<TLeft, TRight, TResult>(
        this Task<Either<TLeft, TRight>> eitherTask,
        Func<TRight, Task<TResult>> mapperAsync)
    {
        var either = await eitherTask;
        return either.IsRight
            ? Either<TLeft, TResult>.FromRight(await mapperAsync(either.Right))
            : Either<TLeft, TResult>.FromLeft(either.Left);
    }

    public static async Task<Either<TLeft, TResult>> MapAsync<TLeft, TRight, TResult>(
        this Either<TLeft, TRight> either,
        Func<TRight, Task<TResult>> mapperAsync)
    {
        return either.IsRight
            ? Either<TLeft, TResult>.FromRight(await mapperAsync(either.Right))
            : Either<TLeft, TResult>.FromLeft(either.Left);
    }
}
