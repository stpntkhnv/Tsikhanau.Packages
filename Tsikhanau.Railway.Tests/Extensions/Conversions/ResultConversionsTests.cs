namespace Tsikhanau.Railway.Tests;

public class ResultConversionsTests
{
    private static readonly Error SourceError = Error.Create("SOURCE", "source failed");

    [Fact]
    public void ToOptional_Success_ReturnsSomeWithValue()
    {
        var optional = Result.Success(2).ToOptional();

        optional.HasValue.ShouldBeTrue();
        optional.Value.ShouldBe(2);
    }

    [Fact]
    public void ToOptional_Failure_ReturnsNone()
    {
        var optional = Result.Failure<Int32>(SourceError).ToOptional();

        optional.IsNone.ShouldBeTrue();
    }

    [Fact]
    public void ToOptional_FailureWithCustomErrorType_ReturnsNone()
    {
        var optional = Result<Int32, String>.Failure("failed").ToOptional();

        optional.IsNone.ShouldBeTrue();
    }
}
