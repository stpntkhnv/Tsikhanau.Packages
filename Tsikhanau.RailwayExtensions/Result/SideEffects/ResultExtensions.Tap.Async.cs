using Tsikhanau.Outcomes.Result;

namespace Tsikhanau.RailwayExtensions.Result.SideEffects;

public static partial class ResultExtensions
{
    public static async Task<Result<TData, TError>> TapAsync<TData, TError>(
        this Task<Result<TData, TError>> resultTask,
        Action<TData> action) where TData : notnull where TError : notnull
    {
        var result = await resultTask;
        return result.Tap(action);
    }

    public static async Task<Result<TData, TError>> TapAsync<TData, TError>(
        this Task<Result<TData, TError>> resultTask,
        Func<TData, Task> actionAsync) where TError : notnull where TData : notnull
    {
        var result = await resultTask;
        if (result.IsSuccess)
        {
            await actionAsync(result.Value);
        }
        return result;
    }

    public static async Task<Result<TData, TError>> TapAsync<TData, TError>(
        this Result<TData, TError> result,
        Func<TData, Task> actionAsync) where TData : notnull where TError : notnull
    {
        if (result.IsSuccess)
        {
            await actionAsync(result.Value);
        }
        return result;
    }
}