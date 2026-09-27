namespace Tsikhanau.Railway.Tests;

public class ResultOnSuccessTests
{
    private static readonly Error SourceError = Error.Create("SOURCE", "source failed");

    [Fact]
    public void OnSuccess_Success_CallsActionAndReturnsSourceResult()
    {
        var action = Substitute.For<Action<Int32>>();
        var source = Result.Success(2);

        var result = source.OnSuccess(action);

        result.ShouldBe(source);
        action.Received(1).Invoke(2);
    }

    [Fact]
    public void OnSuccess_Failure_ReturnsSourceResultWithoutCallingAction()
    {
        var action = Substitute.For<Action<Int32>>();

        var result = Result.Failure<Int32>(SourceError).OnSuccess(action);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBeSameAs(SourceError);
        action.DidNotReceiveWithAnyArgs().Invoke(default);
    }

    [Fact]
    public async Task OnSuccessAsync_TaskWithSyncAction_Success_CallsActionAndReturnsSourceResult()
    {
        var action = Substitute.For<Action<Int32>>();
        var source = Result.Success(2);

        var result = await Task.FromResult(source).OnSuccessAsync(action);

        result.ShouldBe(source);
        action.Received(1).Invoke(2);
    }

    [Fact]
    public async Task OnSuccessAsync_TaskWithSyncAction_Failure_ReturnsSourceResultWithoutCallingAction()
    {
        var action = Substitute.For<Action<Int32>>();

        var result = await Task.FromResult(Result.Failure<Int32>(SourceError)).OnSuccessAsync(action);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBeSameAs(SourceError);
        action.DidNotReceiveWithAnyArgs().Invoke(default);
    }

    [Fact]
    public async Task OnSuccessAsync_TaskWithAsyncAction_Success_CallsActionAndReturnsSourceResult()
    {
        var action = Substitute.For<Func<Int32, Task>>();
        var source = Result.Success(2);

        var result = await Task.FromResult(source).OnSuccessAsync(action);

        result.ShouldBe(source);
        await action.Received(1).Invoke(2);
    }

    [Fact]
    public async Task OnSuccessAsync_TaskWithAsyncAction_Failure_ReturnsSourceResultWithoutCallingAction()
    {
        var action = Substitute.For<Func<Int32, Task>>();

        var result = await Task.FromResult(Result.Failure<Int32>(SourceError)).OnSuccessAsync(action);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBeSameAs(SourceError);
        await action.DidNotReceiveWithAnyArgs().Invoke(default);
    }

    [Fact]
    public async Task OnSuccessAsync_ResultWithAsyncAction_Success_CallsActionAndReturnsSourceResult()
    {
        var action = Substitute.For<Func<Int32, Task>>();
        var source = Result.Success(2);

        var result = await source.OnSuccessAsync(action);

        result.ShouldBe(source);
        await action.Received(1).Invoke(2);
    }

    [Fact]
    public async Task OnSuccessAsync_ResultWithAsyncAction_Failure_ReturnsSourceResultWithoutCallingAction()
    {
        var action = Substitute.For<Func<Int32, Task>>();

        var result = await Result.Failure<Int32>(SourceError).OnSuccessAsync(action);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBeSameAs(SourceError);
        await action.DidNotReceiveWithAnyArgs().Invoke(default);
    }
}
