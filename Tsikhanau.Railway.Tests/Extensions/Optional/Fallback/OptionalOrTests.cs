namespace Tsikhanau.Railway.Tests;

public class OptionalOrTests
{
    [Fact]
    public void Or_Some_ReturnsSourceIgnoringFallback()
    {
        var result = Optional<Int32>.Some(2).Or(Optional<Int32>.Some(5));

        result.Value.ShouldBe(2);
    }

    [Fact]
    public void Or_None_ReturnsFallback()
    {
        var result = Optional<Int32>.None().Or(Optional<Int32>.Some(5));

        result.Value.ShouldBe(5);
    }

    [Fact]
    public void Or_None_FallbackNone_ReturnsNone()
    {
        var result = Optional<Int32>.None().Or(Optional<Int32>.None());

        result.IsNone.ShouldBeTrue();
    }

    [Fact]
    public void OrElse_Some_ReturnsSourceWithoutCallingFactory()
    {
        var factory = Substitute.For<Func<Optional<Int32>>>();

        var result = Optional<Int32>.Some(2).OrElse(factory);

        result.Value.ShouldBe(2);
        factory.DidNotReceive().Invoke();
    }

    [Fact]
    public void OrElse_None_ReturnsFactoryResult()
    {
        var factory = Substitute.For<Func<Optional<Int32>>>();
        factory().Returns(Optional<Int32>.Some(5));

        var result = Optional<Int32>.None().OrElse(factory);

        result.Value.ShouldBe(5);
        factory.Received(1).Invoke();
    }

    [Fact]
    public async Task OrAsync_TaskWithFallback_Some_ReturnsSourceIgnoringFallback()
    {
        var result = await Task.FromResult(Optional<Int32>.Some(2)).OrAsync(Optional<Int32>.Some(5));

        result.Value.ShouldBe(2);
    }

    [Fact]
    public async Task OrAsync_TaskWithFallback_None_ReturnsFallback()
    {
        var result = await Task.FromResult(Optional<Int32>.None()).OrAsync(Optional<Int32>.Some(5));

        result.Value.ShouldBe(5);
    }

    [Fact]
    public async Task OrElseAsync_TaskWithSyncFactory_Some_ReturnsSourceWithoutCallingFactory()
    {
        var factory = Substitute.For<Func<Optional<Int32>>>();

        var result = await Task.FromResult(Optional<Int32>.Some(2)).OrElseAsync(factory);

        result.Value.ShouldBe(2);
        factory.DidNotReceive().Invoke();
    }

    [Fact]
    public async Task OrElseAsync_TaskWithSyncFactory_None_ReturnsFactoryResult()
    {
        var factory = Substitute.For<Func<Optional<Int32>>>();
        factory().Returns(Optional<Int32>.Some(5));

        var result = await Task.FromResult(Optional<Int32>.None()).OrElseAsync(factory);

        result.Value.ShouldBe(5);
        factory.Received(1).Invoke();
    }

    [Fact]
    public async Task OrElseAsync_TaskWithAsyncFactory_Some_ReturnsSourceWithoutCallingFactory()
    {
        var factory = Substitute.For<Func<Task<Optional<Int32>>>>();

        var result = await Task.FromResult(Optional<Int32>.Some(2)).OrElseAsync(factory);

        result.Value.ShouldBe(2);
        await factory.DidNotReceive().Invoke();
    }

    [Fact]
    public async Task OrElseAsync_TaskWithAsyncFactory_None_ReturnsFactoryResult()
    {
        var factory = Substitute.For<Func<Task<Optional<Int32>>>>();
        factory().Returns(Task.FromResult(Optional<Int32>.Some(5)));

        var result = await Task.FromResult(Optional<Int32>.None()).OrElseAsync(factory);

        result.Value.ShouldBe(5);
        await factory.Received(1).Invoke();
    }

    [Fact]
    public async Task OrElseAsync_OptionalWithAsyncFactory_Some_ReturnsSourceWithoutCallingFactory()
    {
        var factory = Substitute.For<Func<Task<Optional<Int32>>>>();

        var result = await Optional<Int32>.Some(2).OrElseAsync(factory);

        result.Value.ShouldBe(2);
        await factory.DidNotReceive().Invoke();
    }

    [Fact]
    public async Task OrElseAsync_OptionalWithAsyncFactory_None_ReturnsFactoryResult()
    {
        var factory = Substitute.For<Func<Task<Optional<Int32>>>>();
        factory().Returns(Task.FromResult(Optional<Int32>.Some(5)));

        var result = await Optional<Int32>.None().OrElseAsync(factory);

        result.Value.ShouldBe(5);
        await factory.Received(1).Invoke();
    }
}
