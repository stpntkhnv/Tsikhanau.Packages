using BenchmarkDotNet.Attributes;

namespace Tsikhanau.Railway.Benchmarks;

[MemoryDiagnoser]
public class ResultCollectEightBenchmarks
{
    private readonly Result<Int32> _r1 = Result.Success(1);
    private readonly Result<Int32> _r2 = Result.Success(2);
    private readonly Result<Int32> _r3 = Result.Success(3);
    private readonly Result<Int32> _r4 = Result.Success(4);
    private readonly Result<Int32> _r5 = Result.Success(5);
    private readonly Result<Int32> _r6 = Result.Success(6);
    private readonly Result<Int32> _r7 = Result.Success(7);
    private readonly Result<Int32> _r8 = Result.Success(8);

    [Benchmark(Baseline = true)]
    public Int32 Plain() =>
        _r1.IsSuccess && _r2.IsSuccess && _r3.IsSuccess && _r4.IsSuccess
        && _r5.IsSuccess && _r6.IsSuccess && _r7.IsSuccess && _r8.IsSuccess
            ? _r1.Value + _r2.Value + _r3.Value + _r4.Value + _r5.Value + _r6.Value + _r7.Value + _r8.Value
            : -1;

    [Benchmark]
    public Int32 Collect() =>
        Result
            .Collect(_r1)
            .And(_r2)
            .And(_r3)
            .And(_r4)
            .And(_r5)
            .And(_r6)
            .And(_r7)
            .And(_r8)
            .Map(static (a, b, c, d, e, f, g, h) => a + b + c + d + e + f + g + h)
            .Match(static value => value, static _ => -1);

    [Benchmark]
    public Int32 CombineAllWorkaround() =>
        Result
            .CombineAll(
                _r1.Map(static _ => Unit.Value),
                _r2.Map(static _ => Unit.Value),
                _r3.Map(static _ => Unit.Value),
                _r4.Map(static _ => Unit.Value),
                _r5.Map(static _ => Unit.Value),
                _r6.Map(static _ => Unit.Value),
                _r7.Map(static _ => Unit.Value),
                _r8.Map(static _ => Unit.Value))
            .Map(_ => _r1.Value + _r2.Value + _r3.Value + _r4.Value + _r5.Value + _r6.Value + _r7.Value + _r8.Value)
            .Match(static value => value, static _ => -1);
}
