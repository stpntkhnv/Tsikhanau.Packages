using System.Runtime.CompilerServices;
using Tsikhanau.Monads.Either;

namespace Tsikhanau.RailwayExtensions.Either.Map;

public static partial class EitherExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Either<TNewLeft, TRight> MapLeft<TLeft, TRight, TNewLeft>(
        this Either<TLeft, TRight> either,
        Func<TLeft, TNewLeft> mapper)
    {
        return either.IsLeft
            ? Either<TNewLeft, TRight>.FromLeft(mapper(either.Left))
            : Either<TNewLeft, TRight>.FromRight(either.Right);
    }
}
