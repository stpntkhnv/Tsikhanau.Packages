using BenchmarkDotNet.Attributes;

namespace Tsikhanau.Railway.Benchmarks;

[MemoryDiagnoser]
public class ResultAsyncChainBenchmarks
{
    private static readonly Error StartError = Error.Failure("START", "Start failed");
    private static readonly Error NotPositive = Error.Failure("NOT_POSITIVE", "Value must be positive");
    private static readonly Error TooLarge = Error.Failure("TOO_LARGE", "Value is too large");

    private static Int32 _sink;

    private readonly Int32 _input = 42;

    [ParamsAllValues]
    public Boolean Fails { get; set; }

    [Benchmark(Baseline = true)]
    public async Task<Int32> PlainAsync()
    {
        if (Fails)
        {
            return -1;
        }

        if (_input <= 0)
        {
            return -1;
        }

        var doubled = await Task.FromResult(_input * 2);
        var incremented = await Task.FromResult(doubled + 1);

        if (!await Task.FromResult(incremented < 1000))
        {
            return -1;
        }

        _sink = incremented;
        await Task.CompletedTask;

        return await Task.FromResult(incremented);
    }

    [Benchmark]
    public Task<Int32> RailwayChainAsync() =>
        Start()
            .BindAsync(static x => Task.FromResult(x > 0 ? Result.Success(x * 2) : Result.Failure<Int32>(NotPositive)))
            .MapAsync(static x => Task.FromResult(x + 1))
            .EnsureAsync(static x => Task.FromResult(x < 1000), TooLarge)
            .TapAsync(static x =>
            {
                _sink = x;
                return Task.CompletedTask;
            })
            .MatchAsync(static x => Task.FromResult(x), static _ => Task.FromResult(-1));

    private Result<Int32> Start() =>
        Fails ? Result.Failure<Int32>(StartError) : Result.Success(_input);
}
