namespace Tsikhanau.Railway.Tests;

public class OptionalWhereTests
{
    [Fact]
    public void Where_Some_PredicateTrue_ReturnsSameOptional()
    {
        var predicate = Substitute.For<Func<Int32, Boolean>>();
        predicate(2).Returns(true);
        var optional = Optional<Int32>.Some(2);

        var result = optional.Where(predicate);

        result.ShouldBe(optional);
        predicate.Received(1).Invoke(2);
    }

    [Fact]
    public void Where_Some_PredicateFalse_ReturnsNone()
    {
        var result = Optional<Int32>.Some(2).Where(_ => false);

        result.IsNone.ShouldBeTrue();
    }

    [Fact]
    public void Where_None_ReturnsNoneWithoutCallingPredicate()
    {
        var predicate = Substitute.For<Func<Int32, Boolean>>();

        var result = Optional<Int32>.None().Where(predicate);

        result.IsNone.ShouldBeTrue();
        predicate.DidNotReceiveWithAnyArgs().Invoke(default);
    }

    [Fact]
    public async Task WhereAsync_TaskWithSyncPredicate_Some_PredicateTrue_ReturnsSameOptional()
    {
        var predicate = Substitute.For<Func<Int32, Boolean>>();
        predicate(2).Returns(true);
        var optional = Optional<Int32>.Some(2);

        var result = await Task.FromResult(optional).WhereAsync(predicate);

        result.ShouldBe(optional);
        predicate.Received(1).Invoke(2);
    }

    [Fact]
    public async Task WhereAsync_TaskWithSyncPredicate_Some_PredicateFalse_ReturnsNone()
    {
        var result = await Task.FromResult(Optional<Int32>.Some(2)).WhereAsync(_ => false);

        result.IsNone.ShouldBeTrue();
    }

    [Fact]
    public async Task WhereAsync_TaskWithSyncPredicate_None_ReturnsNoneWithoutCallingPredicate()
    {
        var predicate = Substitute.For<Func<Int32, Boolean>>();

        var result = await Task.FromResult(Optional<Int32>.None()).WhereAsync(predicate);

        result.IsNone.ShouldBeTrue();
        predicate.DidNotReceiveWithAnyArgs().Invoke(default);
    }

    [Fact]
    public async Task WhereAsync_TaskWithAsyncPredicate_Some_PredicateTrue_ReturnsSameOptional()
    {
        var predicate = Substitute.For<Func<Int32, Task<Boolean>>>();
        predicate(2).Returns(Task.FromResult(true));
        var optional = Optional<Int32>.Some(2);

        var result = await Task.FromResult(optional).WhereAsync(predicate);

        result.ShouldBe(optional);
        await predicate.Received(1).Invoke(2);
    }

    [Fact]
    public async Task WhereAsync_TaskWithAsyncPredicate_Some_PredicateFalse_ReturnsNone()
    {
        var result = await Task.FromResult(Optional<Int32>.Some(2)).WhereAsync(_ => Task.FromResult(false));

        result.IsNone.ShouldBeTrue();
    }

    [Fact]
    public async Task WhereAsync_TaskWithAsyncPredicate_None_ReturnsNoneWithoutCallingPredicate()
    {
        var predicate = Substitute.For<Func<Int32, Task<Boolean>>>();

        var result = await Task.FromResult(Optional<Int32>.None()).WhereAsync(predicate);

        result.IsNone.ShouldBeTrue();
        await predicate.DidNotReceiveWithAnyArgs().Invoke(default);
    }

    [Fact]
    public async Task WhereAsync_OptionalWithAsyncPredicate_Some_PredicateTrue_ReturnsSameOptional()
    {
        var predicate = Substitute.For<Func<Int32, Task<Boolean>>>();
        predicate(2).Returns(Task.FromResult(true));
        var optional = Optional<Int32>.Some(2);

        var result = await optional.WhereAsync(predicate);

        result.ShouldBe(optional);
        await predicate.Received(1).Invoke(2);
    }

    [Fact]
    public async Task WhereAsync_OptionalWithAsyncPredicate_Some_PredicateFalse_ReturnsNone()
    {
        var result = await Optional<Int32>.Some(2).WhereAsync(_ => Task.FromResult(false));

        result.IsNone.ShouldBeTrue();
    }

    [Fact]
    public async Task WhereAsync_OptionalWithAsyncPredicate_None_ReturnsNoneWithoutCallingPredicate()
    {
        var predicate = Substitute.For<Func<Int32, Task<Boolean>>>();

        var result = await Optional<Int32>.None().WhereAsync(predicate);

        result.IsNone.ShouldBeTrue();
        await predicate.DidNotReceiveWithAnyArgs().Invoke(default);
    }
}
