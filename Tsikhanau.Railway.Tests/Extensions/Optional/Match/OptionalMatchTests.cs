namespace Tsikhanau.Railway.Tests;

public class OptionalMatchTests
{
    [Fact]
    public void Match_Some_ReturnsOnSomeResultWithoutCallingOnNone()
    {
        var onSome = Substitute.For<Func<Int32, String>>();
        onSome(2).Returns("some 2");
        var onNone = Substitute.For<Func<String>>();

        var result = Optional<Int32>.Some(2).Match(onSome, onNone);

        result.ShouldBe("some 2");
        onSome.Received(1).Invoke(2);
        onNone.DidNotReceive().Invoke();
    }

    [Fact]
    public void Match_None_ReturnsOnNoneResultWithoutCallingOnSome()
    {
        var onSome = Substitute.For<Func<Int32, String>>();
        var onNone = Substitute.For<Func<String>>();
        onNone().Returns("none");

        var result = Optional<Int32>.None().Match(onSome, onNone);

        result.ShouldBe("none");
        onNone.Received(1).Invoke();
        onSome.DidNotReceiveWithAnyArgs().Invoke(default);
    }

    [Fact]
    public async Task MatchAsync_TaskWithSyncHandlers_Some_ReturnsOnSomeResultWithoutCallingOnNone()
    {
        var onSome = Substitute.For<Func<Int32, String>>();
        onSome(2).Returns("some 2");
        var onNone = Substitute.For<Func<String>>();

        var result = await Task.FromResult(Optional<Int32>.Some(2)).MatchAsync(onSome, onNone);

        result.ShouldBe("some 2");
        onSome.Received(1).Invoke(2);
        onNone.DidNotReceive().Invoke();
    }

    [Fact]
    public async Task MatchAsync_TaskWithSyncHandlers_None_ReturnsOnNoneResultWithoutCallingOnSome()
    {
        var onSome = Substitute.For<Func<Int32, String>>();
        var onNone = Substitute.For<Func<String>>();
        onNone().Returns("none");

        var result = await Task.FromResult(Optional<Int32>.None()).MatchAsync(onSome, onNone);

        result.ShouldBe("none");
        onNone.Received(1).Invoke();
        onSome.DidNotReceiveWithAnyArgs().Invoke(default);
    }

    [Fact]
    public async Task MatchAsync_TaskWithAsyncHandlers_Some_ReturnsOnSomeResultWithoutCallingOnNone()
    {
        var onSome = Substitute.For<Func<Int32, Task<String>>>();
        onSome(2).Returns(Task.FromResult("some 2"));
        var onNone = Substitute.For<Func<Task<String>>>();

        var result = await Task.FromResult(Optional<Int32>.Some(2)).MatchAsync(onSome, onNone);

        result.ShouldBe("some 2");
        await onSome.Received(1).Invoke(2);
        await onNone.DidNotReceive().Invoke();
    }

    [Fact]
    public async Task MatchAsync_TaskWithAsyncHandlers_None_ReturnsOnNoneResultWithoutCallingOnSome()
    {
        var onSome = Substitute.For<Func<Int32, Task<String>>>();
        var onNone = Substitute.For<Func<Task<String>>>();
        onNone().Returns(Task.FromResult("none"));

        var result = await Task.FromResult(Optional<Int32>.None()).MatchAsync(onSome, onNone);

        result.ShouldBe("none");
        await onNone.Received(1).Invoke();
        await onSome.DidNotReceiveWithAnyArgs().Invoke(default);
    }

    [Fact]
    public async Task MatchAsync_OptionalWithAsyncHandlers_Some_ReturnsOnSomeResultWithoutCallingOnNone()
    {
        var onSome = Substitute.For<Func<Int32, Task<String>>>();
        onSome(2).Returns(Task.FromResult("some 2"));
        var onNone = Substitute.For<Func<Task<String>>>();

        var result = await Optional<Int32>.Some(2).MatchAsync(onSome, onNone);

        result.ShouldBe("some 2");
        await onSome.Received(1).Invoke(2);
        await onNone.DidNotReceive().Invoke();
    }

    [Fact]
    public async Task MatchAsync_OptionalWithAsyncHandlers_None_ReturnsOnNoneResultWithoutCallingOnSome()
    {
        var onSome = Substitute.For<Func<Int32, Task<String>>>();
        var onNone = Substitute.For<Func<Task<String>>>();
        onNone().Returns(Task.FromResult("none"));

        var result = await Optional<Int32>.None().MatchAsync(onSome, onNone);

        result.ShouldBe("none");
        await onNone.Received(1).Invoke();
        await onSome.DidNotReceiveWithAnyArgs().Invoke(default);
    }
}
