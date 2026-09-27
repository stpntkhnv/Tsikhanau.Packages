namespace Tsikhanau.Railway.Tests;

public class ResultEnsureTests
{
    private static readonly Error SourceError = Error.Create("SOURCE", "source failed");
    private static readonly Error EnsureError = Error.Create("ENSURE", "ensure failed");

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
        result.Error.ShouldBeSameAs(EnsureError);
        predicate.Received(1).Invoke(2);
    }

    [Fact]
    public void Ensure_Failure_ReturnsSourceErrorWithoutCallingPredicate()
    {
        var predicate = Substitute.For<Func<Int32, Boolean>>();

        var result = Result.Failure<Int32>(SourceError).Ensure(predicate, EnsureError);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBeSameAs(SourceError);
        predicate.DidNotReceiveWithAnyArgs().Invoke(default);
    }

    [Fact]
    public void Ensure_Success_PredicateTrueWithNullError_ReturnsSourceValue()
    {
        var result = Result.Success(2).Ensure(_ => true, null!);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(2);
    }

    [Fact]
    public void Ensure_Success_PredicateFalseWithNullError_ThrowsArgumentNullException()
    {
        var exception = Should.Throw<ArgumentNullException>(() => Result.Success(2).Ensure(_ => false, null!));

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
        result.Error.ShouldBeSameAs(EnsureError);
        predicate.Received(1).Invoke(2);
    }

    [Fact]
    public async Task EnsureAsync_TaskWithSyncPredicate_Failure_ReturnsSourceErrorWithoutCallingPredicate()
    {
        var predicate = Substitute.For<Func<Int32, Boolean>>();

        var result = await Task.FromResult(Result.Failure<Int32>(SourceError)).EnsureAsync(predicate, EnsureError);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBeSameAs(SourceError);
        predicate.DidNotReceiveWithAnyArgs().Invoke(default);
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
        result.Error.ShouldBeSameAs(EnsureError);
        await predicate.Received(1).Invoke(2);
    }

    [Fact]
    public async Task EnsureAsync_TaskWithAsyncPredicate_Failure_ReturnsSourceErrorWithoutCallingPredicate()
    {
        var predicate = Substitute.For<Func<Int32, Task<Boolean>>>();

        var result = await Task.FromResult(Result.Failure<Int32>(SourceError)).EnsureAsync(predicate, EnsureError);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBeSameAs(SourceError);
        await predicate.DidNotReceiveWithAnyArgs().Invoke(default);
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
        result.Error.ShouldBeSameAs(EnsureError);
        await predicate.Received(1).Invoke(2);
    }

    [Fact]
    public async Task EnsureAsync_ResultWithAsyncPredicate_Failure_ReturnsSourceErrorWithoutCallingPredicate()
    {
        var predicate = Substitute.For<Func<Int32, Task<Boolean>>>();

        var result = await Result.Failure<Int32>(SourceError).EnsureAsync(predicate, EnsureError);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBeSameAs(SourceError);
        await predicate.DidNotReceiveWithAnyArgs().Invoke(default);
    }
}
