namespace Tsikhanau.Railway.Tests;

public class ResultStaticTests
{
    private static readonly Error FailError = Error.Failure("fail", "Failed");

    [Fact]
    public void Success_Value_ReturnsSuccessWithValue()
    {
        var result = Result.Success(5);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(5);
    }

    [Fact]
    public void Success_NullValue_ThrowsArgumentNullException()
    {
        var exception = Should.Throw<ArgumentNullException>(() => Result.Success<String>(null!));

        exception.ParamName.ShouldBe("value");
    }

    [Fact]
    public void Success_NoArguments_ReturnsUnitSuccess()
    {
        Result<Unit> result = Result.Success();

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(Unit.Value);
    }

    [Fact]
    public void Failure_Error_ReturnsFailureWithError()
    {
        var result = Result.Failure<Int32>(FailError);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBeSameAs(FailError);
    }

    [Fact]
    public void Failure_NullError_ThrowsArgumentNullException()
    {
        var exception = Should.Throw<ArgumentNullException>(() => Result.Failure<Int32>(null!));

        exception.ParamName.ShouldBe("error");
    }

    [Fact]
    public void Failure_ErrorWithoutTypeArgument_ReturnsUnitFailure()
    {
        Result<Unit> result = Result.Failure(FailError);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBeSameAs(FailError);
    }

    [Fact]
    public void Failure_NullErrorWithoutTypeArgument_ThrowsArgumentNullException()
    {
        var exception = Should.Throw<ArgumentNullException>(() => Result.Failure(null!));

        exception.ParamName.ShouldBe("error");
    }
}
