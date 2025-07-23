using Tsikhanau.Outcomes.Result;

namespace Tsikhanau.RailwayExtensions.Result.GetValue;

public static partial class ResultExtensions
{
    public static TOutput Match<TData, TError, TOutput>(
        this Result<TData, TError> result,
        Func<TData, TOutput> onSuccess,
        Func<TError, TOutput> onFailure)
    {
        return result.IsSuccess
            ? onSuccess(result.Value)
            : onFailure(result.Error);
    }
}