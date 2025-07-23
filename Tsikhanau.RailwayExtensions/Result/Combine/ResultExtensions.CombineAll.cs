using Tsikhanau.Outcomes.Result;

namespace Tsikhanau.RailwayExtensions.Result.Combine;

public static partial class ResultExtensions
{
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
}