using Tsikhanau.Outcomes.Result;

namespace Tsikhanau.RailwayExtensions.Result.Combine;

public static partial class ResultExtensions
{
    public static async Task<Result<IList<TData>, IList<TError>>> CombineAllAsync<TData, TError>(
        params Task<Result<TData, TError>>[] resultTasks)
    {
        return await CombineAllAsync((IEnumerable<Task<Result<TData, TError>>>)resultTasks);
    }

    public static async Task<Result<IList<TData>, IList<TError>>> CombineAllAsync<TData, TError>(
        IEnumerable<Task<Result<TData, TError>>> resultTasks)
    {
        var results = await Task.WhenAll(resultTasks);
        return CombineAll(results);
    }
}