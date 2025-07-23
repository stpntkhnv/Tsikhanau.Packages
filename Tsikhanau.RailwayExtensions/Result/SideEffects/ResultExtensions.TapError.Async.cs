using Tsikhanau.Outcomes.Result;

namespace Tsikhanau.RailwayExtensions.Result.SideEffects;

public static partial class ResultExtensions
{
    public static async Task<Result<TData, TError>> TapErrorAsync<TData, TError>(
        this Task<Result<TData, TError>> resultTask,
        Action<TError> action) where TData : notnull where TError : notnull
    {
        var result = await resultTask;
        return result.TapError(action);
    }

    public static async Task<Result<TData, TError>> TapErrorAsync<TData, TError>(
        this Task<Result<TData, TError>> resultTask,
        Func<TError, Task> actionAsync) where TError : notnull where TData : notnull
    {
        var result = await resultTask;
        if (result.IsFailure)
        {
            await actionAsync(result.Error);
        }
        return result;
    }

    public static async Task<Result<TData, TError>> TapErrorAsync<TData, TError>(
        this Result<TData, TError> result,
        Func<TError, Task> actionAsync) where TData : notnull where TError : notnull
    {
        if (result.IsFailure)
        {
            await actionAsync(result.Error);
        }
        return result;
    }
}