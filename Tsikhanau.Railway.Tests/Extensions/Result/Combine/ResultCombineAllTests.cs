namespace Tsikhanau.Railway.Tests;

public class ResultCombineAllTests
{
    private static readonly Error FirstError = Error.Failure("first.failed", "First failed");
    private static readonly Error MissingError = Error.NotFound("user.missing", "User missing");
    private static readonly Error OtherMissingError = Error.NotFound("order.missing", "Order missing");
    private static readonly ValidationError NameError = Error.Validation("name", "Name is required");
    private static readonly ValidationError AgeError = Error.Validation("age", "Age must be positive");
    private static readonly ValidationError AddressErrors = ValidationError.From(
    [
        new FieldError("street", "Street is required"),
        new FieldError("city", "City is required")
    ]);

    private static readonly FieldError[] MergedFieldErrors =
    [
        new FieldError("name", "Name is required"),
        new FieldError("street", "Street is required"),
        new FieldError("city", "City is required"),
        new FieldError("age", "Age must be positive")
    ];

    [Fact]
    public void CombineAll_Params_AllSuccess_ReturnsValuesInOrder()
    {
        var result = Result.CombineAll(Result.Success(1), Result.Success(2), Result.Success(3));

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(new[] { 1, 2, 3 });
    }

    [Fact]
    public void CombineAll_Params_OneFailure_ReturnsThatError()
    {
        var result = Result.CombineAll(
            Result.Success(1),
            Result.Failure<Int32>(FirstError),
            Result.Success(3));

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBeSameAs(FirstError);
    }

    [Fact]
    public void CombineAll_Params_SeveralValidationErrors_ReturnsMergedValidationError()
    {
        var result = Result.CombineAll(
            Result.Failure<Int32>(NameError),
            Result.Success(2),
            Result.Failure<Int32>(AddressErrors),
            Result.Failure<Int32>(AgeError));

        var error = result.Error.ShouldBeOfType<ValidationError>();
        error.FieldErrors.ShouldBe(MergedFieldErrors);
        error.Kind.ShouldBe(ErrorKind.Validation);
        error.Code.ShouldBe(ValidationError.DefaultCode);
        error.Message.ShouldBe(
            "name: Name is required; street: Street is required; city: City is required; age: Age must be positive");
    }

    [Fact]
    public void CombineAll_Params_MixedErrors_ReturnsAggregateErrorWithErrorsInOrder()
    {
        var result = Result.CombineAll(
            Result.Failure<Int32>(FirstError),
            Result.Success(2),
            Result.Failure<Int32>(NameError),
            Result.Failure<Int32>(MissingError));

        var error = result.Error.ShouldBeOfType<AggregateError>();
        error.Errors.ShouldBe(new[] { FirstError, NameError, MissingError });
        error.Kind.ShouldBe(ErrorKind.Failure);
        error.Code.ShouldBe(AggregateError.DefaultCode);
        error.Message.ShouldBe("First failed; name: Name is required; User missing");
    }

    [Fact]
    public void CombineAll_Params_ErrorsOfSameKind_ReturnsAggregateErrorWithSharedKind()
    {
        var result = Result.CombineAll(Result.Failure<Int32>(MissingError), Result.Failure<Int32>(OtherMissingError));

        var error = result.Error.ShouldBeOfType<AggregateError>();
        error.Errors.ShouldBe(new[] { MissingError, OtherMissingError });
        error.Kind.ShouldBe(ErrorKind.NotFound);
    }

    [Fact]
    public void CombineAll_Params_ValidationErrorAndPlainErrorOfValidationKind_ReturnsAggregateErrorWithValidationKind()
    {
        var plainValidationKindError = new Error(ErrorKind.Validation, "custom.invalid", "Custom invalid");

        var result = Result.CombineAll(Result.Failure<Int32>(NameError), Result.Failure<Int32>(plainValidationKindError));

        var error = result.Error.ShouldBeOfType<AggregateError>();
        error.Errors.ShouldBe(new[] { NameError, plainValidationKindError });
        error.Kind.ShouldBe(ErrorKind.Validation);
    }

    [Fact]
    public void CombineAll_Params_Empty_ReturnsEmptySuccess()
    {
        var result = Result.CombineAll<Int32>();

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBeEmpty();
    }

