using Tsikhanau.Foundation.General;
using Tsikhanau.Monads.Result;

namespace Tsikhanau.RailwayExtensions.Result.Combine;

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
        var failure = resultsList.FirstOrDefault(r => r.IsFailure);

        if (failure.IsFailure)
        {
            return Result<IEnumerable<TData>, TError>.Failure(failure.Error);
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
        var failure = results.FirstOrDefault(r => r.IsFailure);

        return failure.IsFailure
            ? Result<Unit, TError>.Failure(failure.Error)
            : Result<Unit, TError>.Success(Unit.Value);
    }
}
