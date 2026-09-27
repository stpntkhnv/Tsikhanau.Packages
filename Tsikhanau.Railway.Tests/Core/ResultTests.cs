namespace Tsikhanau.Railway.Tests;

public class ResultTests
{
    private static readonly Error FailError = Error.Failure("fail", "Failed");

    [Fact]
    public void Success_Value_ReturnsSuccessWithValue()
    {
        var result = Result<Int32>.Success(5);

        result.IsSuccess.ShouldBeTrue();
        result.IsFailure.ShouldBeFalse();
        result.Value.ShouldBe(5);
    }

    [Fact]
    public void Success_Null_ThrowsArgumentNullException()
    {
        var exception = Should.Throw<ArgumentNullException>(() => Result<String>.Success(null!));

        exception.ParamName.ShouldBe("value");
    }

    [Fact]
    public void Failure_Error_ReturnsFailureWithError()
    {
        var result = Result<Int32>.Failure(FailError);

        result.IsFailure.ShouldBeTrue();
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldBeSameAs(FailError);
    }

    [Fact]
    public void Failure_Null_ThrowsArgumentNullException()
    {
        var exception = Should.Throw<ArgumentNullException>(() => Result<Int32>.Failure(null!));

        exception.ParamName.ShouldBe("error");
    }

    [Fact]
    public void Value_Failure_ThrowsInvalidOperationExceptionWithError()
    {
        var result = Result<Int32>.Failure(FailError);

        var exception = Should.Throw<InvalidOperationException>(() => result.Value);

        exception.Message.ShouldBe("Cannot access Value on a failed result. Error: [fail] Failed");
    }

    [Fact]
    public void Error_Success_ThrowsInvalidOperationException()
    {
        var result = Result<Int32>.Success(5);

        var exception = Should.Throw<InvalidOperationException>(() => result.Error);

        exception.Message.ShouldBe("Cannot access Error on a successful result.");
    }

    [Fact]
    public void Default_Result_IsFailure()
    {
        var result = default(Result<Int32>);

        result.IsFailure.ShouldBeTrue();
        result.IsSuccess.ShouldBeFalse();
    }

    [Fact]
    public void Default_Error_ThrowsNotInitialized()
    {
        var result = default(Result<Int32>);

        var exception = Should.Throw<InvalidOperationException>(() => result.Error);

        exception.Message.ShouldBe("Result is not initialized. Create it with Result.Success or Result.Failure.");
    }

    [Fact]
    public void Default_Value_ThrowsInvalidOperationExceptionWithNotInitialized()
    {
        var result = default(Result<Int32>);

        var exception = Should.Throw<InvalidOperationException>(() => result.Value);

        exception.Message.ShouldBe("Cannot access Value on a failed result. Error: not initialized");
    }

    [Fact]
    public void Default_ToString_ReturnsUninitialized()
    {
        var text = default(Result<Int32>).ToString();

        text.ShouldBe("Uninitialized");
    }

    [Fact]
    public void Default_ComparedToDefault_IsEqual()
    {
        var left = default(Result<Int32>);
        var right = default(Result<Int32>);

        left.Equals(right).ShouldBeTrue();
        (left == right).ShouldBeTrue();
        left.GetHashCode().ShouldBe(right.GetHashCode());
    }

    [Fact]
    public void Default_ComparedToFailure_IsNotEqual()
    {
        var result = default(Result<Int32>);

        (result == Result<Int32>.Failure(FailError)).ShouldBeFalse();
        (result != Result<Int32>.Failure(FailError)).ShouldBeTrue();
    }

    [Fact]
    public void Equals_SuccessesWithEqualValues_ReturnsTrue()
    {
        var left = Result<Int32>.Success(5);
        var right = Result<Int32>.Success(5);

        left.Equals(right).ShouldBeTrue();
        (left == right).ShouldBeTrue();
        (left != right).ShouldBeFalse();
        left.GetHashCode().ShouldBe(right.GetHashCode());
    }

