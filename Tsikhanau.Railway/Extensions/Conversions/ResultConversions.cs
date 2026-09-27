using System.Runtime.CompilerServices;

namespace Tsikhanau.Railway;

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
}
