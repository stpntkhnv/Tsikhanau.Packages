using Tsikhanau.Outcomes.Result;

namespace Tsikhanau.RailwayExtensions.Result.FirstSuccess;

public static partial class ResultExtensions
{
    public static async Task<Result<TData, TError>> FirstSuccessAsync<TData, TError>(
        params Task<Result<TData, TError>>[] resultTasks)
    {
        return await FirstSuccessAsync((IEnumerable<Task<Result<TData, TError>>>)resultTasks);
    }

    public static async Task<Result<TData, TError>> FirstSuccessAsync<TData, TError>(
        IEnumerable<Task<Result<TData, TError>>> resultTasks)
    {
        var results = await Task.WhenAll(resultTasks);
        return FirstSuccess(results);
    }
}