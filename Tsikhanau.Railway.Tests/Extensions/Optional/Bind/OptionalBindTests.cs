namespace Tsikhanau.Railway.Tests;

public class OptionalBindTests
{
    [Fact]
    public void Bind_Some_ReturnsBinderResult()
    {
        var binder = Substitute.For<Func<Int32, Optional<String>>>();
        binder(2).Returns(Optional<String>.Some("2"));

        var result = Optional<Int32>.Some(2).Bind(binder);

        result.HasValue.ShouldBeTrue();
        result.Value.ShouldBe("2");
        binder.Received(1).Invoke(2);
    }

    [Fact]
    public void Bind_Some_BinderReturnsNone_ReturnsNone()
    {
        var result = Optional<Int32>.Some(2).Bind(_ => Optional<String>.None());

        result.IsNone.ShouldBeTrue();
    }

    [Fact]
    public void Bind_None_ReturnsNoneWithoutCallingBinder()
    {
        var binder = Substitute.For<Func<Int32, Optional<String>>>();

        var result = Optional<Int32>.None().Bind(binder);

        result.IsNone.ShouldBeTrue();
        binder.DidNotReceiveWithAnyArgs().Invoke(default);
    }

    [Fact]
    public async Task BindAsync_TaskWithSyncBinder_Some_ReturnsBinderResult()
    {
        var binder = Substitute.For<Func<Int32, Optional<Int32>>>();
        binder(2).Returns(Optional<Int32>.Some(20));

        var result = await Task.FromResult(Optional<Int32>.Some(2)).BindAsync(binder);

        result.Value.ShouldBe(20);
        binder.Received(1).Invoke(2);
    }

    [Fact]
    public async Task BindAsync_TaskWithSyncBinder_BinderReturnsNone_ReturnsNone()
    {
        var result = await Task.FromResult(Optional<Int32>.Some(2)).BindAsync(_ => Optional<Int32>.None());

        result.IsNone.ShouldBeTrue();
    }

    [Fact]
    public async Task BindAsync_TaskWithSyncBinder_None_ReturnsNoneWithoutCallingBinder()
    {
        var binder = Substitute.For<Func<Int32, Optional<Int32>>>();

        var result = await Task.FromResult(Optional<Int32>.None()).BindAsync(binder);

        result.IsNone.ShouldBeTrue();
        binder.DidNotReceiveWithAnyArgs().Invoke(default);
    }

    [Fact]
    public async Task BindAsync_TaskWithAsyncBinder_Some_ReturnsBinderResult()
    {
        var binder = Substitute.For<Func<Int32, Task<Optional<Int32>>>>();
        binder(2).Returns(Task.FromResult(Optional<Int32>.Some(20)));

        var result = await Task.FromResult(Optional<Int32>.Some(2)).BindAsync(binder);

        result.Value.ShouldBe(20);
        await binder.Received(1).Invoke(2);
    }

    [Fact]
    public async Task BindAsync_TaskWithAsyncBinder_BinderReturnsNone_ReturnsNone()
    {
        var result = await Task.FromResult(Optional<Int32>.Some(2))
            .BindAsync(_ => Task.FromResult(Optional<Int32>.None()));

        result.IsNone.ShouldBeTrue();
    }

    [Fact]
    public async Task BindAsync_TaskWithAsyncBinder_None_ReturnsNoneWithoutCallingBinder()
    {
        var binder = Substitute.For<Func<Int32, Task<Optional<Int32>>>>();

        var result = await Task.FromResult(Optional<Int32>.None()).BindAsync(binder);

        result.IsNone.ShouldBeTrue();
        await binder.DidNotReceiveWithAnyArgs().Invoke(default);
    }

    [Fact]
    public async Task BindAsync_OptionalWithAsyncBinder_Some_ReturnsBinderResult()
    {
        var binder = Substitute.For<Func<Int32, Task<Optional<Int32>>>>();
        binder(2).Returns(Task.FromResult(Optional<Int32>.Some(20)));

        var result = await Optional<Int32>.Some(2).BindAsync(binder);

        result.Value.ShouldBe(20);
        await binder.Received(1).Invoke(2);
    }

    [Fact]
    public async Task BindAsync_OptionalWithAsyncBinder_BinderReturnsNone_ReturnsNone()
    {
        var result = await Optional<Int32>.Some(2).BindAsync(_ => Task.FromResult(Optional<Int32>.None()));

        result.IsNone.ShouldBeTrue();
    }

    [Fact]
    public async Task BindAsync_OptionalWithAsyncBinder_None_ReturnsNoneWithoutCallingBinder()
    {
        var binder = Substitute.For<Func<Int32, Task<Optional<Int32>>>>();

        var result = await Optional<Int32>.None().BindAsync(binder);

        result.IsNone.ShouldBeTrue();
        await binder.DidNotReceiveWithAnyArgs().Invoke(default);
    }
}
