using Tsikhanau.Outcomes.Result;

namespace Tsikhanau.RailwayExtensions.Result.FirstSuccess;

public static partial class ResultExtensions
{
    public static Result<TData, TError> FirstSuccess<TData, TError>(
        params Result<TData, TError>[] results)
    {
        return FirstSuccess((IEnumerable<Result<TData, TError>>)results);
    }

    public static Result<TData, TError> FirstSuccess<TData, TError>(
        IEnumerable<Result<TData, TError>> results)
    {
        var resultsList = results.ToList();
        var success = resultsList.FirstOrDefault(r => r.IsSuccess);
        
        if (success.IsSuccess)
        {
            return success;
        }

        var lastFailure = resultsList.LastOrDefault();
        return lastFailure.IsFailure
            ? lastFailure
            : Result<TData, TError>.Failure(default(TError)!);
    }
}