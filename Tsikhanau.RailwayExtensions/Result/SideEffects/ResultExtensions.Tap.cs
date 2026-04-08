using Tsikhanau.Monads.Result;

namespace Tsikhanau.RailwayExtensions.Result.SideEffects;

public static partial class ResultExtensions
{
    public static Result<TData, TError> Tap<TData, TError>(
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
}
