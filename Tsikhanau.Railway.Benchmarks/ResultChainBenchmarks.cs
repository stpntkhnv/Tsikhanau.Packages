using BenchmarkDotNet.Attributes;

namespace Tsikhanau.Railway.Benchmarks;

[MemoryDiagnoser]
public class ResultChainBenchmarks
{
    private static readonly Error StartError = Error.Create("START", "Start failed");
    private static readonly Error NotPositive = Error.Create("NOT_POSITIVE", "Value must be positive");
    private static readonly Error TooLarge = Error.Create("TOO_LARGE", "Value is too large");

    private static Int32 _sink;

    private readonly Int32 _input = 42;

    [ParamsAllValues]
    public Boolean Fails { get; set; }

    [Benchmark(Baseline = true)]
    public Int32 PlainIfElse()
    {
        if (Fails)
        {
            return -1;
        }

        if (_input <= 0)
        {
            return -1;
        }

        var incremented = _input * 2 + 1;

        if (incremented >= 1000)
        {
            return -1;
        }

        _sink = incremented;
        return incremented;
    }

    [Benchmark]
    public Int32 RailwayChain() =>
        Start()
            .Bind(static x => x > 0 ? Result.Success(x * 2) : Result.Failure<Int32>(NotPositive))
            .Map(static x => x + 1)
            .Ensure(static x => x < 1000, TooLarge)
            .Tap(static x => _sink = x)
            .Match(static x => x, static _ => -1);

    private Result<Int32, Error> Start() =>
        Fails ? Result.Failure<Int32>(StartError) : Result.Success(_input);
}
