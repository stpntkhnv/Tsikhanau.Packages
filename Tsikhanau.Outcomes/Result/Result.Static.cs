using Tsikhanau.Foundation.General;

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
}