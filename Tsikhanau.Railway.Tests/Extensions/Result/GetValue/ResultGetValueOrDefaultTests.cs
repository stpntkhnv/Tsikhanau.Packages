namespace Tsikhanau.Railway.Tests;

public class ResultGetValueOrDefaultTests
{
    private static readonly Error SourceError = Error.Failure("source.failed", "Source failed");

    [Fact]
    public void GetValueOrDefault_Success_ReturnsValue()
    {
        var result = Result.Success(2).GetValueOrDefault();

        result.ShouldBe(2);
    }

    [Fact]
    public void GetValueOrDefault_ValueTypeFailure_ReturnsTypeDefault()
    {
        var result = Result.Failure<Int32>(SourceError).GetValueOrDefault();

        result.ShouldBe(0);
    }

    [Fact]
    public void GetValueOrDefault_ReferenceTypeFailure_ReturnsNull()
    {
        var result = Result.Failure<String>(SourceError).GetValueOrDefault();

        result.ShouldBeNull();
    }

    [Fact]
    public void GetValueOrDefault_WithDefaultValue_Success_ReturnsValue()
    {
        var result = Result.Success(2).GetValueOrDefault(7);

        result.ShouldBe(2);
    }

    [Fact]
    public void GetValueOrDefault_WithDefaultValue_Failure_ReturnsDefaultValue()
    {
        var result = Result.Failure<Int32>(SourceError).GetValueOrDefault(7);

        result.ShouldBe(7);
    }

    [Fact]
    public void GetValueOrDefault_WithFactory_Success_ReturnsValueWithoutCallingFactory()
    {
        var factory = Substitute.For<Func<Int32>>();

        var result = Result.Success(2).GetValueOrDefault(factory);

        result.ShouldBe(2);
        factory.DidNotReceive().Invoke();
    }

    [Fact]
    public void GetValueOrDefault_WithFactory_Failure_ReturnsFactoryValue()
    {
        var result = Result.Failure<Int32>(SourceError).GetValueOrDefault(() => 7);

        result.ShouldBe(7);
    }

    [Fact]
    public async Task GetValueOrDefaultAsync_Task_Success_ReturnsValue()
    {
        var result = await Task.FromResult(Result.Success(2)).GetValueOrDefaultAsync();

        result.ShouldBe(2);
    }

    [Fact]
    public async Task GetValueOrDefaultAsync_Task_ValueTypeFailure_ReturnsTypeDefault()
    {
        var result = await Task.FromResult(Result.Failure<Int32>(SourceError)).GetValueOrDefaultAsync();

        result.ShouldBe(0);
    }

    [Fact]
    public async Task GetValueOrDefaultAsync_Task_ReferenceTypeFailure_ReturnsNull()
    {
        var result = await Task.FromResult(Result.Failure<String>(SourceError)).GetValueOrDefaultAsync();

        result.ShouldBeNull();
    }

    [Fact]
    public async Task GetValueOrDefaultAsync_TaskWithDefaultValue_Success_ReturnsValue()
    {
        var result = await Task.FromResult(Result.Success(2)).GetValueOrDefaultAsync(7);

        result.ShouldBe(2);
    }

    [Fact]
    public async Task GetValueOrDefaultAsync_TaskWithDefaultValue_Failure_ReturnsDefaultValue()
    {
        var result = await Task.FromResult(Result.Failure<Int32>(SourceError)).GetValueOrDefaultAsync(7);

        result.ShouldBe(7);
    }

    [Fact]
    public async Task GetValueOrDefaultAsync_TaskWithSyncFactory_Success_ReturnsValueWithoutCallingFactory()
    {
        var factory = Substitute.For<Func<Int32>>();

        var result = await Task.FromResult(Result.Success(2)).GetValueOrDefaultAsync(factory);

        result.ShouldBe(2);
        factory.DidNotReceive().Invoke();
    }

    [Fact]
    public async Task GetValueOrDefaultAsync_TaskWithSyncFactory_Failure_ReturnsFactoryValue()
    {
        var result = await Task.FromResult(Result.Failure<Int32>(SourceError)).GetValueOrDefaultAsync(() => 7);

        result.ShouldBe(7);
    }

    [Fact]
    public async Task GetValueOrDefaultAsync_TaskWithAsyncFactory_Success_ReturnsValueWithoutCallingFactory()
    {
        var factory = Substitute.For<Func<Task<Int32>>>();

        var result = await Task.FromResult(Result.Success(2)).GetValueOrDefaultAsync(factory);

        result.ShouldBe(2);
        await factory.DidNotReceive().Invoke();
    }

    [Fact]
    public async Task GetValueOrDefaultAsync_TaskWithAsyncFactory_Failure_ReturnsFactoryValue()
    {
        var result = await Task.FromResult(Result.Failure<Int32>(SourceError))
            .GetValueOrDefaultAsync(() => Task.FromResult(7));

        result.ShouldBe(7);
    }
}
