namespace Tsikhanau.Railway.Analyzers.Tests;

public class UncheckedResultAccessAnalyzerTests
{
    private const String Helpers = """
        public static class Users
        {
            public static Result<Int32> Find() => 1;

            public static void Use(Object value) { }

            [System.Diagnostics.CodeAnalysis.DoesNotReturn]
            public static void Fail(Error error) => throw new InvalidOperationException(error.Message);

            public static void Refresh(out Result<Int32> result) => result = 1;

            public static Holder Holder() => new();

            public static void ShouldBeTrue([System.Diagnostics.CodeAnalysis.DoesNotReturnIf(false)] this Boolean actual) { }

            public static void ShouldBeFalse([System.Diagnostics.CodeAnalysis.DoesNotReturnIf(true)] this Boolean actual) { }
        }


        public class Holder
        {
            public Result<Int32> Result { get; set; } = 1;
        }

        """;

    private static Task VerifyAsync(String source) =>
        Verifier.AnalyzeAsync<UncheckedResultAccessAnalyzer>(Helpers + source);

    [Fact]
    public Task ValueWithoutCheck_Reports() => VerifyAsync("""
        public class Caller
        {
            public Int32 Run(Result<Int32> result) => {|TR0003:result.Value|};
        }
        """);

    [Fact]
    public Task ErrorWithoutCheck_Reports() => VerifyAsync("""
        public class Caller
        {
            public Error Run(Result<Int32> result) => {|TR0003:result.Error|};
        }
        """);

    [Fact]
    public Task ValueOfInvocation_Reports() => VerifyAsync("""
        public class Caller
        {
            public Int32 Run() => {|TR0003:Users.Find().Value|};
        }
        """);

    [Fact]
    public Task IfIsSuccess_DoesNotReport() => VerifyAsync("""
        public class Caller
        {
            public void Run(Result<Int32> result)
            {
                if (result.IsSuccess)
                {
                    Users.Use(result.Value);
                }
                else
                {
                    Users.Use(result.Error);
                }

                if (result.IsFailure)
                {
                    Users.Use(result.Error);
                }
                else
                {
                    Users.Use(result.Value);
                }
            }
        }
        """);

    [Fact]
    public Task WrongBranch_Reports() => VerifyAsync("""
        public class Caller
        {
            public void Run(Result<Int32> result)
            {
                if (result.IsSuccess)
                {
                    Users.Use({|TR0003:result.Error|});
                }
                else
                {
                    Users.Use({|TR0003:result.Value|});
                }
            }
        }
        """);

    [Fact]
    public Task EarlyReturn_DoesNotReport() => VerifyAsync("""
        public class Caller
        {
            public Result<String> Run(Result<Int32> result)
            {
                if (result.IsFailure)
                {
                    return result.Error;
                }

                return result.Value.ToString();
            }
        }
        """);

    [Fact]
    public Task EarlyThrowWithNegation_DoesNotReport() => VerifyAsync("""
        public class Caller
        {
            public Int32 Run(Result<Int32> result)
            {
                if (!result.IsSuccess)
                {
                    throw new InvalidOperationException(result.Error.Message);
                }

                return result.Value;
            }
        }
        """);

    [Fact]
    public Task AfterIfWithoutExit_Reports() => VerifyAsync("""
        public class Caller
        {
            public Int32 Run(Result<Int32> result)
            {
                if (result.IsSuccess)
                {
                    Users.Use(result.Value);
                }

                return {|TR0003:result.Value|};
            }
        }
        """);

    [Fact]
    public Task ConditionalExpressions_DoNotReport() => VerifyAsync("""
        public class Caller
        {
            public void Run(Result<Int32> result)
            {
                Users.Use(result.IsSuccess ? result.Value : 0);
                Users.Use(result.IsFailure ? result.Error.Code : "");
                Users.Use(result.IsSuccess && result.Value > 0);
                Users.Use(result.IsFailure || result.Value > 0);
                Users.Use(!result.IsSuccess ? 0 : result.Value);
            }
        }
        """);

    [Fact]
    public Task BooleanComparisonsAndPatterns_DoNotReport() => VerifyAsync("""
        public class Caller
        {
            public void Run(Result<Int32> first, Result<Int32> second, Result<Int32> third, Result<Int32> fourth)
            {
                if (first.IsSuccess == false)
                {
                    return;
                }

                if (second.IsSuccess is not true)
                {
                    return;
                }

                if (third is { IsFailure: true })
                {
                    return;
                }

                if (fourth is { IsSuccess: true, Value: > 0 })
                {
                    Users.Use(fourth.Value);
                }

                if (false == fourth.IsSuccess)
                {
                    return;
                }

                if (Environment.Is64BitProcess)
                {
                    Users.Use(0);
                }

                Users.Use(first.Value + second.Value + third.Value + fourth.Value);
            }
        }
        """);

    [Fact]
    public Task NegatedPropertyPattern_DoesNotReport() => VerifyAsync("""
        public class Caller
        {
            public Int32 Run(Result<Int32> result)
            {
                if (result is { IsSuccess: not true })
                {
                    return 0;
                }

                return result.Value;
            }
        }
        """);

