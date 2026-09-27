using BenchmarkDotNet.Attributes;

namespace Tsikhanau.Railway.Benchmarks;

[MemoryDiagnoser]
public class ResultCombineBenchmarks
{
    private static readonly Error ItemFailed = Error.Failure("ITEM_FAILED", "Item failed");

    private Result<Int32>[] _results = [];
    private Int32[] _values = [];
    private Boolean[] _failed = [];

    [Params(10, 100)]
    public Int32 N { get; set; }

    [ParamsAllValues]
    public Boolean OneFailure { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var failureIndex = OneFailure ? N / 2 : -1;

        _values = Enumerable.Range(0, N).ToArray();
        _failed = Enumerable.Range(0, N).Select(i => i == failureIndex).ToArray();
        _results = Enumerable.Range(0, N)
            .Select(i => i == failureIndex ? Result.Failure<Int32>(ItemFailed) : Result.Success(i))
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public Int32 PlainLoop()
    {
        var sum = 0;

        for (var i = 0; i < _values.Length; i++)
        {
            if (_failed[i])
            {
                return -1;
            }

            sum += _values[i];
        }

        return sum;
    }

    [Benchmark]
    public Int32 Combine() =>
        Result.Combine(_results)
            .Match(static values => values.Sum(), static _ => -1);

    [Benchmark]
    public Int32 CombineAll() =>
        Result.CombineAll(_results)
            .Match(static values => values.Sum(), static _ => -1);
}
