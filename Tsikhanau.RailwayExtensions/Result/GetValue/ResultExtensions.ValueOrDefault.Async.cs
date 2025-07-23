using Tsikhanau.Outcomes.Result;

namespace Tsikhanau.RailwayExtensions.Result.GetValue;

public static partial class ResultExtensions
{
    public static async Task<TData> GetValueOrDefaultAsync<TData, TError>(
        this Task<Result<TData, TError>> resultTask,
        TData defaultValue = default!) where TData : notnull where TError : notnull
    {
        var result = await resultTask;
        return result.GetValueOrDefault(defaultValue);
    }

    public static async Task<TData> GetValueOrDefaultAsync<TData, TError>(
        this Task<Result<TData, TError>> resultTask,
        Func<TData> defaultValueFactory) where TError : notnull where TData : notnull
    {
        var result = await resultTask;
        return result.GetValueOrDefault(defaultValueFactory);
    }

    public static async Task<TData> GetValueOrDefaultAsync<TData, TError>(
        this Task<Result<TData, TError>> resultTask,
        Func<Task<TData>> defaultValueFactoryAsync) where TData : notnull where TError : notnull
    {
        var result = await resultTask;
        return result.IsSuccess ? result.Value : await defaultValueFactoryAsync();
    }
}