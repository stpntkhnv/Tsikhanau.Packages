using Tsikhanau.Monads.Result;

namespace Tsikhanau.RailwayExtensions.Result.Map;

public static partial class ResultExtensions
{
    public static async Task<Result<TData, TNewError>> MapErrorAsync<TData, TError, TNewError>(
        this Task<Result<TData, TError>> resultTask,
        Func<TError, TNewError> errorMapper) where TData : notnull where TError : notnull where TNewError : notnull
    {
        var result = await resultTask;
        return result.MapError(errorMapper);
    }

    public static async Task<Result<TData, TNewError>> MapErrorAsync<TData, TError, TNewError>(
        this Task<Result<TData, TError>> resultTask,
        Func<TError, Task<TNewError>> errorMapperAsync) where TData : notnull where TNewError : notnull where TError : notnull
    {
        var result = await resultTask;
        return result.IsSuccess
            ? Result<TData, TNewError>.Success(result.Value)
            : Result<TData, TNewError>.Failure(await errorMapperAsync(result.Error));
    }

    public static async Task<Result<TData, TNewError>> MapErrorAsync<TData, TError, TNewError>(
        this Result<TData, TError> result,
        Func<TError, Task<TNewError>> errorMapperAsync) where TData : notnull where TNewError : notnull where TError : notnull
    {
        return result.IsSuccess
            ? Result<TData, TNewError>.Success(result.Value)
            : Result<TData, TNewError>.Failure(await errorMapperAsync(result.Error));
    }
}