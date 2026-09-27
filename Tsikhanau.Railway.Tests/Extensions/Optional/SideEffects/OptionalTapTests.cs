namespace Tsikhanau.Railway.Tests;

public class OptionalTapTests
{
    [Fact]
    public void Tap_Some_CallsActionAndReturnsSameOptional()
    {
        var action = Substitute.For<Action<Int32>>();
        var optional = Optional<Int32>.Some(2);

        var result = optional.Tap(action);

        result.ShouldBe(optional);
        action.Received(1).Invoke(2);
    }

    [Fact]
    public void Tap_None_ReturnsNoneWithoutCallingAction()
    {
        var action = Substitute.For<Action<Int32>>();

        var result = Optional<Int32>.None().Tap(action);

        result.IsNone.ShouldBeTrue();
        action.DidNotReceiveWithAnyArgs().Invoke(default);
    }

    [Fact]
    public async Task TapAsync_TaskWithSyncAction_Some_CallsActionAndReturnsSameOptional()
    {
        var action = Substitute.For<Action<Int32>>();
        var optional = Optional<Int32>.Some(2);

        var result = await Task.FromResult(optional).TapAsync(action);

        result.ShouldBe(optional);
        action.Received(1).Invoke(2);
    }

    [Fact]
    public async Task TapAsync_TaskWithSyncAction_None_ReturnsNoneWithoutCallingAction()
    {
        var action = Substitute.For<Action<Int32>>();

        var result = await Task.FromResult(Optional<Int32>.None()).TapAsync(action);

        result.IsNone.ShouldBeTrue();
        action.DidNotReceiveWithAnyArgs().Invoke(default);
    }

    [Fact]
    public async Task TapAsync_TaskWithAsyncAction_Some_CallsActionAndReturnsSameOptional()
    {
        var action = Substitute.For<Func<Int32, Task>>();
        action(2).Returns(Task.CompletedTask);
        var optional = Optional<Int32>.Some(2);

        var result = await Task.FromResult(optional).TapAsync(action);

        result.ShouldBe(optional);
        await action.Received(1).Invoke(2);
    }

    [Fact]
    public async Task TapAsync_TaskWithAsyncAction_None_ReturnsNoneWithoutCallingAction()
    {
        var action = Substitute.For<Func<Int32, Task>>();

        var result = await Task.FromResult(Optional<Int32>.None()).TapAsync(action);

        result.IsNone.ShouldBeTrue();
        await action.DidNotReceiveWithAnyArgs().Invoke(default);
    }

    [Fact]
    public async Task TapAsync_OptionalWithAsyncAction_Some_CallsActionAndReturnsSameOptional()
    {
        var action = Substitute.For<Func<Int32, Task>>();
        action(2).Returns(Task.CompletedTask);
        var optional = Optional<Int32>.Some(2);

        var result = await optional.TapAsync(action);

        result.ShouldBe(optional);
        await action.Received(1).Invoke(2);
    }

    [Fact]
    public async Task TapAsync_OptionalWithAsyncAction_None_ReturnsNoneWithoutCallingAction()
    {
        var action = Substitute.For<Func<Int32, Task>>();

        var result = await Optional<Int32>.None().TapAsync(action);

        result.IsNone.ShouldBeTrue();
        await action.DidNotReceiveWithAnyArgs().Invoke(default);
    }
}
