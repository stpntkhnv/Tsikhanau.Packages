using Tsikhanau.Outcomes.Result;

namespace Tsikhanau.RailwayExtensions.Result.Bind;

public static partial class ResultExtensions
{
    // bind to sync binder
    public static async Task<Result<TResult, TError>> BindAsync<TData, TError, TResult>(
        this Task<Result<TData, TError>> resultTask,
        Func<TData, Result<TResult, TError>> binder) 
        where TResult : notnull 
        where TError : notnull 
        where TData : notnull
    {
        var result = await resultTask;
        return result.Bind(binder);
    }
    
    // bind to async binder
    public static async Task<Result<TResult, TError>> BindAsync<TData, TError, TResult>(
        this Task<Result<TData, TError>> resultTask,
        Func<TData, Task<Result<TResult, TError>>> binderAsync) 
        where TResult : notnull 
        where TError : notnull 
        where TData : notnull
    {
        var result = await resultTask;
        return result.IsSuccess
            ? await binderAsync(result.Value)
            : Result<TResult, TError>.Failure(result.Error);
    }

    // convert to task and bind to async binder
    public static async Task<Result<TResult, TError>> BindAsync<TData, TError, TResult>(
        this Result<TData, TError> result,
        Func<TData, Task<Result<TResult, TError>>> binderAsync) 
        where TResult : notnull 
        where TError : notnull 
        where TData : notnull
    {
        return result.IsSuccess
            ? await binderAsync(result.Value)
            : Result<TResult, TError>.Failure(result.Error);
    }
}