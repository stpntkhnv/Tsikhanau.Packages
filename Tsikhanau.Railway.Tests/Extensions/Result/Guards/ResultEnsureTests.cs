namespace Tsikhanau.Railway.Tests;

public class ResultEnsureTests
{
    private static readonly Error SourceError = Error.Failure("source.failed", "Source failed");
    private static readonly Error EnsureError = Error.Conflict("ensure.failed", "Ensure failed");

    [Fact]
    public void Ensure_Success_PredicateTrue_ReturnsSourceValue()
    {
        var predicate = Substitute.For<Func<Int32, Boolean>>();
        predicate(2).Returns(true);

        var result = Result.Success(2).Ensure(predicate, EnsureError);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(2);
        predicate.Received(1).Invoke(2);
    }

    [Fact]
    public void Ensure_Success_PredicateFalse_ReturnsGivenError()
    {
        var predicate = Substitute.For<Func<Int32, Boolean>>();
        predicate(2).Returns(false);

        var result = Result.Success(2).Ensure(predicate, EnsureError);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(EnsureError);
        predicate.Received(1).Invoke(2);
    }

    [Fact]
    public void Ensure_Failure_ReturnsSourceErrorWithoutCallingPredicate()
    {
        var predicate = Substitute.For<Func<Int32, Boolean>>();

        var result = Result.Failure<Int32>(SourceError).Ensure(predicate, EnsureError);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(SourceError);
        predicate.DidNotReceiveWithAnyArgs().Invoke(default);
    }

    [Fact]
    public void Ensure_Success_PredicateTrueWithNullError_ReturnsSourceValue()
    {
        Error error = null!;

        var result = Result.Success(2).Ensure(_ => true, error);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(2);
    }

    [Fact]
    public void Ensure_Success_PredicateFalseWithNullError_ThrowsArgumentNullException()
    {
        Error error = null!;

        var exception = Should.Throw<ArgumentNullException>(() => Result.Success(2).Ensure(_ => false, error));

        exception.ParamName.ShouldBe("error");
    }

    [Fact]
    public void Ensure_WithErrorFactory_Success_PredicateTrue_ReturnsSourceValueWithoutCallingFactory()
    {
        var predicate = Substitute.For<Func<Int32, Boolean>>();
        predicate(2).Returns(true);
        var errorFactory = Substitute.For<Func<Int32, Error>>();

        var result = Result.Success(2).Ensure(predicate, errorFactory);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(2);
        predicate.Received(1).Invoke(2);
        errorFactory.DidNotReceiveWithAnyArgs().Invoke(default);
    }

    [Fact]
    public void Ensure_WithErrorFactory_Success_PredicateFalse_ReturnsErrorBuiltFromValue()
    {
        var predicate = Substitute.For<Func<Int32, Boolean>>();
        predicate(2).Returns(false);
        var errorFactory = Substitute.For<Func<Int32, Error>>();
        errorFactory(2).Returns(EnsureError);

        var result = Result.Success(2).Ensure(predicate, errorFactory);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(EnsureError);
        predicate.Received(1).Invoke(2);
        errorFactory.Received(1).Invoke(2);
    }

    [Fact]
    public void Ensure_WithErrorFactory_Failure_ReturnsSourceErrorWithoutCallingPredicateOrFactory()
    {
        var predicate = Substitute.For<Func<Int32, Boolean>>();
        var errorFactory = Substitute.For<Func<Int32, Error>>();

        var result = Result.Failure<Int32>(SourceError).Ensure(predicate, errorFactory);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(SourceError);
        predicate.DidNotReceiveWithAnyArgs().Invoke(default);
        errorFactory.DidNotReceiveWithAnyArgs().Invoke(default);
    }

    [Fact]
    public void Ensure_WithErrorFactory_Success_PredicateFalse_FactoryReturnsNull_ThrowsArgumentNullException()
    {
        Func<Int32, Error> errorFactory = _ => null!;

        var exception = Should.Throw<ArgumentNullException>(() => Result.Success(2).Ensure(_ => false, errorFactory));

        exception.ParamName.ShouldBe("error");
    }

    [Fact]
    public async Task EnsureAsync_TaskWithSyncPredicate_Success_PredicateTrue_ReturnsSourceValue()
    {
        var predicate = Substitute.For<Func<Int32, Boolean>>();
        predicate(2).Returns(true);

        var result = await Task.FromResult(Result.Success(2)).EnsureAsync(predicate, EnsureError);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(2);
        predicate.Received(1).Invoke(2);
    }

