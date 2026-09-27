namespace Tsikhanau.Railway.Tests;

public class ResultCombineTests
{
    private static readonly Error FirstError = Error.Failure("first.failed", "First failed");
    private static readonly Error SecondError = Error.NotFound("second.missing", "Second missing");

    [Fact]
    public void Combine_Params_AllSuccess_ReturnsValuesInOrder()
    {
        var result = Result.Combine(Result.Success(1), Result.Success(2), Result.Success(3));

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(new[] { 1, 2, 3 });
    }

    [Fact]
    public void Combine_Params_SomeFail_ReturnsFirstError()
    {
        var result = Result.Combine(
            Result.Success(1),
            Result.Failure<Int32>(FirstError),
            Result.Failure<Int32>(SecondError));

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(FirstError);
    }

    [Fact]
    public void Combine_Params_Empty_ReturnsEmptySuccess()
    {
        var result = Result.Combine<Int32>();

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBeEmpty();
    }

    [Fact]
    public void Combine_Array_AllSuccess_ReturnsValuesInOrder()
    {
        Result<Int32>[] results = [Result.Success(1), Result.Success(2), Result.Success(3)];

        var result = Result.Combine(results);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(new[] { 1, 2, 3 });
    }

    [Fact]
    public void Combine_Array_SomeFail_ReturnsFirstError()
    {
        Result<Int32>[] results = [Result.Success(1), Result.Failure<Int32>(FirstError), Result.Failure<Int32>(SecondError)];

        var result = Result.Combine(results);

        result.Error.ShouldBe(FirstError);
    }

    [Fact]
    public void Combine_CollectionExpression_AllSuccess_ReturnsValuesInOrder()
    {
        var result = Result.Combine([Result.Success("a"), Result.Success("b")]);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(new[] { "a", "b" });
    }

    [Fact]
    public void Combine_CollectionExpression_SomeFail_ReturnsFirstError()
    {
        var result = Result.Combine([Result.Failure<String>(SecondError), Result.Failure<String>(FirstError)]);

        result.Error.ShouldBe(SecondError);
    }

    [Fact]
    public void Combine_Enumerable_AllSuccess_ReturnsValuesInOrder()
    {
        List<Result<Int32>> results = [Result.Success(1), Result.Success(2), Result.Success(3)];

        var result = results.Combine();

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(new[] { 1, 2, 3 });
    }

    [Fact]
    public void Combine_Enumerable_SomeFail_ReturnsFirstError()
    {
        List<Result<Int32>> results =
            [Result.Success(1), Result.Failure<Int32>(FirstError), Result.Failure<Int32>(SecondError)];

        var result = results.Combine();

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(FirstError);
    }

    [Fact]
    public void Combine_Enumerable_Empty_ReturnsEmptySuccess()
    {
        var result = Enumerable.Empty<Result<Int32>>().Combine();

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBeEmpty();
    }

    [Fact]
    public void Combine_Enumerable_Null_ThrowsArgumentNullException()
    {
        IEnumerable<Result<Int32>> results = null!;

        Should.Throw<ArgumentNullException>(() => results.Combine());
    }

    [Fact]
    public void CombineIgnoreValues_Params_AllSuccess_ReturnsUnitSuccess()
    {
        var result = Result.CombineIgnoreValues(Result.Success(1), Result.Success(2));

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(Unit.Value);
    }

    [Fact]
    public void CombineIgnoreValues_Params_SomeFail_ReturnsFirstError()
    {
        var result = Result.CombineIgnoreValues(
            Result.Success(1),
            Result.Failure<Int32>(FirstError),
            Result.Failure<Int32>(SecondError));

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(FirstError);
    }

    [Fact]
    public void CombineIgnoreValues_Params_Empty_ReturnsUnitSuccess()
    {
        var result = Result.CombineIgnoreValues<Int32>();

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(Unit.Value);
    }

    [Fact]
    public void CombineIgnoreValues_Array_SomeFail_ReturnsFirstError()
    {
        Result<Int32>[] results = [Result.Success(1), Result.Failure<Int32>(SecondError), Result.Failure<Int32>(FirstError)];

        var result = Result.CombineIgnoreValues(results);

        result.Error.ShouldBe(SecondError);
    }

    [Fact]
    public void CombineIgnoreValues_CollectionExpression_AllSuccess_ReturnsUnitSuccess()
    {
        var result = Result.CombineIgnoreValues([Result.Success("a"), Result.Success("b")]);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(Unit.Value);
    }

    [Fact]
    public void CombineIgnoreValues_Enumerable_AllSuccess_ReturnsUnitSuccess()
    {
        List<Result<Int32>> results = [Result.Success(1), Result.Success(2)];

        var result = results.CombineIgnoreValues();

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(Unit.Value);
    }

    [Fact]
    public void CombineIgnoreValues_Enumerable_SomeFail_ReturnsFirstError()
    {
        List<Result<Int32>> results =
            [Result.Success(1), Result.Failure<Int32>(FirstError), Result.Failure<Int32>(SecondError)];

        var result = results.CombineIgnoreValues();

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(FirstError);
    }

    [Fact]
    public void CombineIgnoreValues_Enumerable_Empty_ReturnsUnitSuccess()
    {
        var result = Enumerable.Empty<Result<Int32>>().CombineIgnoreValues();

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(Unit.Value);
    }

