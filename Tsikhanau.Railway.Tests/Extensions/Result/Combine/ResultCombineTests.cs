namespace Tsikhanau.Railway.Tests;

public class ResultCombineTests
{
    private static readonly Error FirstError = Error.Create("FIRST", "first failed");
    private static readonly Error SecondError = Error.Create("SECOND", "second failed");

    [Fact]
    public void Combine_Params_AllSuccess_ReturnsValuesInOrder()
    {
        var result = ResultExtensions.Combine(Result.Success(1), Result.Success(2), Result.Success(3));

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(new[] { 1, 2, 3 });
    }

    [Fact]
    public void Combine_Params_SomeFail_ReturnsFirstError()
    {
        var result = ResultExtensions.Combine(
            Result.Success(1),
            Result.Failure<Int32>(FirstError),
            Result.Failure<Int32>(SecondError));

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(FirstError);
    }

    [Fact]
    public void Combine_Params_Empty_ReturnsEmptySuccess()
    {
        var result = ResultExtensions.Combine<Int32, Error>();

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBeEmpty();
    }

    [Fact]
    public void Combine_Enumerable_AllSuccess_ReturnsValuesInOrder()
    {
        IEnumerable<Result<Int32, Error>> results = [Result.Success(1), Result.Success(2), Result.Success(3)];

        var result = ResultExtensions.Combine(results);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(new[] { 1, 2, 3 });
    }

    [Fact]
    public void Combine_Enumerable_SomeFail_ReturnsFirstError()
    {
        IEnumerable<Result<Int32, Error>> results =
            [Result.Success(1), Result.Failure<Int32>(FirstError), Result.Failure<Int32>(SecondError)];

        var result = ResultExtensions.Combine(results);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(FirstError);
    }

    [Fact]
    public void Combine_Enumerable_Empty_ReturnsEmptySuccess()
    {
        var result = ResultExtensions.Combine(Enumerable.Empty<Result<Int32, Error>>());

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBeEmpty();
    }

    [Fact]
    public void Combine_AllSuccessWithValueTypeError_ReturnsValues()
    {
        var result = ResultExtensions.Combine(Result<String, Int32>.Success("a"), Result<String, Int32>.Success("b"));

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(new[] { "a", "b" });
    }

    [Fact]
    public void CombineIgnoreValues_Params_AllSuccess_ReturnsUnitSuccess()
    {
        var result = ResultExtensions.CombineIgnoreValues(Result.Success(1), Result.Success(2));

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(Unit.Value);
    }

    [Fact]
    public void CombineIgnoreValues_Params_SomeFail_ReturnsFirstError()
    {
        var result = ResultExtensions.CombineIgnoreValues(
            Result.Success(1),
            Result.Failure<Int32>(FirstError),
            Result.Failure<Int32>(SecondError));

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(FirstError);
    }

    [Fact]
    public void CombineIgnoreValues_Params_Empty_ReturnsUnitSuccess()
    {
        var result = ResultExtensions.CombineIgnoreValues<Int32, Error>();

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(Unit.Value);
    }

    [Fact]
    public void CombineIgnoreValues_Enumerable_AllSuccess_ReturnsUnitSuccess()
    {
        IEnumerable<Result<Int32, Error>> results = [Result.Success(1), Result.Success(2)];

        var result = ResultExtensions.CombineIgnoreValues(results);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(Unit.Value);
    }

    [Fact]
    public void CombineIgnoreValues_Enumerable_SomeFail_ReturnsFirstError()
    {
        IEnumerable<Result<Int32, Error>> results =
            [Result.Success(1), Result.Failure<Int32>(FirstError), Result.Failure<Int32>(SecondError)];

        var result = ResultExtensions.CombineIgnoreValues(results);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(FirstError);
    }

    [Fact]
    public void CombineIgnoreValues_Enumerable_Empty_ReturnsUnitSuccess()
    {
        var result = ResultExtensions.CombineIgnoreValues(Enumerable.Empty<Result<Int32, Error>>());

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(Unit.Value);
    }

