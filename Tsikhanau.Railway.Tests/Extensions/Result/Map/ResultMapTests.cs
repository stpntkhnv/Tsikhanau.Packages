namespace Tsikhanau.Railway.Tests;

public class ResultMapTests
{
    private static readonly Error SourceError = Error.Failure("source.failed", "Source failed");

    [Fact]
    public void Map_Success_ReturnsMappedValue()
    {
        var mapper = Substitute.For<Func<Int32, String>>();
        mapper(2).Returns("mapped");

        var result = Result.Success(2).Map(mapper);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe("mapped");
        mapper.Received(1).Invoke(2);
    }

    [Fact]
    public void Map_Failure_ReturnsSourceErrorWithoutCallingMapper()
    {
        var mapper = Substitute.For<Func<Int32, String>>();

        var result = Result.Failure<Int32>(SourceError).Map(mapper);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(SourceError);
        mapper.DidNotReceiveWithAnyArgs().Invoke(default);
    }

    [Fact]
    public void Map_Success_MapperReturnsNull_ThrowsArgumentNullException()
    {
        Func<Int32, String> mapper = _ => null!;

        var exception = Should.Throw<ArgumentNullException>(() => Result.Success(2).Map(mapper));

        exception.ParamName.ShouldBe("value");
    }

    [Fact]
    public async Task MapAsync_TaskWithSyncMapper_Success_ReturnsMappedValue()
    {
        var mapper = Substitute.For<Func<Int32, String>>();
        mapper(2).Returns("mapped");

        var result = await Task.FromResult(Result.Success(2)).MapAsync(mapper);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe("mapped");
        mapper.Received(1).Invoke(2);
    }

    [Fact]
    public async Task MapAsync_TaskWithSyncMapper_Failure_ReturnsSourceErrorWithoutCallingMapper()
    {
        var mapper = Substitute.For<Func<Int32, String>>();

        var result = await Task.FromResult(Result.Failure<Int32>(SourceError)).MapAsync(mapper);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(SourceError);
        mapper.DidNotReceiveWithAnyArgs().Invoke(default);
    }

    [Fact]
    public async Task MapAsync_TaskWithSyncMapper_MapperReturnsNull_ThrowsArgumentNullException()
    {
        Func<Int32, String> mapper = _ => null!;

        var exception = await Should.ThrowAsync<ArgumentNullException>(
            () => Task.FromResult(Result.Success(2)).MapAsync(mapper));

        exception.ParamName.ShouldBe("value");
    }

    [Fact]
    public async Task MapAsync_TaskWithAsyncMapper_Success_ReturnsMappedValue()
    {
        var mapper = Substitute.For<Func<Int32, Task<String>>>();
        mapper(2).Returns(Task.FromResult("mapped"));

        var result = await Task.FromResult(Result.Success(2)).MapAsync(mapper);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe("mapped");
        await mapper.Received(1).Invoke(2);
    }

    [Fact]
    public async Task MapAsync_TaskWithAsyncMapper_Failure_ReturnsSourceErrorWithoutCallingMapper()
    {
        var mapper = Substitute.For<Func<Int32, Task<String>>>();

        var result = await Task.FromResult(Result.Failure<Int32>(SourceError)).MapAsync(mapper);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(SourceError);
        await mapper.DidNotReceiveWithAnyArgs().Invoke(default);
    }

    [Fact]
    public async Task MapAsync_TaskWithAsyncMapper_MapperReturnsNull_ThrowsArgumentNullException()
    {
        Func<Int32, Task<String>> mapper = _ => Task.FromResult<String>(null!);

        var exception = await Should.ThrowAsync<ArgumentNullException>(
            () => Task.FromResult(Result.Success(2)).MapAsync(mapper));

        exception.ParamName.ShouldBe("value");
    }

    [Fact]
    public async Task MapAsync_ResultWithAsyncMapper_Success_ReturnsMappedValue()
    {
        var mapper = Substitute.For<Func<Int32, Task<String>>>();
        mapper(2).Returns(Task.FromResult("mapped"));

        var result = await Result.Success(2).MapAsync(mapper);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe("mapped");
        await mapper.Received(1).Invoke(2);
    }

    [Fact]
    public async Task MapAsync_ResultWithAsyncMapper_Failure_ReturnsSourceErrorWithoutCallingMapper()
    {
        var mapper = Substitute.For<Func<Int32, Task<String>>>();

        var result = await Result.Failure<Int32>(SourceError).MapAsync(mapper);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(SourceError);
        await mapper.DidNotReceiveWithAnyArgs().Invoke(default);
    }

    [Fact]
    public async Task MapAsync_ResultWithAsyncMapper_MapperReturnsNull_ThrowsArgumentNullException()
    {
        Func<Int32, Task<String>> mapper = _ => Task.FromResult<String>(null!);

        var exception = await Should.ThrowAsync<ArgumentNullException>(
            () => Result.Success(2).MapAsync(mapper));

        exception.ParamName.ShouldBe("value");
    }
}
