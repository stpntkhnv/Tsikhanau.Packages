using System.Runtime.CompilerServices;
using Tsikhanau.Monads.Either;

namespace Tsikhanau.RailwayExtensions.Either.Map;

public static partial class EitherExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Either<TLeft, TResult> Map<TLeft, TRight, TResult>(
        this Either<TLeft, TRight> either,
        Func<TRight, TResult> mapper)
    {
        return either.IsRight
            ? Either<TLeft, TResult>.FromRight(mapper(either.Right))
            : Either<TLeft, TResult>.FromLeft(either.Left);
    }
}
