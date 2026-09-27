namespace Tsikhanau.Railway;

public static partial class ResultExtensions
{
    public static Result<IEnumerable<TData>, TError> Combine<TData, TError>(
        params Result<TData, TError>[] results)
        where TData : notnull
        where TError : notnull
    {
        return Combine((IEnumerable<Result<TData, TError>>)results);
    }

    public static Result<IEnumerable<TData>, TError> Combine<TData, TError>(
        IEnumerable<Result<TData, TError>> results)
        where TData : notnull
        where TError : notnull
    {
        var resultsList = results.ToList();

        foreach (var result in resultsList)
        {
            if (result.IsFailure)
            {
                return Result<IEnumerable<TData>, TError>.Failure(result.Error);
            }
        }

        var values = resultsList.Select(r => r.Value);
        return Result<IEnumerable<TData>, TError>.Success(values);
    }

    public static Result<Unit, TError> CombineIgnoreValues<TData, TError>(
        params Result<TData, TError>[] results)
        where TData : notnull
        where TError : notnull
    {
        return CombineIgnoreValues((IEnumerable<Result<TData, TError>>)results);
    }

    public static Result<Unit, TError> CombineIgnoreValues<TData, TError>(
        IEnumerable<Result<TData, TError>> results)
        where TData : notnull
        where TError : notnull
    {
        foreach (var result in results)
        {
            if (result.IsFailure)
            {
                return Result<Unit, TError>.Failure(result.Error);
            }
        }

        return Result<Unit, TError>.Success(Unit.Value);
    }
}
