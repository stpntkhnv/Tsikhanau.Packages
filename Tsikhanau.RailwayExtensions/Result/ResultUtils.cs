using Tsikhanau.Foundation.General;
using Tsikhanau.Outcomes;
using Tsikhanau.Outcomes.Result;

namespace Tsikhanau.RailwayExtensions;

public static class ResultUtils
{
    public static Result<IEnumerable<TData>, TError> Combine<TData, TError>(
        params Result<TData, TError>[] results)
    {
        return Combine((IEnumerable<Result<TData, TError>>)results);
    }

    public static Result<IEnumerable<TData>, TError> Combine<TData, TError>(
        IEnumerable<Result<TData, TError>> results)
    {
        var resultsList = results.ToList();
        var failure = resultsList.FirstOrDefault(r => r.IsFailure);
        
        if (failure.IsFailure)
        {
            return Result<IEnumerable<TData>, TError>.Failure(failure.Error);
        }

        var values = resultsList.Select(r => r.Value);
        return Result<IEnumerable<TData>, TError>.Success(values);
    }

    public static async Task<Result<IEnumerable<TData>, TError>> CombineAsync<TData, TError>(
        params Task<Result<TData, TError>>[] resultTasks)
    {
        return await CombineAsync((IEnumerable<Task<Result<TData, TError>>>)resultTasks);
    }

    public static async Task<Result<IEnumerable<TData>, TError>> CombineAsync<TData, TError>(
        IEnumerable<Task<Result<TData, TError>>> resultTasks)
    {
        var results = await Task.WhenAll(resultTasks);
        return Combine(results);
    }

    public static Result<IList<TData>, IList<TError>> CombineAll<TData, TError>(
        params Result<TData, TError>[] results)
    {
        return CombineAll((IEnumerable<Result<TData, TError>>)results);
    }

    public static Result<IList<TData>, IList<TError>> CombineAll<TData, TError>(
        IEnumerable<Result<TData, TError>> results)
    {
        var resultsList = results.ToList();
        var successes = new List<TData>();
        var failures = new List<TError>();

        foreach (var result in resultsList)
        {
            if (result.IsSuccess)
            {
                successes.Add(result.Value);
            }
            else
            {
                failures.Add(result.Error);
            }
        }

        return failures.Count == 0
            ? Result<IList<TData>, IList<TError>>.Success(successes)
            : Result<IList<TData>, IList<TError>>.Failure(failures);
    }

    public static async Task<Result<IList<TData>, IList<TError>>> CombineAllAsync<TData, TError>(
        params Task<Result<TData, TError>>[] resultTasks)
    {
        return await CombineAllAsync((IEnumerable<Task<Result<TData, TError>>>)resultTasks);
    }

    public static async Task<Result<IList<TData>, IList<TError>>> CombineAllAsync<TData, TError>(
        IEnumerable<Task<Result<TData, TError>>> resultTasks)
    {
        var results = await Task.WhenAll(resultTasks);
        return CombineAll(results);
    }

    public static Result<Unit, TError> CombineIgnoreValues<TData, TError>(
        params Result<TData, TError>[] results)
    {
        return CombineIgnoreValues((IEnumerable<Result<TData, TError>>)results);
    }

    public static Result<Unit, TError> CombineIgnoreValues<TData, TError>(
        IEnumerable<Result<TData, TError>> results)
    {
        var failure = results.FirstOrDefault(r => r.IsFailure);
        
        return failure.IsFailure
            ? Result<Unit, TError>.Failure(failure.Error)
            : Result<Unit, TError>.Success(Unit.Value);
    }

    public static async Task<Result<Unit, TError>> CombineIgnoreValuesAsync<TData, TError>(
        params Task<Result<TData, TError>>[] resultTasks)
    {
        return await CombineIgnoreValuesAsync((IEnumerable<Task<Result<TData, TError>>>)resultTasks);
    }

    public static async Task<Result<Unit, TError>> CombineIgnoreValuesAsync<TData, TError>(
        IEnumerable<Task<Result<TData, TError>>> resultTasks)
    {
        var results = await Task.WhenAll(resultTasks);
        return CombineIgnoreValues(results);
    }

    public static Result<TData, TError> FirstSuccess<TData, TError>(
        params Result<TData, TError>[] results)
    {
        return FirstSuccess((IEnumerable<Result<TData, TError>>)results);
    }

    public static Result<TData, TError> FirstSuccess<TData, TError>(
        IEnumerable<Result<TData, TError>> results)
    {
        var resultsList = results.ToList();
        var success = resultsList.FirstOrDefault(r => r.IsSuccess);
        
        if (success.IsSuccess)
        {
            return success;
        }

        var lastFailure = resultsList.LastOrDefault();
        return lastFailure.IsFailure
            ? lastFailure
            : Result<TData, TError>.Failure(default(TError)!);
    }

    public static async Task<Result<TData, TError>> FirstSuccessAsync<TData, TError>(
        params Task<Result<TData, TError>>[] resultTasks)
    {
        return await FirstSuccessAsync((IEnumerable<Task<Result<TData, TError>>>)resultTasks);
    }

    public static async Task<Result<TData, TError>> FirstSuccessAsync<TData, TError>(
        IEnumerable<Task<Result<TData, TError>>> resultTasks)
    {
        var results = await Task.WhenAll(resultTasks);
        return FirstSuccess(results);
    }

    public static Result<TData, TError> Try<TData, TError>(
        Func<TData> func,
        Func<Exception, TError> errorMapper)
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

    public static async Task<Result<TData, TError>> TryAsync<TData, TError>(
        Func<Task<TData>> funcAsync,
        Func<Exception, TError> errorMapper)
    {
        try
        {
            var value = await funcAsync();
            return Result<TData, TError>.Success(value);
        }
        catch (Exception ex)
        {
            return Result<TData, TError>.Failure(errorMapper(ex));
        }
    }

    public static Result<TData, Error> Try<TData>(Func<TData> func)
    {
        return Try(func, Error.FromException);
    }

    public static async Task<Result<TData, Error>> TryAsync<TData>(Func<Task<TData>> funcAsync)
    {
        return await TryAsync(funcAsync, Error.FromException);
    }
}