namespace Tsikhanau.Railway;

public static partial class ResultExtensions
{
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