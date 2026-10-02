using BenchmarkDotNet.Attributes;

namespace Tsikhanau.Railway.Benchmarks;

[MemoryDiagnoser]
public class ResultCollectTwoBenchmarks
{
    private readonly Result<Int32> _r1 = Result.Success(1);
    private readonly Result<Int32> _r2 = Result.Success(2);

    [Benchmark(Baseline = true)]
    public Int32 Plain() => _r1.IsSuccess && _r2.IsSuccess ? _r1.Value + _r2.Value : -1;

    [Benchmark]
    public Int32 Collect() =>
        Result
            .Collect(_r1)
            .And(_r2)
            .Map(static (a, b) => a + b)
            .Match(static value => value, static _ => -1);

    [Benchmark]
    public Int32 CombineAllWorkaround() =>
        Result
            .CombineAll(_r1.Map(static _ => Unit.Value), _r2.Map(static _ => Unit.Value))
            .Map(_ => _r1.Value + _r2.Value)
            .Match(static value => value, static _ => -1);
}
