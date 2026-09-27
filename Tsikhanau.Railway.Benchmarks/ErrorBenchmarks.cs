using BenchmarkDotNet.Attributes;

namespace Tsikhanau.Railway.Benchmarks;

[MemoryDiagnoser]
public class ErrorBenchmarks
{
    private readonly Error _error = Error.Create("INNER", "Inner operation failed");

    private readonly String _code = "NOT_FOUND";
    private readonly String _message = "Item was not found";

    [Benchmark(Baseline = true)]
    public PlainError PlainErrorObject() => new(_code, _message);

    [Benchmark]
    public Error Create() => Error.Create(_code, _message);

    [Benchmark]
    public Error WithContext() => _error.WithContext(_code, _message);

    [Benchmark]
    public Result<Int32, Error> Failure() => Result.Failure<Int32>(_error);

    public sealed record PlainError(String Code, String Message);
}
