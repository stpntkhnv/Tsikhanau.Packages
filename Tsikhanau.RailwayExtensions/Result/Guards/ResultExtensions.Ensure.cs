using Tsikhanau.Monads.Result;

namespace Tsikhanau.RailwayExtensions.Result.Guards;

public static partial class ResultExtensions
{
    public static Result<TData, TError> Ensure<TData, TError>(
        this Result<TData, TError> result,
        Func<TData, Boolean> predicate,
        TError error)
        where TData : notnull
        where TError : notnull
    {
        if (result.IsFailure)
        {
            return result;
        }

        if (predicate(result.Value))
        {
            return result;
        }

        return Result<TData, TError>.Failure(error);
    }
}
