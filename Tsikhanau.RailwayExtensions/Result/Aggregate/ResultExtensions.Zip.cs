using Tsikhanau.Monads.Result;

namespace Tsikhanau.RailwayExtensions.Result.Aggregate;

public static partial class ResultExtensions
{
    public static Result<(TData1, TData2), TError> Zip<TData1, TData2, TError>(
        this Result<TData1, TError> first,
        Result<TData2, TError> second)
        where TData1 : notnull
        where TData2 : notnull
        where TError : notnull
    {
        if (first.IsFailure)
            return Result<(TData1, TData2), TError>.Failure(first.Error);

        if (second.IsFailure)
            return Result<(TData1, TData2), TError>.Failure(second.Error);

        return Result<(TData1, TData2), TError>.Success((first.Value, second.Value));
    }
}
