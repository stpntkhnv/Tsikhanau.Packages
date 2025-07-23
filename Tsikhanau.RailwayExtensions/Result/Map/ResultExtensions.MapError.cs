using Tsikhanau.Outcomes.Result;

namespace Tsikhanau.RailwayExtensions.Result.Map;

public static partial class ResultExtensions
{
    public static Result<TData, TNewError> MapError<TData, TError, TNewError>(
        this Result<TData, TError> result,
        Func<TError, TNewError> errorMapper)
    {
        return result.IsSuccess
            ? Result<TData, TNewError>.Success(result.Value)
            : Result<TData, TNewError>.Failure(errorMapper(result.Error));
    }
}