    [Fact]
    public async Task CombineAsync_Params_AllSuccess_ReturnsValuesInOrder()
    {
        var result = await ResultExtensions.CombineAsync(
            Task.FromResult(Result.Success(1)),
            Task.FromResult(Result.Success(2)));

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(new[] { 1, 2 });
    }

    [Fact]
    public async Task CombineAsync_Params_SomeFail_ReturnsFirstError()
    {
        var result = await ResultExtensions.CombineAsync(
            Task.FromResult(Result.Success(1)),
            Task.FromResult(Result.Failure<Int32>(FirstError)),
            Task.FromResult(Result.Failure<Int32>(SecondError)));

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(FirstError);
    }

    [Fact]
    public async Task CombineAsync_Params_Empty_ReturnsEmptySuccess()
    {
        var result = await ResultExtensions.CombineAsync<Int32, Error>();

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBeEmpty();
    }

    [Fact]
    public async Task CombineAsync_Enumerable_AllSuccess_ReturnsValuesInOrder()
    {
        IEnumerable<Task<Result<Int32, Error>>> resultTasks =
            [Task.FromResult(Result.Success(1)), Task.FromResult(Result.Success(2))];

        var result = await ResultExtensions.CombineAsync(resultTasks);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(new[] { 1, 2 });
    }

    [Fact]
    public async Task CombineAsync_Enumerable_SomeFail_ReturnsFirstError()
    {
        IEnumerable<Task<Result<Int32, Error>>> resultTasks =
        [
            Task.FromResult(Result.Success(1)),
            Task.FromResult(Result.Failure<Int32>(FirstError)),
            Task.FromResult(Result.Failure<Int32>(SecondError))
        ];

        var result = await ResultExtensions.CombineAsync(resultTasks);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(FirstError);
    }

    [Fact]
    public async Task CombineAsync_Enumerable_Empty_ReturnsEmptySuccess()
    {
        var result = await ResultExtensions.CombineAsync(Enumerable.Empty<Task<Result<Int32, Error>>>());

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBeEmpty();
    }

    [Fact]
    public async Task CombineIgnoreValuesAsync_Params_AllSuccess_ReturnsUnitSuccess()
    {
        var result = await ResultExtensions.CombineIgnoreValuesAsync(
            Task.FromResult(Result.Success(1)),
            Task.FromResult(Result.Success(2)));

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(Unit.Value);
    }

    [Fact]
    public async Task CombineIgnoreValuesAsync_Params_SomeFail_ReturnsFirstError()
    {
        var result = await ResultExtensions.CombineIgnoreValuesAsync(
            Task.FromResult(Result.Success(1)),
            Task.FromResult(Result.Failure<Int32>(FirstError)),
            Task.FromResult(Result.Failure<Int32>(SecondError)));

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(FirstError);
    }

    [Fact]
    public async Task CombineIgnoreValuesAsync_Params_Empty_ReturnsUnitSuccess()
    {
        var result = await ResultExtensions.CombineIgnoreValuesAsync<Int32, Error>();

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(Unit.Value);
    }

    [Fact]
    public async Task CombineIgnoreValuesAsync_Enumerable_AllSuccess_ReturnsUnitSuccess()
    {
        IEnumerable<Task<Result<Int32, Error>>> resultTasks =
            [Task.FromResult(Result.Success(1)), Task.FromResult(Result.Success(2))];

        var result = await ResultExtensions.CombineIgnoreValuesAsync(resultTasks);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(Unit.Value);
    }

    [Fact]
    public async Task CombineIgnoreValuesAsync_Enumerable_SomeFail_ReturnsFirstError()
    {
        IEnumerable<Task<Result<Int32, Error>>> resultTasks =
        [
            Task.FromResult(Result.Success(1)),
            Task.FromResult(Result.Failure<Int32>(FirstError)),
            Task.FromResult(Result.Failure<Int32>(SecondError))
        ];

        var result = await ResultExtensions.CombineIgnoreValuesAsync(resultTasks);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(FirstError);
    }

    [Fact]
    public async Task CombineIgnoreValuesAsync_Enumerable_Empty_ReturnsUnitSuccess()
    {
        var result = await ResultExtensions.CombineIgnoreValuesAsync(Enumerable.Empty<Task<Result<Int32, Error>>>());

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(Unit.Value);
    }
}
