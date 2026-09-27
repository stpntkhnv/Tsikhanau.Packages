namespace Tsikhanau.Railway.Tests;

public class OptionalMapTests
{
    [Fact]
    public void Map_Some_ReturnsSomeWithMappedValue()
    {
        var mapper = Substitute.For<Func<Int32, String>>();
        mapper(2).Returns("2");

        var result = Optional<Int32>.Some(2).Map(mapper);

        result.HasValue.ShouldBeTrue();
        result.Value.ShouldBe("2");
        mapper.Received(1).Invoke(2);
    }

    [Fact]
    public void Map_Some_MapperReturnsNull_ThrowsArgumentNullException()
    {
        var optional = Optional<Int32>.Some(2);

        Should.Throw<ArgumentNullException>(() => optional.Map(_ => (String?)null));
    }

    [Fact]
    public void Map_None_ReturnsNoneWithoutCallingMapper()
    {
        var mapper = Substitute.For<Func<Int32, String>>();

        var result = Optional<Int32>.None().Map(mapper);

        result.IsNone.ShouldBeTrue();
        mapper.DidNotReceiveWithAnyArgs().Invoke(default);
    }

    [Fact]
    public async Task MapAsync_TaskWithSyncMapper_Some_ReturnsSomeWithMappedValue()
    {
        var mapper = Substitute.For<Func<Int32, Int32>>();
        mapper(2).Returns(20);

        var result = await Task.FromResult(Optional<Int32>.Some(2)).MapAsync(mapper);

        result.Value.ShouldBe(20);
        mapper.Received(1).Invoke(2);
    }

    [Fact]
    public async Task MapAsync_TaskWithSyncMapper_MapperReturnsNull_ThrowsArgumentNullException()
    {
        var optionalTask = Task.FromResult(Optional<Int32>.Some(2));

        await Should.ThrowAsync<ArgumentNullException>(() => optionalTask.MapAsync(_ => (String?)null));
    }

    [Fact]
    public async Task MapAsync_TaskWithSyncMapper_None_ReturnsNoneWithoutCallingMapper()
    {
        var mapper = Substitute.For<Func<Int32, Int32>>();

        var result = await Task.FromResult(Optional<Int32>.None()).MapAsync(mapper);

        result.IsNone.ShouldBeTrue();
        mapper.DidNotReceiveWithAnyArgs().Invoke(default);
    }

    [Fact]
    public async Task MapAsync_TaskWithAsyncMapper_Some_ReturnsSomeWithMappedValue()
    {
        var mapper = Substitute.For<Func<Int32, Task<Int32>>>();
        mapper(2).Returns(Task.FromResult(20));

        var result = await Task.FromResult(Optional<Int32>.Some(2)).MapAsync(mapper);

        result.Value.ShouldBe(20);
        await mapper.Received(1).Invoke(2);
    }

    [Fact]
    public async Task MapAsync_TaskWithAsyncMapper_MapperReturnsNull_ThrowsArgumentNullException()
    {
        var optionalTask = Task.FromResult(Optional<Int32>.Some(2));

        await Should.ThrowAsync<ArgumentNullException>(() => optionalTask.MapAsync(_ => Task.FromResult<String?>(null)));
    }

    [Fact]
    public async Task MapAsync_TaskWithAsyncMapper_None_ReturnsNoneWithoutCallingMapper()
    {
        var mapper = Substitute.For<Func<Int32, Task<Int32>>>();

        var result = await Task.FromResult(Optional<Int32>.None()).MapAsync(mapper);

        result.IsNone.ShouldBeTrue();
        await mapper.DidNotReceiveWithAnyArgs().Invoke(default);
    }

    [Fact]
    public async Task MapAsync_OptionalWithAsyncMapper_Some_ReturnsSomeWithMappedValue()
    {
        var mapper = Substitute.For<Func<Int32, Task<Int32>>>();
        mapper(2).Returns(Task.FromResult(20));

        var result = await Optional<Int32>.Some(2).MapAsync(mapper);

        result.Value.ShouldBe(20);
        await mapper.Received(1).Invoke(2);
    }

    [Fact]
    public async Task MapAsync_OptionalWithAsyncMapper_MapperReturnsNull_ThrowsArgumentNullException()
    {
        var optional = Optional<Int32>.Some(2);

        await Should.ThrowAsync<ArgumentNullException>(() => optional.MapAsync(_ => Task.FromResult<String?>(null)));
    }

    [Fact]
    public async Task MapAsync_OptionalWithAsyncMapper_None_ReturnsNoneWithoutCallingMapper()
    {
        var mapper = Substitute.For<Func<Int32, Task<Int32>>>();

        var result = await Optional<Int32>.None().MapAsync(mapper);

        result.IsNone.ShouldBeTrue();
        await mapper.DidNotReceiveWithAnyArgs().Invoke(default);
    }
}
