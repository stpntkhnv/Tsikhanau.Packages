using Tsikhanau.Monads.Result;

namespace Tsikhanau.RailwayExtensions.Result.Map;

public static partial class ResultExtensions
{
    // map wrapped value to new value without changing result wrapper
    public static Result<TResult, TError> Map<TData, TError, TResult>(
        this Result<TData, TError> result,
        Func<TData, TResult> mapper) 
        where TResult : notnull 
        where TError : notnull
        where TData : notnull
    {
        return result.IsSuccess
            ? Result<TResult, TError>.Success(mapper(result.Value))
            : Result<TResult, TError>.Failure(result.Error);
    }
}