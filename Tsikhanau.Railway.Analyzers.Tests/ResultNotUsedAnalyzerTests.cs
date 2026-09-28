namespace Tsikhanau.Railway.Analyzers.Tests;

public class ResultNotUsedAnalyzerTests
{
    private const String Service = """
        public class Service
        {
            public Result<Int32> Save() => 1;

            public Task<Result<Int32>> SaveAsync() => Task.FromResult(Result.Success(1));

            public ValueTask<Result<Int32>> SaveValueAsync() => new(Result.Success(1));

            public Int32 Count() => 1;

            public void Consume(Result<Int32> result) { }

            public Result<Int32> Last { get; } = 1;
        }

        """;

    private static Task VerifyAsync(String source) =>
        Verifier.AnalyzeAsync<ResultNotUsedAnalyzer>(Service + source);

    [Fact]
    public Task IgnoredResult_Reports() => VerifyAsync("""
        public class Caller
        {
            public void Run(Service service)
            {
                {|TR0001:service.Save()|};
            }
        }
        """);

    [Fact]
    public Task IgnoredAwaitedResult_Reports() => VerifyAsync("""
        public class Caller
        {
            public async Task Run(Service service)
            {
                {|TR0001:await service.SaveAsync()|};
                {|TR0001:await service.SaveValueAsync()|};
            }
        }
        """);

    [Fact]
    public Task IgnoredTaskOfResult_Reports() => VerifyAsync("""
        public class Caller
        {
            public void Run(Service service)
            {
                {|TR0001:service.SaveAsync()|};
            }
        }
        """);

    [Fact]
    public Task IgnoredConditionalAccessResult_Reports() => VerifyAsync("""
        public class Caller
        {
            public void Run(Service? service)
            {
                {|TR0001:service?.Save()|};
            }
        }
        """);

    [Fact]
    public Task IgnoredResultInVoidLambda_Reports() => VerifyAsync("""
        public class Caller
        {
            public void Run(Service service, List<Int32> items)
            {
                items.ForEach(_ => {|TR0001:service.Save()|});
            }
        }
        """);

    [Fact]
    public Task IgnoredStaticFactoryResult_Reports() => VerifyAsync("""
        public class Caller
        {
            public void Run()
            {
                {|TR0001:Result.Success()|};
            }
        }
        """);

    [Fact]
    public Task UsedResult_DoesNotReport() => VerifyAsync("""
        public class Caller
        {
            public async Task<Result<Int32>> Run(Service service)
            {
                var saved = service.Save();
                _ = service.Save();
                _ = await service.SaveAsync();
                service.Consume(service.Save());
                service.Count();
                return saved.IsSuccess ? saved : await service.SaveAsync();
            }
        }
        """);

    [Fact]
    public Task TapOnStoredResult_DoesNotReport() => VerifyAsync("""
        public class Caller
        {
            private Result<Int32> _field = 1;

            public async Task Run(Result<Int32> parameter, Task<Result<Int32>> pending)
            {
                var result = Result.Success(1);
                result.Tap(_ => { });
                result.Tap(_ => { }).TapError(_ => { });
                parameter.TapError(_ => { });
                _field.Tap(_ => { });
                new Service().Last.Tap(_ => { });
                await result.TapAsync(_ => Task.CompletedTask);
                await pending.TapErrorAsync(_ => { });
            }
        }
        """);

    [Fact]
    public Task TapOnNewResult_Reports() => VerifyAsync("""
        public class Caller
        {
            public async Task Run(Service service)
            {
                {|TR0001:service.Save().Tap(_ => { })|};
                {|TR0001:await service.SaveAsync().TapAsync(_ => { })|};
            }
        }
        """);

    [Fact]
    public Task UnawaitedTapOnStoredResult_Reports() => VerifyAsync("""
        public class Caller
        {
            public void Run(Result<Int32> result)
            {
                {|TR0001:result.TapAsync(_ => Task.CompletedTask)|};
            }
        }
        """);

    [Fact]
    public Task EnsureOnStoredResult_Reports() => VerifyAsync("""
        public class Caller
        {
            public void Run(Result<Int32> result)
            {
                {|TR0001:result.Ensure(x => x > 0, Error.Failure("negative", "Negative"))|};
            }
        }
        """);

    [Fact]
    public Task CodeWithoutRailwayReference_DoesNotReport() =>
        new Microsoft.CodeAnalysis.CSharp.Testing.CSharpAnalyzerTest<ResultNotUsedAnalyzer, Microsoft.CodeAnalysis.Testing.DefaultVerifier>
        {
            TestCode = """
                public class Caller
                {
                    public int Save() => 1;

                    public void Run() => Save();
                }
                """,
            ReferenceAssemblies = Microsoft.CodeAnalysis.Testing.ReferenceAssemblies.Net.Net100
        }.RunAsync(TestContext.Current.CancellationToken);
}
