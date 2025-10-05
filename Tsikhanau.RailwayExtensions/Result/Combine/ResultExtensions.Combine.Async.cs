using Tsikhanau.Foundation.General;
using Tsikhanau.Outcomes.Result;

namespace Tsikhanau.RailwayExtensions.Result.Combine;

public static partial class ResultExtensions
{
    public static async Task<Result<IEnumerable<TData>, TError>> CombineAsync<TData, TError>(
        params Task<Result<TData, TError>>[] resultTasks)
        where TData : notnull
        where TError : notnull
    {
        return await CombineAsync((IEnumerable<Task<Result<TData, TError>>>)resultTasks);
    }

    public static async Task<Result<IEnumerable<TData>, TError>> CombineAsync<TData, TError>(
        IEnumerable<Task<Result<TData, TError>>> resultTasks)
        where TData : notnull
        where TError : notnull
    {
        var results = await Task.WhenAll(resultTasks);
        return Combine(results);
    }

    public static async Task<Result<Unit, TError>> CombineIgnoreValuesAsync<TData, TError>(
        params Task<Result<TData, TError>>[] resultTasks)
        where TData : notnull
        where TError : notnull
    {
        return await CombineIgnoreValuesAsync((IEnumerable<Task<Result<TData, TError>>>)resultTasks);
    }

    public static async Task<Result<Unit, TError>> CombineIgnoreValuesAsync<TData, TError>(
        IEnumerable<Task<Result<TData, TError>>> resultTasks)
        where TData : notnull
        where TError : notnull
    {
        var results = await Task.WhenAll(resultTasks);
        return CombineIgnoreValues(results);
    }
}
