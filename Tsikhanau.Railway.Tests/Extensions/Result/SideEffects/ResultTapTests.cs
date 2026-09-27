namespace Tsikhanau.Railway.Tests;

public class ResultTapTests
{
    private static readonly Error SourceError = Error.Create("SOURCE", "source failed");

    [Fact]
    public void Tap_Success_CallsActionAndReturnsSourceResult()
    {
        var action = Substitute.For<Action<Int32>>();
        var source = Result.Success(2);

        var result = source.Tap(action);

        result.ShouldBe(source);
        action.Received(1).Invoke(2);
    }

    [Fact]
    public void Tap_Failure_ReturnsSourceResultWithoutCallingAction()
    {
        var action = Substitute.For<Action<Int32>>();

        var result = Result.Failure<Int32>(SourceError).Tap(action);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBeSameAs(SourceError);
        action.DidNotReceiveWithAnyArgs().Invoke(default);
    }

    [Fact]
    public async Task TapAsync_TaskWithSyncAction_Success_CallsActionAndReturnsSourceResult()
    {
        var action = Substitute.For<Action<Int32>>();
        var source = Result.Success(2);

        var result = await Task.FromResult(source).TapAsync(action);

        result.ShouldBe(source);
        action.Received(1).Invoke(2);
    }

    [Fact]
    public async Task TapAsync_TaskWithSyncAction_Failure_ReturnsSourceResultWithoutCallingAction()
    {
        var action = Substitute.For<Action<Int32>>();

        var result = await Task.FromResult(Result.Failure<Int32>(SourceError)).TapAsync(action);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBeSameAs(SourceError);
        action.DidNotReceiveWithAnyArgs().Invoke(default);
    }

    [Fact]
    public async Task TapAsync_TaskWithAsyncAction_Success_CallsActionAndReturnsSourceResult()
    {
        var action = Substitute.For<Func<Int32, Task>>();
        var source = Result.Success(2);

        var result = await Task.FromResult(source).TapAsync(action);

        result.ShouldBe(source);
        await action.Received(1).Invoke(2);
    }

    [Fact]
    public async Task TapAsync_TaskWithAsyncAction_Failure_ReturnsSourceResultWithoutCallingAction()
    {
        var action = Substitute.For<Func<Int32, Task>>();

        var result = await Task.FromResult(Result.Failure<Int32>(SourceError)).TapAsync(action);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBeSameAs(SourceError);
        await action.DidNotReceiveWithAnyArgs().Invoke(default);
    }

    [Fact]
    public async Task TapAsync_ResultWithAsyncAction_Success_CallsActionAndReturnsSourceResult()
    {
        var action = Substitute.For<Func<Int32, Task>>();
        var source = Result.Success(2);

        var result = await source.TapAsync(action);

        result.ShouldBe(source);
        await action.Received(1).Invoke(2);
    }

    [Fact]
    public async Task TapAsync_ResultWithAsyncAction_Failure_ReturnsSourceResultWithoutCallingAction()
    {
        var action = Substitute.For<Func<Int32, Task>>();

        var result = await Result.Failure<Int32>(SourceError).TapAsync(action);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBeSameAs(SourceError);
        await action.DidNotReceiveWithAnyArgs().Invoke(default);
    }
}
