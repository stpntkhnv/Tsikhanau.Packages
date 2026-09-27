namespace Tsikhanau.Railway.Tests;

public class ResultFirstSuccessTests
{
    private static readonly Error FirstError = Error.Create("FIRST", "first failed");
    private static readonly Error SecondError = Error.Create("SECOND", "second failed");

    [Fact]
    public void FirstSuccess_Params_SomeSucceed_ReturnsFirstSuccess()
    {
        var result = ResultExtensions.FirstSuccess(
            Result.Failure<Int32>(FirstError),
            Result.Success(2),
            Result.Success(3));

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(2);
    }

    [Fact]
    public void FirstSuccess_Params_AllFail_ReturnsLastFailure()
    {
        var result = ResultExtensions.FirstSuccess(Result.Failure<Int32>(FirstError), Result.Failure<Int32>(SecondError));

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(SecondError);
    }

    [Fact]
    public void FirstSuccess_Params_Empty_ReturnsFailureWithNullError()
    {
        var result = ResultExtensions.FirstSuccess<Int32, Error>();

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBeNull();
    }

    [Fact]
    public void FirstSuccess_Enumerable_SomeSucceed_ReturnsFirstSuccess()
    {
        IEnumerable<Result<Int32, Error>> results =
            [Result.Failure<Int32>(FirstError), Result.Success(2), Result.Success(3)];

        var result = ResultExtensions.FirstSuccess(results);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(2);
    }

    [Fact]
    public void FirstSuccess_Enumerable_AllFail_ReturnsLastFailure()
    {
        IEnumerable<Result<Int32, Error>> results = [Result.Failure<Int32>(FirstError), Result.Failure<Int32>(SecondError)];

        var result = ResultExtensions.FirstSuccess(results);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(SecondError);
    }

    [Fact]
    public void FirstSuccess_Enumerable_Empty_ReturnsFailureWithNullError()
    {
        var result = ResultExtensions.FirstSuccess(Enumerable.Empty<Result<Int32, Error>>());

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBeNull();
    }

    [Fact]
    public async Task FirstSuccessAsync_Params_SomeSucceed_ReturnsFirstSuccess()
    {
        var result = await ResultExtensions.FirstSuccessAsync(
            Task.FromResult(Result.Failure<Int32>(FirstError)),
            Task.FromResult(Result.Success(2)),
            Task.FromResult(Result.Success(3)));

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(2);
    }

    [Fact]
    public async Task FirstSuccessAsync_Params_AllFail_ReturnsLastFailure()
    {
        var result = await ResultExtensions.FirstSuccessAsync(
            Task.FromResult(Result.Failure<Int32>(FirstError)),
            Task.FromResult(Result.Failure<Int32>(SecondError)));

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(SecondError);
    }

    [Fact]
    public async Task FirstSuccessAsync_Params_Empty_ReturnsFailureWithNullError()
    {
        var result = await ResultExtensions.FirstSuccessAsync<Int32, Error>();

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBeNull();
    }

    [Fact]
    public async Task FirstSuccessAsync_Enumerable_SomeSucceed_ReturnsFirstSuccess()
    {
        IEnumerable<Task<Result<Int32, Error>>> resultTasks =
        [
            Task.FromResult(Result.Failure<Int32>(FirstError)),
            Task.FromResult(Result.Success(2)),
            Task.FromResult(Result.Success(3))
        ];

        var result = await ResultExtensions.FirstSuccessAsync(resultTasks);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(2);
    }

    [Fact]
    public async Task FirstSuccessAsync_Enumerable_AllFail_ReturnsLastFailure()
    {
        IEnumerable<Task<Result<Int32, Error>>> resultTasks =
            [Task.FromResult(Result.Failure<Int32>(FirstError)), Task.FromResult(Result.Failure<Int32>(SecondError))];

        var result = await ResultExtensions.FirstSuccessAsync(resultTasks);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(SecondError);
    }

    [Fact]
    public async Task FirstSuccessAsync_Enumerable_Empty_ReturnsFailureWithNullError()
    {
        var result = await ResultExtensions.FirstSuccessAsync(Enumerable.Empty<Task<Result<Int32, Error>>>());

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBeNull();
    }
}
