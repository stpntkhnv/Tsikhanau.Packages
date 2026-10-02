namespace Tsikhanau.Railway.Tests;

public class ResultCollectTests
{
    private static readonly Error MissingError = Error.NotFound("user.missing", "User missing");
    private static readonly ValidationError NameError = Error.Validation("name", "Name is required");
    private static readonly ValidationError AgeError = Error.Validation("age", "Age must be positive");

    public static TheoryData<Int32, Int32> FailurePositions()
    {
        var data = new TheoryData<Int32, Int32>();
        for (var arity = 1; arity <= 8; arity++)
        {
            for (var position = 0; position < arity; position++)
            {
                data.Add(arity, position);
            }
        }

        return data;
    }

    [Fact]
    public void Map_OneValue_PassesIt()
    {
        var result = Result.Collect(Result.Success(1)).Map(static a => a + 1);

        result.Value.ShouldBe(2);
    }

    [Fact]
    public void Map_TwoValuesOfDifferentTypes_PassesThemInOrder()
    {
        var result = Result
            .Collect(Result.Success(1))
            .And(Result.Success("two"))
            .Map(static (a, b) => $"{a}-{b}");

        result.Value.ShouldBe("1-two");
    }

    [Fact]
    public void Map_EightValues_PassesThemInOrder()
    {
        var result = Result
            .Collect(Result.Success(1))
            .And(Result.Success("2"))
            .And(Result.Success(3L))
            .And(Result.Success('4'))
            .And(Result.Success(5.0))
            .And(Result.Success(6m))
            .And(Result.Success((Int16)7))
            .And(Result.Success(true))
            .Map(static (a, b, c, d, e, f, g, h) => $"{a}{b}{c}{d}{e}{f}{g}{h}");

        result.Value.ShouldBe("1234567True");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(8)]
    public void Map_AllSuccessAtEveryArity_PassesValuesInOrder(Int32 arity)
    {
        var results = Enumerable.Range(1, arity).Select(static i => Result.Success(i)).ToArray();

        var result = CollectAndJoin(results);

        result.Value.ShouldBe(String.Concat(Enumerable.Range(1, arity)));
    }

    [Theory]
    [MemberData(nameof(FailurePositions))]
    public void Map_FailureAtAnyPosition_ReturnsThatError(Int32 arity, Int32 position)
    {
        var error = Error.Failure($"failed.{position}", $"Position {position} failed");
        var results = Enumerable
            .Range(0, arity)
            .Select(i => i == position ? Result.Failure<Int32>(error) : Result.Success(i))
            .ToArray();

        var result = CollectAndJoin(results);

        result.Error.ShouldBeSameAs(error);
    }

    [Fact]
    public void Map_SeveralValidationFailures_MergesFieldErrorsInOrder()
    {
        var result = Result
            .Collect(Result.Failure<Int32>(NameError))
            .And(Result.Success("ok"))
            .And(Result.Failure<Int64>(AgeError))
            .Map(static (a, b, c) => a + c);

        var error = result.Error.ShouldBeOfType<ValidationError>();
        error.FieldErrors.Select(static fieldError => fieldError.Field).ShouldBe(["name", "age"]);
    }

    [Fact]
    public void Map_MixedFailures_ReturnsAggregateErrorInOrder()
    {
        var result = Result
            .Collect(Result.Failure<Int32>(NameError))
            .And(Result.Failure<String>(MissingError))
            .Map(static (a, b) => b);

        var error = result.Error.ShouldBeOfType<AggregateError>();
        error.Errors.ShouldBe([NameError, MissingError]);
    }

    [Fact]
    public void Map_SingleFailure_ReturnsTheSameInstance()
    {
        var result = Result
            .Collect(Result.Success(1))
            .And(Result.Failure<Int32>(MissingError))
            .Map(static (a, b) => a + b);

        result.Error.ShouldBeSameAs(MissingError);
    }

    [Fact]
    public void Map_Failure_DoesNotCallTheFunction()
    {
        var called = false;

        var result = Result
            .Collect(Result.Failure<Int32>(MissingError))
            .And(Result.Success(2))
            .Map((a, b) =>
            {
                called = true;
                return a + b;
            });

        result.IsFailure.ShouldBeTrue();
        called.ShouldBeFalse();
    }

    [Fact]
    public void Bind_AllSuccess_ReturnsTheInnerResult()
    {
        var result = Result
            .Collect(Result.Success(1))
            .And(Result.Success(2))
            .Bind(static (a, b) => Result.Success(a + b));

        result.Value.ShouldBe(3);
    }

    [Fact]
    public void Bind_InnerFailure_ReturnsIt()
    {
        var result = Result
            .Collect(Result.Success(1))
            .And(Result.Success(2))
            .Bind(static (a, b) => Result.Failure<Int32>(MissingError));

        result.Error.ShouldBeSameAs(MissingError);
    }

    [Fact]
    public void Bind_CollectedFailure_DoesNotCallTheFunction()
    {
        var called = false;

        var result = Result
            .Collect(Result.Failure<Int32>(NameError))
            .Bind(a =>
            {
                called = true;
                return Result.Success(a);
            });

        result.Error.ShouldBeSameAs(NameError);
        called.ShouldBeFalse();
    }

