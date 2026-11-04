namespace Tsikhanau.Railway.Tests;

public class OptionalConversionsTests
{
    private static readonly Error MissingError = Error.NotFound("value.missing", "Value missing");

    [Fact]
    public void ToResult_Some_WithError_ReturnsSuccessWithValue()
    {
        var result = Optional<Int32>.Some(2).ToResult(MissingError);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(2);
    }

    [Fact]
    public void ToResult_None_WithError_ReturnsFailureWithError()
    {
        var result = Optional<Int32>.None().ToResult(MissingError);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(MissingError);
    }

    [Fact]
    public void ToResult_None_WithErrorSubclass_KeepsErrorInstance()
    {
        var error = Error.Validation("name", "Name is required");

        var result = Optional<String>.None().ToResult(error);

        result.Error.ShouldBeSameAs(error);
        result.Error.ShouldBeOfType<ValidationError>();
    }

    [Fact]
    public void ToResult_None_WithNullError_ThrowsArgumentNullException()
    {
        var optional = Optional<Int32>.None();

        Should.Throw<ArgumentNullException>(() => optional.ToResult((Error)null!));
    }

    [Fact]
    public void ToResult_Some_WithErrorFactory_ReturnsSuccessWithoutCallingFactory()
    {
        var errorFactory = Substitute.For<Func<Error>>();

        var result = Optional<Int32>.Some(2).ToResult(errorFactory);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(2);
        errorFactory.DidNotReceive().Invoke();
    }

    [Fact]
    public void ToResult_None_WithErrorFactory_ReturnsFailureWithFactoryError()
    {
        var errorFactory = Substitute.For<Func<Error>>();
        errorFactory().Returns(MissingError);

        var result = Optional<Int32>.None().ToResult(errorFactory);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(MissingError);
        errorFactory.Received(1).Invoke();
    }

    [Fact]
    public void ToResult_None_ErrorFactoryReturnsNull_ThrowsArgumentNullException()
    {
        var optional = Optional<Int32>.None();

        Should.Throw<ArgumentNullException>(() => optional.ToResult(() => null!));
    }

    [Fact]
    public void AsNullable_Some_ReturnsValue()
    {
        var optional = Optional<Int32>.Some(5);

        var value = optional.AsNullable();

        value.ShouldBe(5);
    }

    [Fact]
    public void AsNullable_None_ReturnsNull()
    {
        var optional = Optional<Int32>.None();

        var value = optional.AsNullable();

        value.ShouldBeNull();
    }

    [Fact]
    public void AsEnumerable_Some_ReturnsTheValue()
    {
        var optional = Optional<String>.Some("a");

        var sequence = optional.AsEnumerable();

        sequence.ShouldBe(["a"]);
    }

    [Fact]
    public void AsEnumerable_None_ReturnsEmpty()
    {
        var optional = Optional<String>.None();

        var sequence = optional.AsEnumerable();

        sequence.ShouldBeEmpty();
    }
}
