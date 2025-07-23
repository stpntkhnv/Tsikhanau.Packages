using Tsikhanau.Outcomes.Result;

namespace Tsikhanau.RailwayExtensions.Result.SideEffects;

public static partial class ResultExtensions
{
    public static Result<TData, TError> Tap<TData, TError>(
        this Result<TData, TError> result,
        Action<TData> action)
    {
        if (result.IsSuccess)
        {
            action(result.Value);
        }
         
        return result;
    }
}