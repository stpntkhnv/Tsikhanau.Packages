using Tsikhanau.Monads.Result;

namespace Tsikhanau.RailwayExtensions.Result.SideEffects;

public static partial class ResultExtensions
{
    public static async Task<Result<TData, TError>> OnSuccessAsync<TData, TError>(
        this Task<Result<TData, TError>> resultTask,
        Action<TData> action) 
        where TData : notnull 
        where TError : notnull
    {
        var result = await resultTask;
        return result.OnSuccess(action);
    }

    public static async Task<Result<TData, TError>> OnSuccessAsync<TData, TError>(
        this Task<Result<TData, TError>> resultTask,
        Func<TData, Task> actionAsync) 
        where TError : notnull 
        where TData : notnull
    {
        var result = await resultTask;
        if (result.IsSuccess)
        {
            await actionAsync(result.Value);
        }
        return result;
    }

    public static async Task<Result<TData, TError>> OnSuccessAsync<TData, TError>(
        this Result<TData, TError> result,
        Func<TData, Task> actionAsync) 
        where TData : notnull 
        where TError : notnull
    {
        if (result.IsSuccess)
        {
            await actionAsync(result.Value);
        }
        return result;
    }

    public static async Task<Result<TData, TError>> OnFailureAsync<TData, TError>(
        this Task<Result<TData, TError>> resultTask,
        Action<TError> action) 
        where TData : notnull 
        where TError : notnull
    {
        var result = await resultTask;
        return result.OnFailure(action);
    }

    public static async Task<Result<TData, TError>> OnFailureAsync<TData, TError>(
        this Task<Result<TData, TError>> resultTask,
        Func<TError, Task> actionAsync) 
        where TData : notnull 
        where TError : notnull
    {
        var result = await resultTask;
        if (result.IsFailure)
        {
            await actionAsync(result.Error);
        }
        return result;
    }

    public static async Task<Result<TData, TError>> OnFailureAsync<TData, TError>(
        this Result<TData, TError> result,
        Func<TError, Task> actionAsync) 
        where TData : notnull 
        where TError : notnull
    {
        if (result.IsFailure)
        {
            await actionAsync(result.Error);
        }
        return result;
    }
}