    [Fact]
    public void CombineAll_Array_SeveralValidationErrors_ReturnsMergedValidationError()
    {
        Result<Int32>[] results =
        [
            Result.Failure<Int32>(NameError),
            Result.Failure<Int32>(AddressErrors),
            Result.Failure<Int32>(AgeError)
        ];

        var result = Result.CombineAll(results);

        result.Error.ShouldBeOfType<ValidationError>().FieldErrors.ShouldBe(MergedFieldErrors);
    }

    [Fact]
    public void CombineAll_CollectionExpression_AllSuccess_ReturnsValuesInOrder()
    {
        var result = Result.CombineAll([Result.Success("a"), Result.Success("b")]);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(new[] { "a", "b" });
    }

    [Fact]
    public void CombineAll_CollectionExpression_MixedErrors_ReturnsAggregateErrorWithErrorsInOrder()
    {
        var result = Result.CombineAll([Result.Failure<String>(MissingError), Result.Failure<String>(FirstError)]);

        var error = result.Error.ShouldBeOfType<AggregateError>();
        error.Errors.ShouldBe(new[] { MissingError, FirstError });
        error.Kind.ShouldBe(ErrorKind.Failure);
    }

    [Fact]
    public void CombineAll_Enumerable_AllSuccess_ReturnsValuesInOrder()
    {
        List<Result<Int32>> results = [Result.Success(1), Result.Success(2), Result.Success(3)];

        var result = results.CombineAll();

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(new[] { 1, 2, 3 });
    }

    [Fact]
    public void CombineAll_Enumerable_OneFailure_ReturnsThatError()
    {
        List<Result<Int32>> results = [Result.Success(1), Result.Failure<Int32>(NameError)];

        var result = results.CombineAll();

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBeSameAs(NameError);
    }

    [Fact]
    public void CombineAll_Enumerable_SeveralValidationErrors_ReturnsMergedValidationError()
    {
        List<Result<Int32>> results =
        [
            Result.Failure<Int32>(NameError),
            Result.Success(2),
            Result.Failure<Int32>(AddressErrors),
            Result.Failure<Int32>(AgeError)
        ];

        var result = results.CombineAll();

        result.Error.ShouldBeOfType<ValidationError>().FieldErrors.ShouldBe(MergedFieldErrors);
    }

    [Fact]
    public void CombineAll_Enumerable_MixedErrors_ReturnsAggregateErrorWithErrorsInOrder()
    {
        List<Result<Int32>> results =
        [
            Result.Failure<Int32>(FirstError),
            Result.Success(2),
            Result.Failure<Int32>(NameError),
            Result.Failure<Int32>(MissingError)
        ];

        var result = results.CombineAll();

        var error = result.Error.ShouldBeOfType<AggregateError>();
        error.Errors.ShouldBe(new[] { FirstError, NameError, MissingError });
        error.Kind.ShouldBe(ErrorKind.Failure);
    }

    [Fact]
    public void CombineAll_Enumerable_Empty_ReturnsEmptySuccess()
    {
        var result = Enumerable.Empty<Result<Int32>>().CombineAll();

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBeEmpty();
    }

    [Fact]
    public void CombineAll_Enumerable_Null_ThrowsArgumentNullException()
    {
        IEnumerable<Result<Int32>> results = null!;

        Should.Throw<ArgumentNullException>(() => results.CombineAll());
    }

    [Fact]
    public async Task CombineAllAsync_Params_AllSuccess_ReturnsValuesInOrder()
    {
        var result = await Result.CombineAllAsync(
            Task.FromResult(Result.Success(1)),
            Task.FromResult(Result.Success(2)),
            Task.FromResult(Result.Success(3)));

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(new[] { 1, 2, 3 });
    }

    [Fact]
    public async Task CombineAllAsync_Params_OneFailure_ReturnsThatError()
    {
        var result = await Result.CombineAllAsync(
            Task.FromResult(Result.Success(1)),
            Task.FromResult(Result.Failure<Int32>(MissingError)));

        result.Error.ShouldBeSameAs(MissingError);
    }

