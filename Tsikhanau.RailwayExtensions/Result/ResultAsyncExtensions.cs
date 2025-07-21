using Tsikhanau.Outcomes.Result;

namespace Tsikhanau.RailwayExtensions.Result;

public static class ResultAsyncExtensions
{
    public static async Task<Result<TResult, TError>> MapAsync<TData, TError, TResult>(
        this Task<Result<TData, TError>> resultTask,
        Func<TData, TResult> mapper)
    {
        var result = await resultTask;
        return result.Map(mapper);
    }

    public static async Task<Result<TResult, TError>> MapAsync<TData, TError, TResult>(
        this Task<Result<TData, TError>> resultTask,
        Func<TData, Task<TResult>> mapperAsync)
    {
        var result = await resultTask;
        return result.IsSuccess
            ? Result<TResult, TError>.Success(await mapperAsync(result.Value))
            : Result<TResult, TError>.Failure(result.Error);
    }

    public static async Task<Result<TResult, TError>> MapAsync<TData, TError, TResult>(
        this Result<TData, TError> result,
        Func<TData, Task<TResult>> mapperAsync)
    {
        return result.IsSuccess
            ? Result<TResult, TError>.Success(await mapperAsync(result.Value))
            : Result<TResult, TError>.Failure(result.Error);
    }

    public static async Task<Result<TResult, TError>> BindAsync<TData, TError, TResult>(
        this Task<Result<TData, TError>> resultTask,
        Func<TData, Result<TResult, TError>> binder)
    {
        var result = await resultTask;
        return result.Bind(binder);
    }

    public static async Task<Result<TResult, TError>> BindAsync<TData, TError, TResult>(
        this Task<Result<TData, TError>> resultTask,
        Func<TData, Task<Result<TResult, TError>>> binderAsync)
    {
        var result = await resultTask;
        return result.IsSuccess
            ? await binderAsync(result.Value)
            : Result<TResult, TError>.Failure(result.Error);
    }

    public static async Task<Result<TResult, TError>> BindAsync<TData, TError, TResult>(
        this Result<TData, TError> result,
        Func<TData, Task<Result<TResult, TError>>> binderAsync)
    {
        return result.IsSuccess
            ? await binderAsync(result.Value)
            : Result<TResult, TError>.Failure(result.Error);
    }

    public static async Task<Result<TData, TError>> OnSuccessAsync<TData, TError>(
        this Task<Result<TData, TError>> resultTask,
        Action<TData> action)
    {
        var result = await resultTask;
        return result.OnSuccess(action);
    }

    public static async Task<Result<TData, TError>> OnSuccessAsync<TData, TError>(
        this Task<Result<TData, TError>> resultTask,
        Func<TData, Task> actionAsync)
    {
        var result = await resultTask;
        if (result.IsSuccess)
        {
            await actionAsync(result.Value);
        }
        return result;
    }

    public static async Task<Result<TData, TError>> OnSuccessAsync<TData, TError>(
        this Result<TData, TError> result,
        Func<TData, Task> actionAsync)
    {
        if (result.IsSuccess)
        {
            await actionAsync(result.Value);
        }
        return result;
    }

    public static async Task<Result<TData, TError>> OnFailureAsync<TData, TError>(
        this Task<Result<TData, TError>> resultTask,
        Action<TError> action)
    {
        var result = await resultTask;
        return result.OnFailure(action);
    }

    public static async Task<Result<TData, TError>> OnFailureAsync<TData, TError>(
        this Task<Result<TData, TError>> resultTask,
        Func<TError, Task> actionAsync)
    {
        var result = await resultTask;
        if (result.IsFailure)
        {
            await actionAsync(result.Error);
        }
        return result;
    }

    public static async Task<Result<TData, TError>> OnFailureAsync<TData, TError>(
        this Result<TData, TError> result,
        Func<TError, Task> actionAsync)
    {
        if (result.IsFailure)
        {
            await actionAsync(result.Error);
        }
        return result;
    }

    public static async Task<Result<TData, TError>> EnsureAsync<TData, TError>(
        this Task<Result<TData, TError>> resultTask,
        Func<TData, Boolean> predicate,
        TError error)
    {
        var result = await resultTask;
        return result.Ensure(predicate, error);
    }

    public static async Task<Result<TData, TError>> EnsureAsync<TData, TError>(
        this Task<Result<TData, TError>> resultTask,
        Func<TData, Task<Boolean>> predicateAsync,
        TError error)
    {
        var result = await resultTask;
        return result.IsFailure
            ? result
            : await predicateAsync(result.Value)
                ? result
                : Result<TData, TError>.Failure(error);
    }

    public static async Task<Result<TData, TError>> EnsureAsync<TData, TError>(
        this Result<TData, TError> result,
        Func<TData, Task<Boolean>> predicateAsync,
        TError error)
    {
        return result.IsFailure
            ? result
            : await predicateAsync(result.Value)
                ? result
                : Result<TData, TError>.Failure(error);
    }

