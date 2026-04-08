using Tsikhanau.Monads.Either;

namespace Tsikhanau.RailwayExtensions.Either.Map;

public static partial class EitherExtensions
{
    public static async Task<Either<TNewLeft, TRight>> MapLeftAsync<TLeft, TRight, TNewLeft>(
        this Task<Either<TLeft, TRight>> eitherTask,
        Func<TLeft, TNewLeft> mapper)
    {
        var either = await eitherTask;
        return either.MapLeft(mapper);
    }

    public static async Task<Either<TNewLeft, TRight>> MapLeftAsync<TLeft, TRight, TNewLeft>(
        this Task<Either<TLeft, TRight>> eitherTask,
        Func<TLeft, Task<TNewLeft>> mapperAsync)
    {
        var either = await eitherTask;
        return either.IsLeft
            ? Either<TNewLeft, TRight>.FromLeft(await mapperAsync(either.Left))
            : Either<TNewLeft, TRight>.FromRight(either.Right);
    }

    public static async Task<Either<TNewLeft, TRight>> MapLeftAsync<TLeft, TRight, TNewLeft>(
        this Either<TLeft, TRight> either,
        Func<TLeft, Task<TNewLeft>> mapperAsync)
    {
        return either.IsLeft
            ? Either<TNewLeft, TRight>.FromLeft(await mapperAsync(either.Left))
            : Either<TNewLeft, TRight>.FromRight(either.Right);
    }
}
