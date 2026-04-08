using System.Runtime.CompilerServices;
using Tsikhanau.Monads.Either;
using Tsikhanau.Monads.Optional;
using Tsikhanau.Monads.Result;

namespace Tsikhanau.RailwayExtensions.Conversions;

public static class OptionalConversions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T, TError> ToResult<T, TError>(
        this Optional<T> optional,
        TError error)
        where T : notnull
        where TError : notnull
    {
        return optional.HasValue
            ? Result<T, TError>.Success(optional.Value)
            : Result<T, TError>.Failure(error);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T, TError> ToResult<T, TError>(
        this Optional<T> optional,
        Func<TError> errorFactory)
        where T : notnull
        where TError : notnull
    {
        return optional.HasValue
            ? Result<T, TError>.Success(optional.Value)
            : Result<T, TError>.Failure(errorFactory());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Either<TLeft, T> ToEither<T, TLeft>(
        this Optional<T> optional,
        TLeft left)
    {
        return optional.HasValue
            ? Either<TLeft, T>.FromRight(optional.Value)
            : Either<TLeft, T>.FromLeft(left);
    }
}
