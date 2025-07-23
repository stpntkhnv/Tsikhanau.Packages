using Tsikhanau.Outcomes.Result;

namespace Tsikhanau.RailwayExtensions.Result.Guards;

public static partial class ResultExtensions
{
    public static Result<TData, TError> Ensure<TData, TError>(
        this Result<TData, TError> result,
        Func<TData, Boolean> predicate,
        TError error)
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