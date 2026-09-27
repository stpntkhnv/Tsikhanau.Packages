namespace Tsikhanau.Railway;

public static partial class ResultExtensions
{
    public static Result<TData, TError> FirstSuccess<TData, TError>(
        params Result<TData, TError>[] results)
        where TData : notnull
        where TError : notnull
    {
        return FirstSuccess((IEnumerable<Result<TData, TError>>)results);
    }

    public static Result<TData, TError> FirstSuccess<TData, TError>(
        IEnumerable<Result<TData, TError>> results)
        where TData : notnull
        where TError : notnull
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
