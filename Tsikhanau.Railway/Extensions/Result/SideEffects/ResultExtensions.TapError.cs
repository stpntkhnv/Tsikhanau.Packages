namespace Tsikhanau.Railway;

public static partial class ResultExtensions
{
    public static Result<TData, TError> TapError<TData, TError>(
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
