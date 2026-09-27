namespace Tsikhanau.Railway;

public static partial class ResultExtensions
{
    public static Result<TData, TNewError> MapError<TData, TError, TNewError>(
        this Result<TData, TError> result,
        Func<TError, TNewError> errorMapper)
        where TData : notnull
        where TError : notnull
        where TNewError : notnull
    {
        return result.IsSuccess
            ? Result<TData, TNewError>.Success(result.Value)
            : Result<TData, TNewError>.Failure(errorMapper(result.Error));
    }
}