    [Fact]
    public void Equals_SuccessesWithDifferentValues_ReturnsFalse()
    {
        var left = Result<Int32>.Success(5);
        var right = Result<Int32>.Success(6);

        left.Equals(right).ShouldBeFalse();
        (left == right).ShouldBeFalse();
        (left != right).ShouldBeTrue();
    }

    [Fact]
    public void Equals_FailuresWithEqualErrors_ReturnsTrue()
    {
        var left = Result<Int32>.Failure(Error.Failure("fail", "Failed"));
        var right = Result<Int32>.Failure(Error.Failure("fail", "Failed"));

        left.Equals(right).ShouldBeTrue();
        (left == right).ShouldBeTrue();
        (left != right).ShouldBeFalse();
        left.GetHashCode().ShouldBe(right.GetHashCode());
    }

    [Fact]
    public void Equals_FailuresWithDifferentErrors_ReturnsFalse()
    {
        var left = Result<Int32>.Failure(Error.Failure("fail", "Failed"));
        var right = Result<Int32>.Failure(Error.Failure("other", "Failed"));

        left.Equals(right).ShouldBeFalse();
        (left == right).ShouldBeFalse();
        (left != right).ShouldBeTrue();
    }

    [Fact]
    public void Equals_SuccessAndFailure_ReturnsFalse()
    {
        var success = Result<Int32>.Success(5);
        var failure = Result<Int32>.Failure(FailError);

        success.Equals(failure).ShouldBeFalse();
        (success == failure).ShouldBeFalse();
        (success != failure).ShouldBeTrue();
    }

    [Fact]
    public void Equals_BoxedEqualResult_ReturnsTrue()
    {
        var result = Result<Int32>.Success(5);
        Object other = Result<Int32>.Success(5);

        result.Equals(other).ShouldBeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData(5)]
    [InlineData("5")]
    public void Equals_NonResultObject_ReturnsFalse(Object? other)
    {
        var result = Result<Int32>.Success(5);

        result.Equals(other).ShouldBeFalse();
    }

    [Fact]
    public void Equals_UnboxedValue_ConvertsImplicitlyAndReturnsTrue()
    {
        var result = Result<Int32>.Success(5);

        result.Equals(5).ShouldBeTrue();
        (result == 5).ShouldBeTrue();
    }

    [Fact]
    public void ToString_Success_FormatsValue()
    {
        var text = Result<Int32>.Success(5).ToString();

        text.ShouldBe("Success(5)");
    }

    [Fact]
    public void ToString_Failure_FormatsError()
    {
        var text = Result<Int32>.Failure(FailError).ToString();

        text.ShouldBe("Failure([fail] Failed)");
    }

    [Fact]
    public void ImplicitConversion_FromValue_ReturnsSuccess()
    {
        Result<Int32> result = 5;

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(5);
    }

    [Fact]
    public void ImplicitConversion_FromError_ReturnsFailure()
    {
        Result<Int32> result = FailError;

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBeSameAs(FailError);
    }

    [Fact]
    public void ImplicitConversion_FromErrorSubclass_ReturnsFailureWithSameInstance()
    {
        var error = Error.Validation("name", "Required");

        Result<Int32> result = error;

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBeSameAs(error);
    }

    [Fact]
    public void ImplicitConversion_FromNullValue_ThrowsArgumentNullException()
    {
        String value = null!;

        var exception = Should.Throw<ArgumentNullException>(() =>
        {
            Result<String> result = value;
            return result;
        });

        exception.ParamName.ShouldBe("value");
    }

    [Fact]
    public void ImplicitConversion_FromNullError_ThrowsArgumentNullException()
    {
        Error error = null!;

        var exception = Should.Throw<ArgumentNullException>(() =>
        {
            Result<Int32> result = error;
            return result;
        });

        exception.ParamName.ShouldBe("error");
    }
}