    [Fact]
    public Task Reassignment_Reports() => VerifyAsync("""
        public class Caller
        {
            public void Run(Result<Int32> result)
            {
                if (result.IsSuccess)
                {
                    result = Users.Find();
                    Users.Use({|TR0003:result.Value|});
                }

                if (result.IsSuccess)
                {
                    Users.Refresh(out result);
                    Users.Use({|TR0003:result.Value|});
                }

                if (result.IsSuccess)
                {
                    (result, var count) = (Users.Find(), 1);
                    Users.Use({|TR0003:result.Value|});
                }
            }
        }
        """);

    [Fact]
    public Task Loop_DoesNotReport() => VerifyAsync("""
        public class Caller
        {
            public Int32 Run()
            {
                var result = Users.Find();
                while (result.IsFailure)
                {
                    result = Users.Find();
                }

                return result.Value;
            }
        }
        """);

    [Fact]
    public Task Foreach_ChecksEachItem() => VerifyAsync("""
        public class Caller
        {
            public void Run(IEnumerable<Result<Int32>> results)
            {
                foreach (var result in results)
                {
                    if (result.IsFailure)
                    {
                        continue;
                    }

                    Users.Use(result.Value);
                }

                foreach (var result in results)
                {
                    Users.Use({|TR0003:result.Value|});
                }
            }
        }
        """);

    [Fact]
    public Task DoesNotReturnIfAssertion_DoesNotReport() => VerifyAsync("""
        public class Caller
        {
            public Int32 Run(Result<Int32> result, Result<Int32> other)
            {
                Debug.Assert(result.IsSuccess);
                Debug.Assert(!other.IsFailure);
                return result.Value + other.Value;
            }
        }
        """);

    [Fact]
    public Task AssertionExtensions_DoNotReport() => VerifyAsync("""
        public class Caller
        {
            public void Run(Result<Int32> first, Result<Int32> second)
            {
                first.IsSuccess.ShouldBeTrue();
                second.IsSuccess.ShouldBeFalse();
                Users.Use(first.Value);
                Users.Use(second.Error);
                Users.Use({|TR0003:second.Value|});
            }
        }
        """);

    [Fact]
    public Task DoesNotReturnCall_DoesNotReport() => VerifyAsync("""
        public class Caller
        {
            public Int32 Run(Result<Int32> result)
            {
                if (result.IsFailure)
                {
                    Users.Fail(result.Error);
                }

                return result.Value;
            }
        }
        """);

    [Fact]
    public Task Fields_AreTracked() => VerifyAsync("""
        public class Caller
        {
            private Result<Int32> _result = Users.Find();

            public Int32 Checked() => _result.IsSuccess ? _result.Value : 0;

            public Int32 Unchecked() => {|TR0003:this._result.Value|};
        }
        """);

    [Fact]
    public Task PropertyChains_AreTracked() => VerifyAsync("""
        public class Caller
        {
            public void Run(Holder holder, Holder other)
            {
                if (holder.Result.IsSuccess && other.Result.IsSuccess)
                {
                    Users.Use(holder.Result.Value);
                    holder = Users.Holder();
                    Users.Use({|TR0003:holder.Result.Value|});
                    Users.Use(other.Result.Value);
                }
            }
        }
        """);

    [Fact]
    public Task Lambdas_InheritStateFromEnclosingCode() => VerifyAsync("""
        public class Caller
        {
            public void Run(Result<Int32> result, List<Int32> items)
            {
                if (result.IsSuccess)
                {
                    items.ForEach(item => Users.Use(item + result.Value));
                }

                items.ForEach(item => Users.Use(item + {|TR0003:result.Value|}));
            }
        }
        """);

    [Fact]
    public Task LambdaParameters_AreNotReported() => VerifyAsync("""
        public class Caller
        {
            public List<Int32> Run(IEnumerable<Result<Int32>> results) =>
                results.Where(result => result.IsSuccess).Select(result => result.Value).ToList();
        }
        """);

    [Fact]
    public Task LocalFunctions_StartUnchecked() => VerifyAsync("""
        public class Caller
        {
            public Int32 Run(Result<Int32> result)
            {
                return result.IsSuccess ? Read(result) : 0;

                static Int32 Read(Result<Int32> local) => {|TR0003:local.Value|};
            }
        }
        """);

    [Fact]
    public Task CatchAndFinallyBlocks_AreAnalyzed() => VerifyAsync("""
        public class Caller
        {
            public void Run(Result<Int32> result)
            {
                try
                {
                    Users.Use(result.IsSuccess ? result.Value : 0);
                }
                catch (InvalidOperationException)
                {
                    Users.Use({|TR0003:result.Value|});
                }
                finally
                {
                    Users.Use({|TR0003:result.Error|});
                }
            }
        }
        """);

    [Fact]
    public Task NameOf_DoesNotReport() => VerifyAsync("""
        public class Caller
        {
            public String Run(Result<Int32> result) => nameof(result.Value);
        }
        """);
}
