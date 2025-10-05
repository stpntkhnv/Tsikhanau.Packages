using Tsikhanau.Outcomes.Result;

namespace Tsikhanau.RailwayExtensions.Result.SideEffects;

public static partial class ResultExtensions
{
    public static Result<TData, TError> OnSuccess<TData, TError>(
        this Result<TData, TError> result,
        Action<TData> action)
        where TData : notnull
        where TError : notnull
    {
        if (result.IsSuccess)
        {
            action(result.Value);
        }
        return result;
    }

    public static Result<TData, TError> OnFailure<TData, TError>(
        this Result<TData, TError> result,
        Action<TError> action)
        where TData : notnull
        where TError : notnull
    {
        if (result.IsFailure)
        {
            action(result.Error);
        }
        return result;
    }
}
