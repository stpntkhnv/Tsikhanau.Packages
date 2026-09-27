namespace Tsikhanau.Railway.Tests;

public class ResultBindTests
{
    private static readonly Error SourceError = Error.Failure("source.failed", "Source failed");
    private static readonly Error BinderError = Error.Failure("binder.failed", "Binder failed");

    [Fact]
    public void Bind_Success_ReturnsBinderResult()
    {
        var result = Result.Success(2).Bind(x => Result.Success(x.ToString()));

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe("2");
    }

    [Fact]
    public void Bind_Success_BinderFails_ReturnsBinderError()
    {
        var result = Result.Success(2).Bind(_ => Result.Failure<String>(BinderError));

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(BinderError);
    }

    [Fact]
    public void Bind_Failure_ReturnsSourceErrorWithoutCallingBinder()
    {
        var binder = Substitute.For<Func<Int32, Result<String>>>();

        var result = Result.Failure<Int32>(SourceError).Bind(binder);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(SourceError);
        binder.DidNotReceiveWithAnyArgs().Invoke(default);
    }

    [Fact]
    public async Task BindAsync_TaskWithSyncBinder_Success_ReturnsBinderResult()
    {
        var result = await Task.FromResult(Result.Success(2)).BindAsync(x => Result.Success(x * 10));

        result.Value.ShouldBe(20);
    }

    [Fact]
    public async Task BindAsync_TaskWithSyncBinder_Failure_ReturnsSourceErrorWithoutCallingBinder()
    {
        var binder = Substitute.For<Func<Int32, Result<Int32>>>();

        var result = await Task.FromResult(Result.Failure<Int32>(SourceError)).BindAsync(binder);

        result.Error.ShouldBe(SourceError);
        binder.DidNotReceiveWithAnyArgs().Invoke(default);
    }

    [Fact]
    public async Task BindAsync_TaskWithAsyncBinder_Success_ReturnsBinderResult()
    {
        var result = await Task.FromResult(Result.Success(2))
            .BindAsync(x => Task.FromResult(Result.Success(x * 10)));

        result.Value.ShouldBe(20);
    }

    [Fact]
    public async Task BindAsync_TaskWithAsyncBinder_BinderFails_ReturnsBinderError()
    {
        var result = await Task.FromResult(Result.Success(2))
            .BindAsync(_ => Task.FromResult(Result.Failure<Int32>(BinderError)));

        result.Error.ShouldBe(BinderError);
    }

    [Fact]
    public async Task BindAsync_TaskWithAsyncBinder_Failure_ReturnsSourceErrorWithoutCallingBinder()
    {
        var binder = Substitute.For<Func<Int32, Task<Result<Int32>>>>();

        var result = await Task.FromResult(Result.Failure<Int32>(SourceError)).BindAsync(binder);

        result.Error.ShouldBe(SourceError);
        await binder.DidNotReceiveWithAnyArgs().Invoke(default);
    }

    [Fact]
    public async Task BindAsync_ResultWithAsyncBinder_Success_ReturnsBinderResult()
    {
        var result = await Result.Success(2).BindAsync(x => Task.FromResult(Result.Success(x * 10)));

        result.Value.ShouldBe(20);
    }

    [Fact]
    public async Task BindAsync_ResultWithAsyncBinder_Failure_ReturnsSourceErrorWithoutCallingBinder()
    {
        var binder = Substitute.For<Func<Int32, Task<Result<Int32>>>>();

        var result = await Result.Failure<Int32>(SourceError).BindAsync(binder);

        result.Error.ShouldBe(SourceError);
        await binder.DidNotReceiveWithAnyArgs().Invoke(default);
    }
}
