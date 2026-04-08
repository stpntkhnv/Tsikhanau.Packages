using System.Runtime.CompilerServices;
using Tsikhanau.Monads.Either;
using Tsikhanau.Monads.Optional;
using Tsikhanau.Monads.Result;

namespace Tsikhanau.RailwayExtensions.Conversions;

public static class EitherConversions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<TRight, TLeft> ToResult<TLeft, TRight>(
        this Either<TLeft, TRight> either)
        where TLeft : notnull
        where TRight : notnull
    {
        return either.IsRight
            ? Result<TRight, TLeft>.Success(either.Right)
            : Result<TRight, TLeft>.Failure(either.Left);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Optional<TRight> ToOptional<TLeft, TRight>(
        this Either<TLeft, TRight> either)
    {
        return either.IsRight
            ? Optional<TRight>.Some(either.Right)
            : Optional<TRight>.None();
    }
}
