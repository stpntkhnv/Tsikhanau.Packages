using BenchmarkDotNet.Attributes;

namespace Tsikhanau.Railway.Benchmarks;

[MemoryDiagnoser]
public class ResultTryBenchmarks
{
    private static readonly Func<Int32> Succeeds = static () => 42;
    private static readonly Func<Int32> Throws = static () => throw new InvalidOperationException("Operation failed");

    private Func<Int32> _func = Succeeds;

    [ParamsAllValues]
    public Boolean FuncThrows { get; set; }

    [GlobalSetup]
    public void Setup() => _func = FuncThrows ? Throws : Succeeds;

    [Benchmark(Baseline = true)]
    public Int32 PlainTryCatch()
    {
        try
        {
            return _func();
        }
        catch (Exception)
        {
            return -1;
        }
    }

    [Benchmark]
    public Result<Int32> Try() => Result.Try(_func);
}
