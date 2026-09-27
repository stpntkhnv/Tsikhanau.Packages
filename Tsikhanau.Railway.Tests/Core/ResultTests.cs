namespace Tsikhanau.Railway.Tests;

public class ResultTests
{
    private static readonly Error FailError = Error.Create("FAIL", "failed");

    [Fact]
    public void Success_Value_ReturnsSuccessWithValue()
    {
        var result = Result<Int32, Error>.Success(5);

        result.IsSuccess.ShouldBeTrue();
        result.IsFailure.ShouldBeFalse();
        result.Value.ShouldBe(5);
    }

    [Fact]
    public void Success_Null_ThrowsArgumentNullException()
    {
        var exception = Should.Throw<ArgumentNullException>(() => Result<String, Error>.Success(null!));

        exception.ParamName.ShouldBe("value");
    }

    [Fact]
    public void Failure_Error_ReturnsFailureWithError()
    {
        var result = Result<Int32, Error>.Failure(FailError);

        result.IsFailure.ShouldBeTrue();
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldBeSameAs(FailError);
    }

    [Fact]
    public void Failure_Null_ThrowsArgumentNullException()
    {
        var exception = Should.Throw<ArgumentNullException>(() => Result<Int32, Error>.Failure(null!));

        exception.ParamName.ShouldBe("error");
    }

    [Fact]
    public void Value_Failure_ThrowsInvalidOperationExceptionWithError()
    {
        var result = Result<Int32, Error>.Failure(FailError);

        var exception = Should.Throw<InvalidOperationException>(() => result.Value);

        exception.Message.ShouldBe("Cannot access Value on a failed result. Error: [FAIL] failed");
        exception.Message.ShouldContain(FailError.ToString());
    }

    [Fact]
    public void Error_Success_ThrowsInvalidOperationException()
    {
        var result = Result<Int32, Error>.Success(5);

        var exception = Should.Throw<InvalidOperationException>(() => result.Error);

        exception.Message.ShouldBe("Cannot access Error on a successful result.");
    }

    [Fact]
    public void Equals_SuccessesWithEqualValues_ReturnsTrue()
    {
        var left = Result<Int32, Error>.Success(5);
        var right = Result<Int32, Error>.Success(5);

        left.Equals(right).ShouldBeTrue();
        (left == right).ShouldBeTrue();
        (left != right).ShouldBeFalse();
        left.GetHashCode().ShouldBe(right.GetHashCode());
    }

    [Fact]
    public void Equals_SuccessesWithDifferentValues_ReturnsFalse()
    {
        var left = Result<Int32, Error>.Success(5);
        var right = Result<Int32, Error>.Success(6);

        left.Equals(right).ShouldBeFalse();
        (left == right).ShouldBeFalse();
        (left != right).ShouldBeTrue();
    }

    [Fact]
    public void Equals_FailuresWithEqualErrors_ReturnsTrue()
    {
        var left = Result<Int32, Error>.Failure(Error.Create("FAIL", "failed"));
        var right = Result<Int32, Error>.Failure(Error.Create("FAIL", "failed"));

        left.Equals(right).ShouldBeTrue();
        (left == right).ShouldBeTrue();
        (left != right).ShouldBeFalse();
        left.GetHashCode().ShouldBe(right.GetHashCode());
    }

    [Fact]
    public void Equals_FailuresWithDifferentErrors_ReturnsFalse()
    {
        var left = Result<Int32, Error>.Failure(Error.Create("FAIL", "failed"));
        var right = Result<Int32, Error>.Failure(Error.Create("OTHER", "failed"));

        left.Equals(right).ShouldBeFalse();
        (left == right).ShouldBeFalse();
        (left != right).ShouldBeTrue();
    }

    [Fact]
    public void Equals_SuccessAndFailureWithSameUnderlyingValue_ReturnsFalse()
    {
        var success = Result<Int32, Int32>.Success(1);
        var failure = Result<Int32, Int32>.Failure(1);

        success.Equals(failure).ShouldBeFalse();
        (success == failure).ShouldBeFalse();
        (success != failure).ShouldBeTrue();
    }

    [Fact]
    public void Equals_BoxedEqualResult_ReturnsTrue()
    {
        var result = Result<Int32, Error>.Success(5);
        Object other = Result<Int32, Error>.Success(5);

        result.Equals(other).ShouldBeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData(5)]
    [InlineData("5")]
    public void Equals_NonResultObject_ReturnsFalse(Object? other)
    {
        var result = Result<Int32, Error>.Success(5);

        result.Equals(other).ShouldBeFalse();
    }

    [Fact]
    public void Equals_UnboxedData_ConvertsImplicitlyAndReturnsTrue()
    {
        var result = Result<Int32, Error>.Success(5);

        result.Equals(5).ShouldBeTrue();
        (result == 5).ShouldBeTrue();
    }

    [Fact]
    public void ToString_Success_FormatsValue()
    {
        var text = Result<Int32, Error>.Success(5).ToString();

        text.ShouldBe("Success(5)");
    }

    [Fact]
    public void ToString_Failure_FormatsError()
    {
        var text = Result<Int32, Error>.Failure(FailError).ToString();

        text.ShouldBe("Failure([FAIL] failed)");
    }

    [Fact]
    public void ImplicitConversion_FromData_ReturnsSuccess()
    {
        Result<Int32, Error> result = 5;

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(5);
    }

    [Fact]
    public void ImplicitConversion_FromError_ReturnsFailure()
    {
        Result<Int32, Error> result = FailError;

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBeSameAs(FailError);
    }

    [Fact]
    public void ImplicitConversion_FromNullData_ThrowsArgumentNullException()
    {
        String data = null!;

        var exception = Should.Throw<ArgumentNullException>(() =>
        {
            Result<String, Error> result = data;
            return result;
        });

        exception.ParamName.ShouldBe("value");
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ImplicitConversion_ToBoolean_ReturnsIsSuccess(Boolean isSuccess)
    {
        var result = isSuccess ? Result<Int32, Error>.Success(5) : Result<Int32, Error>.Failure(FailError);

        Boolean converted = result;

        converted.ShouldBe(isSuccess);
    }

    [Fact]
    public void Default_Result_IsFailureWithNullError()
    {
        var result = default(Result<Int32, Error>);

        result.IsFailure.ShouldBeTrue();
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldBeNull();
    }

    [Fact]
    public void Default_Value_ThrowsInvalidOperationExceptionWithEmptyError()
    {
        var result = default(Result<Int32, Error>);

        var exception = Should.Throw<InvalidOperationException>(() => result.Value);

        exception.Message.ShouldBe("Cannot access Value on a failed result. Error: ");
    }

    [Fact]
    public void Default_ToString_FormatsEmptyFailure()
    {
        var text = default(Result<Int32, Error>).ToString();

        text.ShouldBe("Failure()");
    }

    [Fact]
    public void Default_ComparedToDefault_IsEqual()
    {
        var left = default(Result<Int32, Error>);
        var right = default(Result<Int32, Error>);

        left.Equals(right).ShouldBeTrue();
        (left == right).ShouldBeTrue();
        left.GetHashCode().ShouldBe(right.GetHashCode());
    }

    [Fact]
    public void Default_ComparedToFailure_IsNotEqual()
    {
        var result = default(Result<Int32, Error>);

        (result == Result<Int32, Error>.Failure(FailError)).ShouldBeFalse();
    }
}