    public static async Task<TOutput> MatchAsync<TData, TError, TOutput>(
        this Task<Result<TData, TError>> resultTask,
        Func<TData, TOutput> onSuccess,
        Func<TError, TOutput> onFailure)
    {
        var result = await resultTask;
        return result.Match(onSuccess, onFailure);
    }

    public static async Task<TOutput> MatchAsync<TData, TError, TOutput>(
        this Task<Result<TData, TError>> resultTask,
        Func<TData, Task<TOutput>> onSuccessAsync,
        Func<TError, Task<TOutput>> onFailureAsync)
    {
        var result = await resultTask;
        return result.IsSuccess
            ? await onSuccessAsync(result.Value)
            : await onFailureAsync(result.Error);
    }

    public static async Task<TOutput> MatchAsync<TData, TError, TOutput>(
        this Result<TData, TError> result,
        Func<TData, Task<TOutput>> onSuccessAsync,
        Func<TError, Task<TOutput>> onFailureAsync)
    {
        return result.IsSuccess
            ? await onSuccessAsync(result.Value)
            : await onFailureAsync(result.Error);
    }

    public static async Task<Result<TData, TNewError>> MapErrorAsync<TData, TError, TNewError>(
        this Task<Result<TData, TError>> resultTask,
        Func<TError, TNewError> errorMapper)
    {
        var result = await resultTask;
        return result.MapError(errorMapper);
    }

    public static async Task<Result<TData, TNewError>> MapErrorAsync<TData, TError, TNewError>(
        this Task<Result<TData, TError>> resultTask,
        Func<TError, Task<TNewError>> errorMapperAsync)
    {
        var result = await resultTask;
        return result.IsSuccess
            ? Result<TData, TNewError>.Success(result.Value)
            : Result<TData, TNewError>.Failure(await errorMapperAsync(result.Error));
    }

    public static async Task<Result<TData, TNewError>> MapErrorAsync<TData, TError, TNewError>(
        this Result<TData, TError> result,
        Func<TError, Task<TNewError>> errorMapperAsync)
    {
        return result.IsSuccess
            ? Result<TData, TNewError>.Success(result.Value)
            : Result<TData, TNewError>.Failure(await errorMapperAsync(result.Error));
    }

    public static async Task<Result<TData, TError>> TapAsync<TData, TError>(
        this Task<Result<TData, TError>> resultTask,
        Action<TData> action)
    {
        var result = await resultTask;
        return result.Tap(action);
    }

    public static async Task<Result<TData, TError>> TapAsync<TData, TError>(
        this Task<Result<TData, TError>> resultTask,
        Func<TData, Task> actionAsync)
    {
        var result = await resultTask;
        if (result.IsSuccess)
        {
            await actionAsync(result.Value);
        }
        return result;
    }

    public static async Task<Result<TData, TError>> TapAsync<TData, TError>(
        this Result<TData, TError> result,
        Func<TData, Task> actionAsync)
    {
        if (result.IsSuccess)
        {
            await actionAsync(result.Value);
        }
        return result;
    }

    public static async Task<Result<TData, TError>> TapErrorAsync<TData, TError>(
        this Task<Result<TData, TError>> resultTask,
        Action<TError> action)
    {
        var result = await resultTask;
        return result.TapError(action);
    }

    public static async Task<Result<TData, TError>> TapErrorAsync<TData, TError>(
        this Task<Result<TData, TError>> resultTask,
        Func<TError, Task> actionAsync)
    {
        var result = await resultTask;
        if (result.IsFailure)
        {
            await actionAsync(result.Error);
        }
        return result;
    }

    public static async Task<Result<TData, TError>> TapErrorAsync<TData, TError>(
        this Result<TData, TError> result,
        Func<TError, Task> actionAsync)
    {
        if (result.IsFailure)
        {
            await actionAsync(result.Error);
        }
        return result;
    }

    public static async Task<TData> GetValueOrDefaultAsync<TData, TError>(
        this Task<Result<TData, TError>> resultTask,
        TData defaultValue = default!)
    {
        var result = await resultTask;
        return result.GetValueOrDefault(defaultValue);
    }

    public static async Task<TData> GetValueOrDefaultAsync<TData, TError>(
        this Task<Result<TData, TError>> resultTask,
        Func<TData> defaultValueFactory)
    {
        var result = await resultTask;
        return result.GetValueOrDefault(defaultValueFactory);
    }

    public static async Task<TData> GetValueOrDefaultAsync<TData, TError>(
        this Task<Result<TData, TError>> resultTask,
        Func<Task<TData>> defaultValueFactoryAsync)
    {
        var result = await resultTask;
        return result.IsSuccess ? result.Value : await defaultValueFactoryAsync();
    }
}