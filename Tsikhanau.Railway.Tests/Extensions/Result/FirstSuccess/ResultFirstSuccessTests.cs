namespace Tsikhanau.Railway.Tests;

public class ResultFirstSuccessTests
{
    private static readonly Error FirstError = Error.Failure("first.failed", "First failed");
    private static readonly Error SecondError = Error.NotFound("second.missing", "Second missing");

    [Fact]
    public void FirstSuccess_Params_SomeSucceed_ReturnsFirstSuccess()
    {
        var result = Result.FirstSuccess(
            Result.Failure<Int32>(FirstError),
            Result.Success(2),
            Result.Success(3));

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(2);
    }

    [Fact]
    public void FirstSuccess_Params_AllFail_ReturnsLastFailure()
    {
        var result = Result.FirstSuccess(Result.Failure<Int32>(FirstError), Result.Failure<Int32>(SecondError));

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(SecondError);
    }

    [Fact]
    public void FirstSuccess_Params_Empty_ThrowsArgumentException()
    {
        var exception = Should.Throw<ArgumentException>(() => Result.FirstSuccess<Int32>());

        exception.ParamName.ShouldBe("results");
    }

    [Fact]
    public void FirstSuccess_Array_SomeSucceed_ReturnsFirstSuccess()
    {
        Result<Int32>[] results = [Result.Failure<Int32>(FirstError), Result.Success(2), Result.Success(3)];

        var result = Result.FirstSuccess(results);

        result.Value.ShouldBe(2);
    }

    [Fact]
    public void FirstSuccess_Array_Empty_ThrowsArgumentException()
    {
        Result<Int32>[] results = [];

        Should.Throw<ArgumentException>(() => Result.FirstSuccess(results));
    }

    [Fact]
    public void FirstSuccess_CollectionExpression_AllFail_ReturnsLastFailure()
    {
        var result = Result.FirstSuccess([Result.Failure<String>(SecondError), Result.Failure<String>(FirstError)]);

        result.Error.ShouldBe(FirstError);
    }

    [Fact]
    public void FirstSuccess_Enumerable_SomeSucceed_ReturnsFirstSuccess()
    {
        List<Result<Int32>> results = [Result.Failure<Int32>(FirstError), Result.Success(2), Result.Success(3)];

        var result = results.FirstSuccess();

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(2);
    }

    [Fact]
    public void FirstSuccess_Enumerable_AllFail_ReturnsLastFailure()
    {
        List<Result<Int32>> results = [Result.Failure<Int32>(FirstError), Result.Failure<Int32>(SecondError)];

        var result = results.FirstSuccess();

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(SecondError);
    }

    [Fact]
    public void FirstSuccess_Enumerable_Empty_ThrowsArgumentException()
    {
        var exception = Should.Throw<ArgumentException>(() => Enumerable.Empty<Result<Int32>>().FirstSuccess());

        exception.ParamName.ShouldBe("results");
    }

    [Fact]
    public void FirstSuccess_Enumerable_Null_ThrowsArgumentNullException()
    {
        IEnumerable<Result<Int32>> results = null!;

        Should.Throw<ArgumentNullException>(() => results.FirstSuccess());
    }

    [Fact]
    public async Task FirstSuccessAsync_Params_SomeSucceed_ReturnsFirstSuccess()
    {
        var result = await Result.FirstSuccessAsync(
            Task.FromResult(Result.Failure<Int32>(FirstError)),
            Task.FromResult(Result.Success(2)),
            Task.FromResult(Result.Success(3)));

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(2);
    }

    [Fact]
    public async Task FirstSuccessAsync_Params_AllFail_ReturnsLastFailure()
    {
        var result = await Result.FirstSuccessAsync(
            Task.FromResult(Result.Failure<Int32>(FirstError)),
            Task.FromResult(Result.Failure<Int32>(SecondError)));

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(SecondError);
    }

    [Fact]
    public async Task FirstSuccessAsync_Params_Empty_ThrowsArgumentException()
    {
        await Should.ThrowAsync<ArgumentException>(() => Result.FirstSuccessAsync<Int32>());
    }

    [Fact]
    public async Task FirstSuccessAsync_Params_TasksCompleteOutOfOrder_ReturnsFirstSuccessByPosition()
    {
        var first = new TaskCompletionSource<Result<Int32>>();
        var second = new TaskCompletionSource<Result<Int32>>();

        var combined = Result.FirstSuccessAsync(first.Task, second.Task);
        second.SetResult(Result.Success(2));
        first.SetResult(Result.Success(1));
        var result = await combined;

        result.Value.ShouldBe(1);
    }

    [Fact]
    public async Task FirstSuccessAsync_Enumerable_SomeSucceed_ReturnsFirstSuccess()
    {
        List<Task<Result<Int32>>> resultTasks =
        [
            Task.FromResult(Result.Failure<Int32>(FirstError)),
            Task.FromResult(Result.Success(2)),
            Task.FromResult(Result.Success(3))
        ];

        var result = await resultTasks.FirstSuccessAsync();

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(2);
    }

    [Fact]
    public async Task FirstSuccessAsync_Enumerable_AllFail_ReturnsLastFailure()
    {
        List<Task<Result<Int32>>> resultTasks =
            [Task.FromResult(Result.Failure<Int32>(FirstError)), Task.FromResult(Result.Failure<Int32>(SecondError))];

        var result = await resultTasks.FirstSuccessAsync();

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(SecondError);
    }

    [Fact]
    public async Task FirstSuccessAsync_Enumerable_Empty_ThrowsArgumentException()
    {
        await Should.ThrowAsync<ArgumentException>(() => Enumerable.Empty<Task<Result<Int32>>>().FirstSuccessAsync());
    }
}
