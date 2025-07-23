using Tsikhanau.Outcomes.Result;

namespace Tsikhanau.RailwayExtensions.Result.GetValue;

public static partial class ResultExtensions
{
    public static async Task<TOutput> MatchAsync<TData, TError, TOutput>(
        this Task<Result<TData, TError>> resultTask,
        Func<TData, TOutput> onSuccess,
        Func<TError, TOutput> onFailure)
        where TData : notnull 
        where TError : notnull
    {
        var result = await resultTask;
        return result.Match(onSuccess, onFailure);
    }

    public static async Task<TOutput> MatchAsync<TData, TError, TOutput>(
        this Task<Result<TData, TError>> resultTask,
        Func<TData, Task<TOutput>> onSuccessAsync,
        Func<TError, Task<TOutput>> onFailureAsync) 
        where TData : notnull 
        where TError : notnull
    {
        var result = await resultTask;
        return result.IsSuccess
            ? await onSuccessAsync(result.Value)
            : await onFailureAsync(result.Error);
    }

    public static async Task<TOutput> MatchAsync<TData, TError, TOutput>(
        this Result<TData, TError> result,
        Func<TData, Task<TOutput>> onSuccessAsync,
        Func<TError, Task<TOutput>> onFailureAsync) 
        where TData : notnull
        where TError : notnull
    {
        return result.IsSuccess
            ? await onSuccessAsync(result.Value)
            : await onFailureAsync(result.Error);
    }
}