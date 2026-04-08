using System.Runtime.CompilerServices;
using Tsikhanau.Monads.Either;
using Tsikhanau.Monads.Optional;
using Tsikhanau.Monads.Result;

namespace Tsikhanau.RailwayExtensions.Conversions;

public static class ResultConversions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Optional<TData> ToOptional<TData, TError>(
        this Result<TData, TError> result)
        where TData : notnull
        where TError : notnull
    {
        return result.IsSuccess
            ? Optional<TData>.Some(result.Value)
            : Optional<TData>.None();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Either<TError, TData> ToEither<TData, TError>(
        this Result<TData, TError> result)
        where TData : notnull
        where TError : notnull
    {
        return result.IsSuccess
            ? Either<TError, TData>.FromRight(result.Value)
            : Either<TError, TData>.FromLeft(result.Error);
    }
}