    [Fact]
    public void Default_OfEveryArity_Throws()
    {
        var one = Result.Success(1);

        Should.Throw<InvalidOperationException>(() => default(Collected<Int32>).Map(static a => a));
        Should.Throw<InvalidOperationException>(() => default(Collected<Int32>).Bind(static a => Result.Success(a)));
        Should.Throw<InvalidOperationException>(() => default(Collected<Int32>).And(one));
        Should.Throw<InvalidOperationException>(() => default(Collected<Int32, Int32>).Map(static (a, b) => a));
        Should.Throw<InvalidOperationException>(() => default(Collected<Int32, Int32>).Bind(static (a, b) => Result.Success(a)));
        Should.Throw<InvalidOperationException>(() => default(Collected<Int32, Int32>).And(one));
        Should.Throw<InvalidOperationException>(() => default(Collected<Int32, Int32, Int32>).Map(static (a, b, c) => a));
        Should.Throw<InvalidOperationException>(() => default(Collected<Int32, Int32, Int32>).Bind(static (a, b, c) => Result.Success(a)));
        Should.Throw<InvalidOperationException>(() => default(Collected<Int32, Int32, Int32>).And(one));
        Should.Throw<InvalidOperationException>(() => default(Collected<Int32, Int32, Int32, Int32>).Map(static (a, b, c, d) => a));
        Should.Throw<InvalidOperationException>(() => default(Collected<Int32, Int32, Int32, Int32>).Bind(static (a, b, c, d) => Result.Success(a)));
        Should.Throw<InvalidOperationException>(() => default(Collected<Int32, Int32, Int32, Int32>).And(one));
        Should.Throw<InvalidOperationException>(() => default(Collected<Int32, Int32, Int32, Int32, Int32>).Map(static (a, b, c, d, e) => a));
        Should.Throw<InvalidOperationException>(() => default(Collected<Int32, Int32, Int32, Int32, Int32>).Bind(static (a, b, c, d, e) => Result.Success(a)));
        Should.Throw<InvalidOperationException>(() => default(Collected<Int32, Int32, Int32, Int32, Int32>).And(one));
        Should.Throw<InvalidOperationException>(() => default(Collected<Int32, Int32, Int32, Int32, Int32, Int32>).Map(static (a, b, c, d, e, f) => a));
        Should.Throw<InvalidOperationException>(() => default(Collected<Int32, Int32, Int32, Int32, Int32, Int32>).Bind(static (a, b, c, d, e, f) => Result.Success(a)));
        Should.Throw<InvalidOperationException>(() => default(Collected<Int32, Int32, Int32, Int32, Int32, Int32>).And(one));
        Should.Throw<InvalidOperationException>(() => default(Collected<Int32, Int32, Int32, Int32, Int32, Int32, Int32>).Map(static (a, b, c, d, e, f, g) => a));
        Should.Throw<InvalidOperationException>(() => default(Collected<Int32, Int32, Int32, Int32, Int32, Int32, Int32>).Bind(static (a, b, c, d, e, f, g) => Result.Success(a)));
        Should.Throw<InvalidOperationException>(() => default(Collected<Int32, Int32, Int32, Int32, Int32, Int32, Int32>).And(one));
        Should.Throw<InvalidOperationException>(() => default(Collected<Int32, Int32, Int32, Int32, Int32, Int32, Int32, Int32>).Map(static (a, b, c, d, e, f, g, h) => a));
        Should.Throw<InvalidOperationException>(() => default(Collected<Int32, Int32, Int32, Int32, Int32, Int32, Int32, Int32>).Bind(static (a, b, c, d, e, f, g, h) => Result.Success(a)));
    }

    [Fact]
    public void MapAndBind_DefaultResultCollected_Throw()
    {
        var collected = Result.Collect(default(Result<Int32>));

        Should.Throw<InvalidOperationException>(() => collected.Map(static a => a));
        Should.Throw<InvalidOperationException>(() => collected.Bind(Result.Success));
    }

    [Fact]
    public void Map_NullFunction_Throws()
    {
        var collected = Result.Collect(Result.Success(1));

        Should.Throw<ArgumentNullException>(() => collected.Map<Int32>(null!));
        Should.Throw<ArgumentNullException>(() => collected.Bind<Int32>(null!));
    }

    private static Result<String> CollectAndJoin(Result<Int32>[] r) =>
        r.Length switch
        {
            1 => Result.Collect(r[0]).Map(static a => $"{a}"),
            2 => Result.Collect(r[0]).And(r[1]).Map(static (a, b) => $"{a}{b}"),
            3 => Result.Collect(r[0]).And(r[1]).And(r[2]).Map(static (a, b, c) => $"{a}{b}{c}"),
            4 => Result.Collect(r[0]).And(r[1]).And(r[2]).And(r[3]).Map(static (a, b, c, d) => $"{a}{b}{c}{d}"),
            5 => Result.Collect(r[0]).And(r[1]).And(r[2]).And(r[3]).And(r[4])
                .Map(static (a, b, c, d, e) => $"{a}{b}{c}{d}{e}"),
            6 => Result.Collect(r[0]).And(r[1]).And(r[2]).And(r[3]).And(r[4]).And(r[5])
                .Map(static (a, b, c, d, e, f) => $"{a}{b}{c}{d}{e}{f}"),
            7 => Result.Collect(r[0]).And(r[1]).And(r[2]).And(r[3]).And(r[4]).And(r[5]).And(r[6])
                .Map(static (a, b, c, d, e, f, g) => $"{a}{b}{c}{d}{e}{f}{g}"),
            8 => Result.Collect(r[0]).And(r[1]).And(r[2]).And(r[3]).And(r[4]).And(r[5]).And(r[6]).And(r[7])
                .Map(static (a, b, c, d, e, f, g, h) => $"{a}{b}{c}{d}{e}{f}{g}{h}"),
            _ => throw new ArgumentOutOfRangeException(nameof(r)),
        };
}
