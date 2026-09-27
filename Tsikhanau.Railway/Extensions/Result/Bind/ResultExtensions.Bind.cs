using Tsikhanau.Monads.Result;

namespace Tsikhanau.RailwayExtensions.Result.Bind;

public static partial class ResultExtensions
{
    public static Result<TResult, TError> Bind<TData, TError, TResult>(
        this Result<TData, TError> result,
        Func<TData, Result<TResult, TError>> binder)
        where TData : notnull
        where TError : notnull
        where TResult : notnull
    {
        return result.IsSuccess
            ? binder(result.Value)
            : Result<TResult, TError>.Failure(result.Error);
    }
}
