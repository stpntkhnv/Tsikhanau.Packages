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
    public void Map_Some_MapperReturnsNull_ReturnsNone()
    {
        Func<Int32, String> mapper = _ => null!;

        var result = Optional<Int32>.Some(2).Map(mapper);

        result.IsNone.ShouldBeTrue();
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
    public async Task MapAsync_TaskWithSyncMapper_MapperReturnsNull_ReturnsNone()
    {
        Func<Int32, String> mapper = _ => null!;

        var result = await Task.FromResult(Optional<Int32>.Some(2)).MapAsync(mapper);

        result.IsNone.ShouldBeTrue();
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
    public async Task MapAsync_TaskWithAsyncMapper_MapperReturnsNull_ReturnsNone()
    {
        Func<Int32, Task<String>> mapper = _ => Task.FromResult<String>(null!);

        var result = await Task.FromResult(Optional<Int32>.Some(2)).MapAsync(mapper);

        result.IsNone.ShouldBeTrue();
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
    public async Task MapAsync_OptionalWithAsyncMapper_MapperReturnsNull_ReturnsNone()
    {
        Func<Int32, Task<String>> mapper = _ => Task.FromResult<String>(null!);

        var result = await Optional<Int32>.Some(2).MapAsync(mapper);

        result.IsNone.ShouldBeTrue();
    }

    [Fact]
    public async Task MapAsync_OptionalWithAsyncMapper_None_ReturnsNoneWithoutCallingMapper()
    {
        var mapper = Substitute.For<Func<Int32, Task<Int32>>>();

        var result = await Optional<Int32>.None().MapAsync(mapper);

        result.IsNone.ShouldBeTrue();
        await mapper.DidNotReceiveWithAnyArgs().Invoke(default);
    }

    [Fact]
    public void Map_Some_NullableMapperReturnsNull_ReturnsNone()
    {
        var names = new Dictionary<Int32, String> { [1] = "one" };

        var result = Optional<Int32>.Some(2).Map(x => names.GetValueOrDefault(x));

        result.IsNone.ShouldBeTrue();
    }

    [Fact]
    public void Map_Some_NullableMapperReturnsValue_ReturnsSome()
    {
        var names = new Dictionary<Int32, String> { [1] = "one" };

        var result = Optional<Int32>.Some(1).Map(x => names.GetValueOrDefault(x));

        result.Value.ShouldBe("one");
    }
}
