namespace Tsikhanau.Railway.Tests;

public class AggregateErrorTests
{
    [Fact]
    public void From_Errors_KeepsOrderAndJoinsMessages()
    {
        var first = Error.NotFound("first", "First");
        var second = Error.NotFound("second", "Second");
        var third = Error.NotFound("third", "Third");

        var error = AggregateError.From([first, second, third]);

        error.Errors.ShouldBe([first, second, third]);
        error.Code.ShouldBe("aggregate");
        error.Message.ShouldBe("First; Second; Third");
        error.Inner.ShouldBeNull();
        error.Metadata.ShouldBeNull();
    }

    [Theory]
    [InlineData(ErrorKind.NotFound, ErrorKind.NotFound, ErrorKind.NotFound)]
    [InlineData(ErrorKind.Validation, ErrorKind.Validation, ErrorKind.Validation)]
    [InlineData(ErrorKind.NotFound, ErrorKind.Conflict, ErrorKind.Failure)]
    [InlineData(ErrorKind.Unexpected, ErrorKind.Forbidden, ErrorKind.Failure)]
    public void From_Errors_UsesSharedKindOrFailure(ErrorKind firstKind, ErrorKind secondKind, ErrorKind expected)
    {
        var error = AggregateError.From([new Error(firstKind, "first", "First"), new Error(secondKind, "second", "Second")]);

        error.Kind.ShouldBe(expected);
    }

    [Fact]
    public void From_SingleError_UsesItsKind()
    {
        var error = AggregateError.From([Error.Conflict("first", "First")]);

        error.Kind.ShouldBe(ErrorKind.Conflict);
        error.Message.ShouldBe("First");
    }

    [Fact]
    public void From_SourceChangedAfterwards_KeepsOriginalErrors()
    {
        var first = Error.Failure("first", "First");
        var source = new List<Error> { first };

        var error = AggregateError.From(source);
        source.Add(Error.Failure("second", "Second"));

        error.Errors.ShouldBe([first]);
    }

    [Fact]
    public void From_Empty_ThrowsArgumentException()
    {
        var exception = Should.Throw<ArgumentException>(() => AggregateError.From([]));

        exception.ShouldBeOfType<ArgumentException>();
        exception.ParamName.ShouldBe("errors");
    }

    [Fact]
    public void From_NullElement_ThrowsArgumentException()
    {
        Error[] errors = [Error.Failure("first", "First"), null!];

        var exception = Should.Throw<ArgumentException>(() => AggregateError.From(errors));

        exception.ShouldBeOfType<ArgumentException>();
        exception.ParamName.ShouldBe("errors");
    }

    [Fact]
    public void From_Null_ThrowsArgumentNullException()
    {
        var exception = Should.Throw<ArgumentNullException>(() => AggregateError.From(null!));

        exception.ParamName.ShouldBe("errors");
    }

    [Fact]
    public void Equals_SameErrorsInSameOrder_ReturnsTrue()
    {
        var left = AggregateError.From([Error.Failure("first", "First"), Error.Failure("second", "Second")]);
        var right = AggregateError.From([Error.Failure("first", "First"), Error.Failure("second", "Second")]);

        left.Equals(right).ShouldBeTrue();
        (left == right).ShouldBeTrue();
        (left != right).ShouldBeFalse();
        left.GetHashCode().ShouldBe(right.GetHashCode());
    }

    [Fact]
    public void Equals_SameErrorsInDifferentOrder_ReturnsFalse()
    {
        var left = AggregateError.From([Error.Failure("first", "First"), Error.Failure("second", "Second")]);
        var right = AggregateError.From([Error.Failure("second", "Second"), Error.Failure("first", "First")]);

        left.Equals(right).ShouldBeFalse();
        (left == right).ShouldBeFalse();
        (left != right).ShouldBeTrue();
    }

    [Fact]
    public void Equals_SameMessageDifferentErrors_ReturnsFalse()
    {
        var left = AggregateError.From([Error.Failure("first", "Same"), Error.Failure("second", "Other")]);
        var right = AggregateError.From([Error.Failure("third", "Same"), Error.Failure("fourth", "Other")]);

        left.Message.ShouldBe(right.Message);
        left.Equals(right).ShouldBeFalse();
        (left == right).ShouldBeFalse();
    }

    [Fact]
    public void Equals_PlainErrorWithSameFields_ReturnsFalse()
    {
        var aggregate = AggregateError.From([Error.Failure("first", "First")]);
        var error = new Error(ErrorKind.Failure, "aggregate", "First");

        aggregate.Equals(error).ShouldBeFalse();
        error.Equals(aggregate).ShouldBeFalse();
        (error == aggregate).ShouldBeFalse();
    }

    [Fact]
    public void ToString_MultipleErrors_FormatsCodeAndJoinedMessage()
    {
        var text = AggregateError.From([Error.Failure("first", "First"), Error.Conflict("second", "Second")]).ToString();

        text.ShouldBe("[aggregate] First; Second");
    }

    [Fact]
    public void Errors_CannotBeCastToMutableArray()
    {
        var error = AggregateError.From([Error.Failure("a", "A"), Error.Failure("b", "B")]);

        (error.Errors is Error[]).ShouldBeFalse();
    }
}
