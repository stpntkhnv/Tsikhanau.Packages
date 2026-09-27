namespace Tsikhanau.Railway;

public static partial class ResultExtensions
{
    public static async Task<Result<TData, TError>> EnsureAsync<TData, TError>(
        this Task<Result<TData, TError>> resultTask,
        Func<TData, Boolean> predicate,
        TError error) 
        where TData : notnull 
        where TError : notnull
    {
        var result = await resultTask;
        return result.Ensure(predicate, error);
    }

    public static async Task<Result<TData, TError>> EnsureAsync<TData, TError>(
        this Task<Result<TData, TError>> resultTask,
        Func<TData, Task<Boolean>> predicateAsync,
        TError error) 
        where TData : notnull
        where TError : notnull
    {
        var result = await resultTask;

        if (result.IsFailure)
        {
            return result;
        }

        if (await predicateAsync(result.Value))
        {
            return result;
        }

        return error;
    }

    public static async Task<Result<TData, TError>> EnsureAsync<TData, TError>(
        this Result<TData, TError> result,
        Func<TData, Task<Boolean>> predicateAsync,
        TError error) 
        where TData : notnull 
        where TError : notnull
    {
        if (result.IsFailure)
        {
            return result;
        }

        if (await predicateAsync(result.Value))
        {
            return result;
        }
        
        return error;
    }
}