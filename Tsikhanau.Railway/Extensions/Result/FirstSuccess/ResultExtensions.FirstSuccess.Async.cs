namespace Tsikhanau.Railway;

public static partial class ResultExtensions
{
    public static async Task<Result<TData, TError>> FirstSuccessAsync<TData, TError>(
        params Task<Result<TData, TError>>[] resultTasks)
        where TData : notnull
        where TError : notnull
    {
        return await FirstSuccessAsync((IEnumerable<Task<Result<TData, TError>>>)resultTasks);
    }

    public static async Task<Result<TData, TError>> FirstSuccessAsync<TData, TError>(
        IEnumerable<Task<Result<TData, TError>>> resultTasks)
        where TData : notnull
        where TError : notnull
    {
        var results = await Task.WhenAll(resultTasks);
        return FirstSuccess(results);
    }
}