    [Fact]
    public async Task EnsureAsync_TaskWithSyncPredicate_Success_PredicateFalse_ReturnsGivenError()
    {
        var predicate = Substitute.For<Func<Int32, Boolean>>();
        predicate(2).Returns(false);

        var result = await Task.FromResult(Result.Success(2)).EnsureAsync(predicate, EnsureError);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(EnsureError);
        predicate.Received(1).Invoke(2);
    }

    [Fact]
    public async Task EnsureAsync_TaskWithSyncPredicate_Failure_ReturnsSourceErrorWithoutCallingPredicate()
    {
        var predicate = Substitute.For<Func<Int32, Boolean>>();

        var result = await Task.FromResult(Result.Failure<Int32>(SourceError)).EnsureAsync(predicate, EnsureError);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(SourceError);
        predicate.DidNotReceiveWithAnyArgs().Invoke(default);
    }

    [Fact]
    public async Task EnsureAsync_TaskWithSyncPredicateAndErrorFactory_Success_PredicateTrue_ReturnsSourceValueWithoutCallingFactory()
    {
        var predicate = Substitute.For<Func<Int32, Boolean>>();
        predicate(2).Returns(true);
        var errorFactory = Substitute.For<Func<Int32, Error>>();

        var result = await Task.FromResult(Result.Success(2)).EnsureAsync(predicate, errorFactory);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(2);
        predicate.Received(1).Invoke(2);
        errorFactory.DidNotReceiveWithAnyArgs().Invoke(default);
    }

    [Fact]
    public async Task EnsureAsync_TaskWithSyncPredicateAndErrorFactory_Success_PredicateFalse_ReturnsErrorBuiltFromValue()
    {
        var predicate = Substitute.For<Func<Int32, Boolean>>();
        predicate(2).Returns(false);
        var errorFactory = Substitute.For<Func<Int32, Error>>();
        errorFactory(2).Returns(EnsureError);

        var result = await Task.FromResult(Result.Success(2)).EnsureAsync(predicate, errorFactory);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(EnsureError);
        predicate.Received(1).Invoke(2);
        errorFactory.Received(1).Invoke(2);
    }

    [Fact]
    public async Task EnsureAsync_TaskWithSyncPredicateAndErrorFactory_Failure_ReturnsSourceErrorWithoutCallingPredicateOrFactory()
    {
        var predicate = Substitute.For<Func<Int32, Boolean>>();
        var errorFactory = Substitute.For<Func<Int32, Error>>();

        var result = await Task.FromResult(Result.Failure<Int32>(SourceError)).EnsureAsync(predicate, errorFactory);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(SourceError);
        predicate.DidNotReceiveWithAnyArgs().Invoke(default);
        errorFactory.DidNotReceiveWithAnyArgs().Invoke(default);
    }

    [Fact]
    public async Task EnsureAsync_TaskWithAsyncPredicate_Success_PredicateTrue_ReturnsSourceValue()
    {
        var predicate = Substitute.For<Func<Int32, Task<Boolean>>>();
        predicate(2).Returns(Task.FromResult(true));

        var result = await Task.FromResult(Result.Success(2)).EnsureAsync(predicate, EnsureError);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(2);
        await predicate.Received(1).Invoke(2);
    }

    [Fact]
    public async Task EnsureAsync_TaskWithAsyncPredicate_Success_PredicateFalse_ReturnsGivenError()
    {
        var predicate = Substitute.For<Func<Int32, Task<Boolean>>>();
        predicate(2).Returns(Task.FromResult(false));

        var result = await Task.FromResult(Result.Success(2)).EnsureAsync(predicate, EnsureError);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(EnsureError);
        await predicate.Received(1).Invoke(2);
    }

    [Fact]
    public async Task EnsureAsync_TaskWithAsyncPredicate_Failure_ReturnsSourceErrorWithoutCallingPredicate()
    {
        var predicate = Substitute.For<Func<Int32, Task<Boolean>>>();

        var result = await Task.FromResult(Result.Failure<Int32>(SourceError)).EnsureAsync(predicate, EnsureError);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(SourceError);
        await predicate.DidNotReceiveWithAnyArgs().Invoke(default);
    }

    [Fact]
    public async Task EnsureAsync_TaskWithAsyncPredicateAndErrorFactory_Success_PredicateTrue_ReturnsSourceValueWithoutCallingFactory()
    {
        var predicate = Substitute.For<Func<Int32, Task<Boolean>>>();
        predicate(2).Returns(Task.FromResult(true));
        var errorFactory = Substitute.For<Func<Int32, Error>>();

        var result = await Task.FromResult(Result.Success(2)).EnsureAsync(predicate, errorFactory);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(2);
        await predicate.Received(1).Invoke(2);
        errorFactory.DidNotReceiveWithAnyArgs().Invoke(default);
    }

