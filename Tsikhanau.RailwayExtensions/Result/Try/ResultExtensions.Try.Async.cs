using Tsikhanau.Outcomes.Errors;
using Tsikhanau.Outcomes.Result;

namespace Tsikhanau.RailwayExtensions.Result.Try;

public static partial class ResultExtensions
{
    public static async Task<Result<TData, TError>> TryAsync<TData, TError>(
        Func<Task<TData>> funcAsync,
        Func<Exception, TError> errorMapper)
    {
        try
        {
            var value = await funcAsync();
            return Result<TData, TError>.Success(value);
        }
        catch (Exception ex)
        {
            return Result<TData, TError>.Failure(errorMapper(ex));
        }
    }

    public static async Task<Result<TData, Error>> TryAsync<TData>(Func<Task<TData>> funcAsync)
    {
        return await TryAsync(funcAsync, Error.FromException);
    }
}