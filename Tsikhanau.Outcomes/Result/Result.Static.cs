using Tsikhanau.Foundation.General;
using Tsikhanau.Outcomes.Errors;

namespace Tsikhanau.Outcomes.Result;

public static class Result
{
    public static Result<TData, Error> Success<TData>(TData data)
        => Result<TData, Error>.Success(data);
    
    public static Result<Unit, Error> Success()
        => Result<Unit, Error>.Success(Unit.Value);

    public static Result<TData, Error> Failure<TData>(Error error)
        => Result<TData, Error>.Failure(error);
    
    public static Result<Unit, Error> Failure()
        => Result<Unit, Error>.Failure(Error.Unknown);

    public static Result<TData, Error> ToResult<TData>(this TData data, Func<TData, Boolean> defineSuccess)
    {
        if (defineSuccess(data))
        {
            return Result<TData, Error>.Success(data);
        }
        
        return Result<TData, Error>.Failure(Error.Unknown);
    }
}