    [Fact]
    public async Task EnsureAsync_TaskWithAsyncPredicateAndErrorFactory_Success_PredicateFalse_ReturnsErrorBuiltFromValue()
    {
        var predicate = Substitute.For<Func<Int32, Task<Boolean>>>();
        predicate(2).Returns(Task.FromResult(false));
        var errorFactory = Substitute.For<Func<Int32, Error>>();
        errorFactory(2).Returns(EnsureError);

        var result = await Task.FromResult(Result.Success(2)).EnsureAsync(predicate, errorFactory);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(EnsureError);
        await predicate.Received(1).Invoke(2);
        errorFactory.Received(1).Invoke(2);
    }

    [Fact]
    public async Task EnsureAsync_TaskWithAsyncPredicateAndErrorFactory_Failure_ReturnsSourceErrorWithoutCallingPredicateOrFactory()
    {
        var predicate = Substitute.For<Func<Int32, Task<Boolean>>>();
        var errorFactory = Substitute.For<Func<Int32, Error>>();

        var result = await Task.FromResult(Result.Failure<Int32>(SourceError)).EnsureAsync(predicate, errorFactory);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(SourceError);
        await predicate.DidNotReceiveWithAnyArgs().Invoke(default);
        errorFactory.DidNotReceiveWithAnyArgs().Invoke(default);
    }

    [Fact]
    public async Task EnsureAsync_ResultWithAsyncPredicate_Success_PredicateTrue_ReturnsSourceValue()
    {
        var predicate = Substitute.For<Func<Int32, Task<Boolean>>>();
        predicate(2).Returns(Task.FromResult(true));

        var result = await Result.Success(2).EnsureAsync(predicate, EnsureError);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(2);
        await predicate.Received(1).Invoke(2);
    }

    [Fact]
    public async Task EnsureAsync_ResultWithAsyncPredicate_Success_PredicateFalse_ReturnsGivenError()
    {
        var predicate = Substitute.For<Func<Int32, Task<Boolean>>>();
        predicate(2).Returns(Task.FromResult(false));

        var result = await Result.Success(2).EnsureAsync(predicate, EnsureError);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(EnsureError);
        await predicate.Received(1).Invoke(2);
    }

    [Fact]
    public async Task EnsureAsync_ResultWithAsyncPredicate_Failure_ReturnsSourceErrorWithoutCallingPredicate()
    {
        var predicate = Substitute.For<Func<Int32, Task<Boolean>>>();

        var result = await Result.Failure<Int32>(SourceError).EnsureAsync(predicate, EnsureError);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(SourceError);
        await predicate.DidNotReceiveWithAnyArgs().Invoke(default);
    }

    [Fact]
    public async Task EnsureAsync_ResultWithAsyncPredicateAndErrorFactory_Success_PredicateTrue_ReturnsSourceValueWithoutCallingFactory()
    {
        var predicate = Substitute.For<Func<Int32, Task<Boolean>>>();
        predicate(2).Returns(Task.FromResult(true));
        var errorFactory = Substitute.For<Func<Int32, Error>>();

        var result = await Result.Success(2).EnsureAsync(predicate, errorFactory);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(2);
        await predicate.Received(1).Invoke(2);
        errorFactory.DidNotReceiveWithAnyArgs().Invoke(default);
    }

    [Fact]
    public async Task EnsureAsync_ResultWithAsyncPredicateAndErrorFactory_Success_PredicateFalse_ReturnsErrorBuiltFromValue()
    {
        var predicate = Substitute.For<Func<Int32, Task<Boolean>>>();
        predicate(2).Returns(Task.FromResult(false));
        var errorFactory = Substitute.For<Func<Int32, Error>>();
        errorFactory(2).Returns(EnsureError);

        var result = await Result.Success(2).EnsureAsync(predicate, errorFactory);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(EnsureError);
        await predicate.Received(1).Invoke(2);
        errorFactory.Received(1).Invoke(2);
    }

    [Fact]
    public async Task EnsureAsync_ResultWithAsyncPredicateAndErrorFactory_Failure_ReturnsSourceErrorWithoutCallingPredicateOrFactory()
    {
        var predicate = Substitute.For<Func<Int32, Task<Boolean>>>();
        var errorFactory = Substitute.For<Func<Int32, Error>>();

        var result = await Result.Failure<Int32>(SourceError).EnsureAsync(predicate, errorFactory);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(SourceError);
        await predicate.DidNotReceiveWithAnyArgs().Invoke(default);
        errorFactory.DidNotReceiveWithAnyArgs().Invoke(default);
    }
}