    [Fact]
    public async Task CombineAllAsync_Params_SeveralValidationErrors_ReturnsMergedValidationError()
    {
        var result = await Result.CombineAllAsync(
            Task.FromResult(Result.Failure<Int32>(NameError)),
            Task.FromResult(Result.Failure<Int32>(AddressErrors)),
            Task.FromResult(Result.Success(3)),
            Task.FromResult(Result.Failure<Int32>(AgeError)));

        result.Error.ShouldBeOfType<ValidationError>().FieldErrors.ShouldBe(MergedFieldErrors);
    }

    [Fact]
    public async Task CombineAllAsync_Params_MixedErrors_ReturnsAggregateErrorWithErrorsInOrder()
    {
        var result = await Result.CombineAllAsync(
            Task.FromResult(Result.Failure<Int32>(FirstError)),
            Task.FromResult(Result.Failure<Int32>(NameError)),
            Task.FromResult(Result.Failure<Int32>(MissingError)));

        var error = result.Error.ShouldBeOfType<AggregateError>();
        error.Errors.ShouldBe(new[] { FirstError, NameError, MissingError });
        error.Kind.ShouldBe(ErrorKind.Failure);
    }

    [Fact]
    public async Task CombineAllAsync_Params_Empty_ReturnsEmptySuccess()
    {
        var result = await Result.CombineAllAsync<Int32>();

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBeEmpty();
    }

    [Fact]
    public async Task CombineAllAsync_Params_TasksCompleteOutOfOrder_ReturnsErrorsInInputOrder()
    {
        var first = new TaskCompletionSource<Result<Int32>>();
        var second = new TaskCompletionSource<Result<Int32>>();

        var combined = Result.CombineAllAsync(first.Task, second.Task);
        second.SetResult(Result.Failure<Int32>(MissingError));
        first.SetResult(Result.Failure<Int32>(OtherMissingError));
        var result = await combined;

        result.Error.ShouldBeOfType<AggregateError>().Errors.ShouldBe(new[] { OtherMissingError, MissingError });
    }

    [Fact]
    public async Task CombineAllAsync_Enumerable_AllSuccess_ReturnsValuesInOrder()
    {
        List<Task<Result<Int32>>> resultTasks =
        [
            Task.FromResult(Result.Success(1)),
            Task.FromResult(Result.Success(2)),
            Task.FromResult(Result.Success(3))
        ];

        var result = await resultTasks.CombineAllAsync();

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(new[] { 1, 2, 3 });
    }

    [Fact]
    public async Task CombineAllAsync_Enumerable_OneFailure_ReturnsThatError()
    {
        List<Task<Result<Int32>>> resultTasks =
            [Task.FromResult(Result.Failure<Int32>(AgeError)), Task.FromResult(Result.Success(2))];

        var result = await resultTasks.CombineAllAsync();

        result.Error.ShouldBeSameAs(AgeError);
    }

    [Fact]
    public async Task CombineAllAsync_Enumerable_SeveralValidationErrors_ReturnsMergedValidationError()
    {
        List<Task<Result<Int32>>> resultTasks =
        [
            Task.FromResult(Result.Failure<Int32>(NameError)),
            Task.FromResult(Result.Failure<Int32>(AddressErrors)),
            Task.FromResult(Result.Failure<Int32>(AgeError))
        ];

        var result = await resultTasks.CombineAllAsync();

        result.Error.ShouldBeOfType<ValidationError>().FieldErrors.ShouldBe(MergedFieldErrors);
    }

    [Fact]
    public async Task CombineAllAsync_Enumerable_MixedErrors_ReturnsAggregateErrorWithErrorsInOrder()
    {
        List<Task<Result<Int32>>> resultTasks =
        [
            Task.FromResult(Result.Failure<Int32>(MissingError)),
            Task.FromResult(Result.Success(2)),
            Task.FromResult(Result.Failure<Int32>(AgeError))
        ];

        var result = await resultTasks.CombineAllAsync();

        var error = result.Error.ShouldBeOfType<AggregateError>();
        error.Errors.ShouldBe(new[] { MissingError, AgeError });
        error.Kind.ShouldBe(ErrorKind.Failure);
    }

    [Fact]
    public async Task CombineAllAsync_Enumerable_Empty_ReturnsEmptySuccess()
    {
        var result = await Enumerable.Empty<Task<Result<Int32>>>().CombineAllAsync();

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBeEmpty();
    }
}
