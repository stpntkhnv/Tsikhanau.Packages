namespace Tsikhanau.Railway;

public static partial class ResultExtensions
{
    public static TData GetValueOrDefault<TData, TError>(
        this Result<TData, TError> result,
        TData defaultValue = default!)
        where TData : notnull
        where TError : notnull
        =>
        result.IsSuccess ? result.Value : defaultValue;

    public static TData GetValueOrDefault<TData, TError>(
        this Result<TData, TError> result,
        Func<TData> defaultValueFactory)
        where TData : notnull
        where TError : notnull
        =>
        result.IsSuccess ? result.Value : defaultValueFactory();
}
