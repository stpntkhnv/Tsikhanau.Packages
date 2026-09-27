namespace Tsikhanau.Railway.Tests;

public class ResultCombineAllTests
{
    private static readonly Error FirstError = Error.Create("FIRST", "first failed");
    private static readonly Error SecondError = Error.Create("SECOND", "second failed");

    [Fact]
    public void CombineAll_Params_AllSuccess_ReturnsAllValuesInOrder()
    {
        var result = ResultExtensions.CombineAll(Result.Success(1), Result.Success(2), Result.Success(3));

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(new[] { 1, 2, 3 });
    }

    [Fact]
    public void CombineAll_Params_SomeFail_ReturnsAllErrorsInOrder()
    {
        var result = ResultExtensions.CombineAll(
            Result.Success(1),
            Result.Failure<Int32>(FirstError),
            Result.Success(2),
            Result.Failure<Int32>(SecondError));

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(new[] { FirstError, SecondError });
    }

    [Fact]
    public void CombineAll_Params_Empty_ReturnsSuccessWithEmptyList()
    {
        var result = ResultExtensions.CombineAll<Int32, Error>();

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBeEmpty();
    }

    [Fact]
    public void CombineAll_Enumerable_AllSuccess_ReturnsAllValuesInOrder()
    {
        IEnumerable<Result<Int32, Error>> results = [Result.Success(1), Result.Success(2), Result.Success(3)];

        var result = ResultExtensions.CombineAll(results);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(new[] { 1, 2, 3 });
    }

    [Fact]
    public void CombineAll_Enumerable_SomeFail_ReturnsAllErrorsInOrder()
    {
        IEnumerable<Result<Int32, Error>> results =
        [
            Result.Success(1),
            Result.Failure<Int32>(FirstError),
            Result.Success(2),
            Result.Failure<Int32>(SecondError)
        ];

        var result = ResultExtensions.CombineAll(results);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(new[] { FirstError, SecondError });
    }

    [Fact]
    public void CombineAll_Enumerable_Empty_ReturnsSuccessWithEmptyList()
    {
        var result = ResultExtensions.CombineAll(Enumerable.Empty<Result<Int32, Error>>());

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBeEmpty();
    }

    [Fact]
    public async Task CombineAllAsync_Params_AllSuccess_ReturnsAllValuesInOrder()
    {
        var result = await ResultExtensions.CombineAllAsync(
            Task.FromResult(Result.Success(1)),
            Task.FromResult(Result.Success(2)),
            Task.FromResult(Result.Success(3)));

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(new[] { 1, 2, 3 });
    }

    [Fact]
    public async Task CombineAllAsync_Params_SomeFail_ReturnsAllErrorsInOrder()
    {
        var result = await ResultExtensions.CombineAllAsync(
            Task.FromResult(Result.Success(1)),
            Task.FromResult(Result.Failure<Int32>(FirstError)),
            Task.FromResult(Result.Success(2)),
            Task.FromResult(Result.Failure<Int32>(SecondError)));

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(new[] { FirstError, SecondError });
    }

    [Fact]
    public async Task CombineAllAsync_Params_Empty_ReturnsSuccessWithEmptyList()
    {
        var result = await ResultExtensions.CombineAllAsync<Int32, Error>();

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBeEmpty();
    }

    [Fact]
    public async Task CombineAllAsync_Enumerable_AllSuccess_ReturnsAllValuesInOrder()
    {
        IEnumerable<Task<Result<Int32, Error>>> resultTasks =
        [
            Task.FromResult(Result.Success(1)),
            Task.FromResult(Result.Success(2)),
            Task.FromResult(Result.Success(3))
        ];

        var result = await ResultExtensions.CombineAllAsync(resultTasks);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(new[] { 1, 2, 3 });
    }

    [Fact]
    public async Task CombineAllAsync_Enumerable_SomeFail_ReturnsAllErrorsInOrder()
    {
        IEnumerable<Task<Result<Int32, Error>>> resultTasks =
        [
            Task.FromResult(Result.Success(1)),
            Task.FromResult(Result.Failure<Int32>(FirstError)),
            Task.FromResult(Result.Success(2)),
            Task.FromResult(Result.Failure<Int32>(SecondError))
        ];

        var result = await ResultExtensions.CombineAllAsync(resultTasks);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(new[] { FirstError, SecondError });
    }

    [Fact]
    public async Task CombineAllAsync_Enumerable_Empty_ReturnsSuccessWithEmptyList()
    {
        var result = await ResultExtensions.CombineAllAsync(Enumerable.Empty<Task<Result<Int32, Error>>>());

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBeEmpty();
    }
}
