using Tsikhanau.Outcomes.Result;

namespace Tsikhanau.RailwayExtensions.Result;

public static class ResultExtensions
{
    public static Result<TResult, TError> Map<TData, TError, TResult>(
        this Result<TData, TError> result,
        Func<TData, TResult> mapper)
    {
        return result.IsSuccess
            ? Result<TResult, TError>.Success(mapper(result.Value))
            : Result<TResult, TError>.Failure(result.Error);
    }

    public static Result<TResult, TError> Bind<TData, TError, TResult>(
        this Result<TData, TError> result,
        Func<TData, Result<TResult, TError>> binder)
    {
        return result.IsSuccess
            ? binder(result.Value)
            : Result<TResult, TError>.Failure(result.Error);
    }

    public static Result<TData, TError> OnSuccess<TData, TError>(
        this Result<TData, TError> result,
        Action<TData> action)
    {
        if (result.IsSuccess)
        {
            action(result.Value);
        }
        return result;
    }

    public static Result<TData, TError> OnFailure<TData, TError>(
        this Result<TData, TError> result,
        Action<TError> action)
    {
        if (result.IsFailure)
        {
            action(result.Error);
        }
        return result;
    }

    public static Result<TData, TError> Ensure<TData, TError>(
        this Result<TData, TError> result,
        Func<TData, Boolean> predicate,
        TError error)
    {
        if (result.IsFailure)
        {
            return result;
        }

        if (predicate(result.Value))
        {
            return result;
        }

        return Result<TData, TError>.Failure(error);
    }

    public static TOutput Match<TData, TError, TOutput>(
        this Result<TData, TError> result,
        Func<TData, TOutput> onSuccess,
        Func<TError, TOutput> onFailure)
    {
        return result.IsSuccess
            ? onSuccess(result.Value)
            : onFailure(result.Error);
    }

    public static Result<TData, TNewError> MapError<TData, TError, TNewError>(
        this Result<TData, TError> result,
        Func<TError, TNewError> errorMapper)
    {
        return result.IsSuccess
            ? Result<TData, TNewError>.Success(result.Value)
            : Result<TData, TNewError>.Failure(errorMapper(result.Error));
    }

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

    public static Result<TData, TError> TapError<TData, TError>(
        this Result<TData, TError> result,
        Action<TError> action)
    {
        if (result.IsFailure)
        {
            action(result.Error);
        }
        
        return result;
    }

    public static TData GetValueOrDefault<TData, TError>(
        this Result<TData, TError> result,
        TData defaultValue = default!) =>
        result.IsSuccess ? result.Value : defaultValue;

    public static TData GetValueOrDefault<TData, TError>(
        this Result<TData, TError> result,
        Func<TData> defaultValueFactory) =>
        result.IsSuccess ? result.Value : defaultValueFactory();

    public static Result<(TData1, TData2), TError> Zip<TData1, TData2, TError>(
        this Result<TData1, TError> first,
        Result<TData2, TError> second)
    {
        if (first.IsFailure)
            return Result<(TData1, TData2), TError>.Failure(first.Error);
        
        if (second.IsFailure)
            return Result<(TData1, TData2), TError>.Failure(second.Error);

        return Result<(TData1, TData2), TError>.Success((first.Value, second.Value));
    }
}