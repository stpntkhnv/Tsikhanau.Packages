namespace Tsikhanau.Railway.Tests;

public class ResultZipTests
{
    private static readonly Error FirstError = Error.Create("FIRST", "first failed");
    private static readonly Error SecondError = Error.Create("SECOND", "second failed");

    [Fact]
    public void Zip_BothSuccess_ReturnsTupleOfValues()
    {
        var result = Result.Success(2).Zip(Result.Success("two"));

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe((2, "two"));
    }

    [Fact]
    public void Zip_FirstFails_ReturnsFirstError()
    {
        var result = Result.Failure<Int32>(FirstError).Zip(Result.Success("two"));

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(FirstError);
    }

    [Fact]
    public void Zip_SecondFails_ReturnsSecondError()
    {
        var result = Result.Success(2).Zip(Result.Failure<String>(SecondError));

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(SecondError);
    }

    [Fact]
    public void Zip_BothFail_ReturnsFirstError()
    {
        var result = Result.Failure<Int32>(FirstError).Zip(Result.Failure<String>(SecondError));

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(FirstError);
    }
}
