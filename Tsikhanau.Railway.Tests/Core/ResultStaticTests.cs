namespace Tsikhanau.Railway.Tests;

public class ResultStaticTests
{
    private static readonly Error FailError = Error.Create("FAIL", "failed");

    [Fact]
    public void Success_Data_ReturnsSuccessWithData()
    {
        var result = Result.Success(5);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(5);
    }

    [Fact]
    public void Success_NullData_ThrowsArgumentNullException()
    {
        var exception = Should.Throw<ArgumentNullException>(() => Result.Success<String>(null!));

        exception.ParamName.ShouldBe("value");
    }

    [Fact]
    public void Success_NoArguments_ReturnsUnitSuccess()
    {
        var result = Result.Success();

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
    public void Failure_NoArguments_ReturnsUnknownError()
    {
        var result = Result.Failure();

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBeSameAs(Error.Unknown);
    }

    [Fact]
    public void ToResult_PredicateReturnsTrue_ReturnsSuccessWithData()
    {
        var predicate = Substitute.For<Func<Int32, Boolean>>();
        predicate.Invoke(5).Returns(true);

        var result = 5.ToResult(predicate);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(5);
        predicate.Received(1).Invoke(5);
    }

    [Fact]
    public void ToResult_PredicateReturnsFalse_ReturnsUnknownError()
    {
        var predicate = Substitute.For<Func<Int32, Boolean>>();
        predicate.Invoke(5).Returns(false);

        var result = 5.ToResult(predicate);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBeSameAs(Error.Unknown);
        predicate.Received(1).Invoke(5);
    }

    [Fact]
    public void ToResult_NullPredicate_ThrowsNullReferenceException()
    {
        Should.Throw<NullReferenceException>(() => 5.ToResult(null!));
    }

    [Fact]
    public void ToResult_NullDataAndPredicateReturnsTrue_ThrowsArgumentNullException()
    {
        String data = null!;

        var exception = Should.Throw<ArgumentNullException>(() => data.ToResult(_ => true));

        exception.ParamName.ShouldBe("value");
    }

    [Fact]
    public void ToResult_NullDataAndPredicateReturnsFalse_ReturnsUnknownError()
    {
        String data = null!;

        var result = data.ToResult(_ => false);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBeSameAs(Error.Unknown);
    }
}
