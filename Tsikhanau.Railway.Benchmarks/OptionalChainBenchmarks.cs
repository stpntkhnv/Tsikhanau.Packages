using BenchmarkDotNet.Attributes;

namespace Tsikhanau.Railway.Benchmarks;

[MemoryDiagnoser]
public class OptionalChainBenchmarks
{
    private readonly Int32 _input = 42;

    [ParamsAllValues]
    public Boolean IsNone { get; set; }

    [Benchmark(Baseline = true)]
    public Int32 PlainIfElse()
    {
        if (IsNone)
        {
            return -1;
        }

        var doubled = _input * 2;

        if (doubled >= 1000)
        {
            return -1;
        }

        if (doubled <= 10)
        {
            return -1;
        }

        return doubled - 10;
    }

    [Benchmark]
    public Int32 OptionalChain() =>
        Start()
            .Map(static x => x * 2)
            .Where(static x => x < 1000)
            .Bind(static x => x > 10 ? Optional<Int32>.Some(x - 10) : Optional<Int32>.None())
            .Match(static x => x, static () => -1);

    private Optional<Int32> Start() =>
        IsNone ? Optional<Int32>.None() : Optional<Int32>.Some(_input);
}
