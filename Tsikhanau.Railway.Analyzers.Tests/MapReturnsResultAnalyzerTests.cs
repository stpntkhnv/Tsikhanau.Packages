namespace Tsikhanau.Railway.Analyzers.Tests;

public class MapReturnsResultAnalyzerTests
{
    private const String Service = """
        public static class Users
        {
            public static Result<String> Find(Int32 id) => id.ToString();

            public static Task<Result<String>> FindAsync(Int32 id) => Task.FromResult(Find(id));
        }

        """;

    private static Task VerifyAsync(String source) =>
        Verifier.AnalyzeAsync<MapReturnsResultAnalyzer>(Service + source);

    private static Task FixAsync(String source, String fixedSource) => FixAsync(String.Empty, source, fixedSource);

    private static Task FixAsync(String header, String source, String fixedSource) =>
        Verifier.FixAsync<MapReturnsResultAnalyzer, MapToBindCodeFixProvider>(
            header + Service + source,
            header + Service + fixedSource);

    [Fact]
    public Task Map_MapperReturnsResult_Reports() => VerifyAsync("""
        public class Caller
        {
            public Result<Result<String>> Run(Result<Int32> result) => result.{|TR0002:Map|}(Users.Find);
        }
        """);

    [Fact]
    public Task MapAsync_MapperReturnsResult_ReportsEveryOverload() => VerifyAsync("""
        public class Caller
        {
            public async Task Run(Result<Int32> result, Task<Result<Int32>> pending)
            {
                _ = await pending.{|TR0002:MapAsync|}(id => Users.Find(id));
                _ = await pending.{|TR0002:MapAsync|}(id => Users.FindAsync(id));
                _ = await result.{|TR0002:MapAsync|}(id => Users.FindAsync(id));
            }
        }
        """);

    [Fact]
    public Task MapCalledStatically_Reports() => VerifyAsync("""
        public class Caller
        {
            public Result<Result<String>> Run(Result<Int32> result) => ResultExtensions.{|TR0002:Map|}(result, Users.Find);
        }
        """);

    [Fact]
    public Task MapOnNullableResult_Reports() => VerifyAsync("""
        public class Caller
        {
            public Result<Result<String>>? Run(Result<Int32>? result) => result?.{|TR0002:Map|}(Users.Find);
        }
        """);

    [Fact]
    public Task MapReturningValue_DoesNotReport() => VerifyAsync("""
        public class Caller
        {
            public async Task Run(Result<Int32> result, Task<Result<Int32>> pending)
            {
                _ = result.Map(id => id.ToString());
                _ = await pending.MapAsync(id => Task.FromResult(id * 2));
                _ = result.Bind(Users.Find);
                _ = result.Map(id => Optional<String>.None());
            }
        }
        """);

    [Fact]
    public Task Fix_Map_ReplacesWithBind() => FixAsync("""
        public class Caller
        {
            public void Run(Result<Int32> result)
            {
                var user = result.{|TR0002:Map|}(Users.Find);
            }
        }
        """, """
        public class Caller
        {
            public void Run(Result<Int32> result)
            {
                var user = result.Bind(Users.Find);
            }
        }
        """);

    [Fact]
    public Task Fix_MapAsync_ReplacesWithBindAsync() => FixAsync("""
        public class Caller
        {
            public async Task Run(Task<Result<Int32>> pending)
            {
                var user = await pending.{|TR0002:MapAsync|}(id => Users.FindAsync(id));
            }
        }
        """, """
        public class Caller
        {
            public async Task Run(Task<Result<Int32>> pending)
            {
                var user = await pending.BindAsync(id => Users.FindAsync(id));
            }
        }
        """);

    [Fact]
    public Task Fix_ExplicitTypeArguments_UnwrapsResultType() => FixAsync("""
        public class Caller
        {
            public void Run(Result<Int32> result)
            {
                var user = result.{|TR0002:Map<Int32, Result<String>>|}(Users.Find);
            }
        }
        """, """
        public class Caller
        {
            public void Run(Result<Int32> result)
            {
                var user = result.Bind<Int32, String>(Users.Find);
            }
        }
        """);

    [Fact]
    public Task Fix_QualifiedTypeArgument_UnwrapsResultType() => FixAsync("""
        public class Caller
        {
            public void Run(Result<Int32> result)
            {
                var user = result.{|TR0002:Map<Int32, Tsikhanau.Railway.Result<String>>|}(Users.Find);
            }
        }
        """, """
        public class Caller
        {
            public void Run(Result<Int32> result)
            {
                var user = result.Bind<Int32, String>(Users.Find);
            }
        }
        """);

    [Fact]
    public Task Fix_AliasTypeArgument_DropsTypeArguments()
    {
        const String alias = "using UserResult = Tsikhanau.Railway.Result<System.String>;\n";

        return FixAsync(alias, """
            public class Caller
            {
                public void Run(Result<Int32> result)
                {
                    var user = result.{|TR0002:Map<Int32, UserResult>|}(Users.Find);
                }
            }
            """, """
            public class Caller
            {
                public void Run(Result<Int32> result)
                {
                    var user = result.Bind(Users.Find);
                }
            }
            """);
    }
}
