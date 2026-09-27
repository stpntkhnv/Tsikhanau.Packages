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
}