    [Fact]
    public void CombineIgnoreValues_Enumerable_Null_ThrowsArgumentNullException()
    {
        IEnumerable<Result<Int32>> results = null!;

        Should.Throw<ArgumentNullException>(() => results.CombineIgnoreValues());
    }

    [Fact]
    public async Task CombineAsync_Params_AllSuccess_ReturnsValuesInOrder()
    {
        var result = await Result.CombineAsync(
            Task.FromResult(Result.Success(1)),
            Task.FromResult(Result.Success(2)));

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(new[] { 1, 2 });
    }

    [Fact]
    public async Task CombineAsync_Params_SomeFail_ReturnsFirstError()
    {
        var result = await Result.CombineAsync(
            Task.FromResult(Result.Success(1)),
            Task.FromResult(Result.Failure<Int32>(FirstError)),
            Task.FromResult(Result.Failure<Int32>(SecondError)));

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(FirstError);
    }

    [Fact]
    public async Task CombineAsync_Params_Empty_ReturnsEmptySuccess()
    {
        var result = await Result.CombineAsync<Int32>();

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBeEmpty();
    }

    [Fact]
    public async Task CombineAsync_Params_TasksCompleteOutOfOrder_ReturnsValuesInInputOrder()
    {
        var first = new TaskCompletionSource<Result<Int32>>();
        var second = new TaskCompletionSource<Result<Int32>>();

        var combined = Result.CombineAsync(first.Task, second.Task);
        second.SetResult(Result.Success(2));
        first.SetResult(Result.Success(1));
        var result = await combined;

        result.Value.ShouldBe(new[] { 1, 2 });
    }

    [Fact]
    public async Task CombineAsync_Params_FailuresCompleteOutOfOrder_ReturnsFirstErrorByPosition()
    {
        var first = new TaskCompletionSource<Result<Int32>>();
        var second = new TaskCompletionSource<Result<Int32>>();

        var combined = Result.CombineAsync(first.Task, second.Task);
        second.SetResult(Result.Failure<Int32>(SecondError));
        first.SetResult(Result.Failure<Int32>(FirstError));
        var result = await combined;

        result.Error.ShouldBe(FirstError);
    }

    [Fact]
    public async Task CombineAsync_Enumerable_AllSuccess_ReturnsValuesInOrder()
    {
        List<Task<Result<Int32>>> resultTasks =
            [Task.FromResult(Result.Success(1)), Task.FromResult(Result.Success(2))];

        var result = await resultTasks.CombineAsync();

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(new[] { 1, 2 });
    }

    [Fact]
    public async Task CombineAsync_Enumerable_SomeFail_ReturnsFirstError()
    {
        List<Task<Result<Int32>>> resultTasks =
        [
            Task.FromResult(Result.Success(1)),
            Task.FromResult(Result.Failure<Int32>(FirstError)),
            Task.FromResult(Result.Failure<Int32>(SecondError))
        ];

        var result = await resultTasks.CombineAsync();

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(FirstError);
    }

    [Fact]
    public async Task CombineAsync_Enumerable_Empty_ReturnsEmptySuccess()
    {
        var result = await Enumerable.Empty<Task<Result<Int32>>>().CombineAsync();

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBeEmpty();
    }

    [Fact]
    public async Task CombineIgnoreValuesAsync_Params_AllSuccess_ReturnsUnitSuccess()
    {
        var result = await Result.CombineIgnoreValuesAsync(
            Task.FromResult(Result.Success(1)),
            Task.FromResult(Result.Success(2)));

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(Unit.Value);
    }

    [Fact]
    public async Task CombineIgnoreValuesAsync_Params_SomeFail_ReturnsFirstError()
    {
        var result = await Result.CombineIgnoreValuesAsync(
            Task.FromResult(Result.Success(1)),
            Task.FromResult(Result.Failure<Int32>(FirstError)),
            Task.FromResult(Result.Failure<Int32>(SecondError)));

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(FirstError);
    }

    [Fact]
    public async Task CombineIgnoreValuesAsync_Params_Empty_ReturnsUnitSuccess()
    {
        var result = await Result.CombineIgnoreValuesAsync<Int32>();

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(Unit.Value);
    }

    [Fact]
    public async Task CombineIgnoreValuesAsync_Enumerable_AllSuccess_ReturnsUnitSuccess()
    {
        List<Task<Result<Int32>>> resultTasks =
            [Task.FromResult(Result.Success(1)), Task.FromResult(Result.Success(2))];

        var result = await resultTasks.CombineIgnoreValuesAsync();

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(Unit.Value);
    }

    [Fact]
    public async Task CombineIgnoreValuesAsync_Enumerable_SomeFail_ReturnsFirstError()
    {
        List<Task<Result<Int32>>> resultTasks =
        [
            Task.FromResult(Result.Success(1)),
            Task.FromResult(Result.Failure<Int32>(FirstError)),
            Task.FromResult(Result.Failure<Int32>(SecondError))
        ];

        var result = await resultTasks.CombineIgnoreValuesAsync();

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(FirstError);
    }

    [Fact]
    public async Task CombineIgnoreValuesAsync_Enumerable_Empty_ReturnsUnitSuccess()
    {
        var result = await Enumerable.Empty<Task<Result<Int32>>>().CombineIgnoreValuesAsync();

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(Unit.Value);
    }
}
