using Tsikhanau.Outcomes.Result;

namespace Tsikhanau.RailwayExtensions.Result.GetValue;

public static partial class ResultExtensions 
{
    public static TData GetValueOrDefault<TData, TError>(
        this Result<TData, TError> result,
        TData defaultValue = default!) =>
        result.IsSuccess ? result.Value : defaultValue;

    public static TData GetValueOrDefault<TData, TError>(
        this Result<TData, TError> result,
        Func<TData> defaultValueFactory) =>
        result.IsSuccess ? result.Value : defaultValueFactory();
}