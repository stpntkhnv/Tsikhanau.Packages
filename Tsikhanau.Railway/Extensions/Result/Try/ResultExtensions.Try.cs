using Tsikhanau.Monads.Errors;
using Tsikhanau.Monads.Result;

namespace Tsikhanau.RailwayExtensions.Result.Try;

public static partial class ResultExtensions
{
    public static Result<TData, TError> Try<TData, TError>(
        Func<TData> func,
        Func<Exception, TError> errorMapper)
        where TData : notnull
        where TError : notnull
    {
        try
        {
            return Result<TData, TError>.Success(func());
        }
        catch (Exception ex)
        {
            return Result<TData, TError>.Failure(errorMapper(ex));
        }
    }

    public static Result<TData, Error> Try<TData>(Func<TData> func)
        where TData : notnull
    {
        return Try(func, Error.FromException);
    }
}
