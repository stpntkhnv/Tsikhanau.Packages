using System.Runtime.CompilerServices;

namespace Tsikhanau.Railway;

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
}
