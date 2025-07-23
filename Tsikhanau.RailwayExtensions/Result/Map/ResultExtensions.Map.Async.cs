using Tsikhanau.Outcomes.Result;

namespace Tsikhanau.RailwayExtensions.Result.Map;

public static partial class ResultExtensions
{
    // map with sync mapper function
    public static async Task<Result<TResult, TError>> MapAsync<TData, TError, TResult>(
        this Task<Result<TData, TError>> resultTask,
        Func<TData, TResult> mapper) 
        where TResult : notnull 
        where TError : notnull 
        where TData : notnull
    {
        var result = await resultTask;
        return result.Map(mapper);
    }
    
    // map with async mapper 
    public static async Task<Result<TResult, TError>> MapAsync<TData, TError, TResult>(
        this Task<Result<TData, TError>> resultTask,
        Func<TData, Task<TResult>> mapperAsync) 
        where TResult : notnull 
        where TError : notnull 
        where TData : notnull
    {
        var result = await resultTask;
        return result.IsSuccess
            ? Result<TResult, TError>.Success(await mapperAsync(result.Value))
            : Result<TResult, TError>.Failure(result.Error);
    }
    
    // map with converting the current result into a task
    public static async Task<Result<TResult, TError>> MapAsync<TData, TError, TResult>(
        this Result<TData, TError> result,
        Func<TData, Task<TResult>> mapperAsync) 
        where TResult : notnull 
        where TError : notnull 
        where TData : notnull
    {
        return result.IsSuccess
            ? Result<TResult, TError>.Success(await mapperAsync(result.Value))
            : Result<TResult, TError>.Failure(result.Error);
    }
}