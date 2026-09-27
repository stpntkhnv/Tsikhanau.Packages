namespace Tsikhanau.Railway.Tests;

public class ResultTapErrorTests
{
    private static readonly Error SourceError = Error.Failure("source.failed", "Source failed");

    [Fact]
    public void TapError_Failure_CallsActionAndReturnsSourceResult()
    {
        var action = Substitute.For<Action<Error>>();
        var source = Result.Failure<Int32>(SourceError);

        var result = source.TapError(action);

        result.ShouldBe(source);
        result.Error.ShouldBe(SourceError);
        action.Received(1).Invoke(SourceError);
    }

    [Fact]
    public void TapError_Success_ReturnsSourceResultWithoutCallingAction()
    {
        var action = Substitute.For<Action<Error>>();
        var source = Result.Success(2);

        var result = source.TapError(action);

        result.ShouldBe(source);
        action.DidNotReceiveWithAnyArgs().Invoke(default!);
    }

    [Fact]
    public async Task TapErrorAsync_TaskWithSyncAction_Failure_CallsActionAndReturnsSourceResult()
    {
        var action = Substitute.For<Action<Error>>();
        var source = Result.Failure<Int32>(SourceError);

        var result = await Task.FromResult(source).TapErrorAsync(action);

        result.ShouldBe(source);
        result.Error.ShouldBe(SourceError);
        action.Received(1).Invoke(SourceError);
    }

    [Fact]
    public async Task TapErrorAsync_TaskWithSyncAction_Success_ReturnsSourceResultWithoutCallingAction()
    {
        var action = Substitute.For<Action<Error>>();
        var source = Result.Success(2);

        var result = await Task.FromResult(source).TapErrorAsync(action);

        result.ShouldBe(source);
        action.DidNotReceiveWithAnyArgs().Invoke(default!);
    }

    [Fact]
    public async Task TapErrorAsync_TaskWithAsyncAction_Failure_CallsActionAndReturnsSourceResult()
    {
        var action = Substitute.For<Func<Error, Task>>();
        var source = Result.Failure<Int32>(SourceError);

        var result = await Task.FromResult(source).TapErrorAsync(action);

        result.ShouldBe(source);
        result.Error.ShouldBe(SourceError);
        await action.Received(1).Invoke(SourceError);
    }

    [Fact]
    public async Task TapErrorAsync_TaskWithAsyncAction_Success_ReturnsSourceResultWithoutCallingAction()
    {
        var action = Substitute.For<Func<Error, Task>>();
        var source = Result.Success(2);

        var result = await Task.FromResult(source).TapErrorAsync(action);

        result.ShouldBe(source);
        await action.DidNotReceiveWithAnyArgs().Invoke(default!);
    }

    [Fact]
    public async Task TapErrorAsync_ResultWithAsyncAction_Failure_CallsActionAndReturnsSourceResult()
    {
        var action = Substitute.For<Func<Error, Task>>();
        var source = Result.Failure<Int32>(SourceError);

        var result = await source.TapErrorAsync(action);

        result.ShouldBe(source);
        result.Error.ShouldBe(SourceError);
        await action.Received(1).Invoke(SourceError);
    }

    [Fact]
    public async Task TapErrorAsync_ResultWithAsyncAction_Success_ReturnsSourceResultWithoutCallingAction()
    {
        var action = Substitute.For<Func<Error, Task>>();
        var source = Result.Success(2);

        var result = await source.TapErrorAsync(action);

        result.ShouldBe(source);
        await action.DidNotReceiveWithAnyArgs().Invoke(default!);
    }
}
