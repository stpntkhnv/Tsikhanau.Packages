namespace Tsikhanau.Railway.Tests;

public class ResultMapErrorTests
{
    private static readonly Error SourceError = Error.Create("SOURCE", "source failed");

    [Fact]
    public void MapError_Failure_ReturnsMappedError()
    {
        var errorMapper = Substitute.For<Func<Error, String>>();
        errorMapper(SourceError).Returns("mapped");

        var result = Result.Failure<Int32>(SourceError).MapError(errorMapper);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe("mapped");
        errorMapper.Received(1).Invoke(SourceError);
    }

    [Fact]
    public void MapError_Success_ReturnsSourceValueWithoutCallingMapper()
    {
        var errorMapper = Substitute.For<Func<Error, String>>();

        var result = Result.Success(2).MapError(errorMapper);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(2);
        errorMapper.DidNotReceiveWithAnyArgs().Invoke(default!);
    }

    [Fact]
    public void MapError_Failure_MapperReturnsNull_ThrowsArgumentNullException()
    {
        Func<Error, String> errorMapper = _ => null!;

        var exception = Should.Throw<ArgumentNullException>(
            () => Result.Failure<Int32>(SourceError).MapError(errorMapper));

        exception.ParamName.ShouldBe("error");
    }

    [Fact]
    public async Task MapErrorAsync_TaskWithSyncMapper_Failure_ReturnsMappedError()
    {
        var errorMapper = Substitute.For<Func<Error, String>>();
        errorMapper(SourceError).Returns("mapped");

        var result = await Task.FromResult(Result.Failure<Int32>(SourceError)).MapErrorAsync(errorMapper);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe("mapped");
        errorMapper.Received(1).Invoke(SourceError);
    }

    [Fact]
    public async Task MapErrorAsync_TaskWithSyncMapper_Success_ReturnsSourceValueWithoutCallingMapper()
    {
        var errorMapper = Substitute.For<Func<Error, String>>();

        var result = await Task.FromResult(Result.Success(2)).MapErrorAsync(errorMapper);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(2);
        errorMapper.DidNotReceiveWithAnyArgs().Invoke(default!);
    }

    [Fact]
    public async Task MapErrorAsync_TaskWithSyncMapper_MapperReturnsNull_ThrowsArgumentNullException()
    {
        Func<Error, String> errorMapper = _ => null!;

        var exception = await Should.ThrowAsync<ArgumentNullException>(
            () => Task.FromResult(Result.Failure<Int32>(SourceError)).MapErrorAsync(errorMapper));

        exception.ParamName.ShouldBe("error");
    }

    [Fact]
    public async Task MapErrorAsync_TaskWithAsyncMapper_Failure_ReturnsMappedError()
    {
        var errorMapper = Substitute.For<Func<Error, Task<String>>>();
        errorMapper(SourceError).Returns(Task.FromResult("mapped"));

        var result = await Task.FromResult(Result.Failure<Int32>(SourceError)).MapErrorAsync(errorMapper);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe("mapped");
        await errorMapper.Received(1).Invoke(SourceError);
    }

    [Fact]
    public async Task MapErrorAsync_TaskWithAsyncMapper_Success_ReturnsSourceValueWithoutCallingMapper()
    {
        var errorMapper = Substitute.For<Func<Error, Task<String>>>();

        var result = await Task.FromResult(Result.Success(2)).MapErrorAsync(errorMapper);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(2);
        await errorMapper.DidNotReceiveWithAnyArgs().Invoke(default!);
    }

    [Fact]
    public async Task MapErrorAsync_TaskWithAsyncMapper_MapperReturnsNull_ThrowsArgumentNullException()
    {
        Func<Error, Task<String>> errorMapper = _ => Task.FromResult<String>(null!);

        var exception = await Should.ThrowAsync<ArgumentNullException>(
            () => Task.FromResult(Result.Failure<Int32>(SourceError)).MapErrorAsync(errorMapper));

        exception.ParamName.ShouldBe("error");
    }

    [Fact]
    public async Task MapErrorAsync_ResultWithAsyncMapper_Failure_ReturnsMappedError()
    {
        var errorMapper = Substitute.For<Func<Error, Task<String>>>();
        errorMapper(SourceError).Returns(Task.FromResult("mapped"));

        var result = await Result.Failure<Int32>(SourceError).MapErrorAsync(errorMapper);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe("mapped");
        await errorMapper.Received(1).Invoke(SourceError);
    }

    [Fact]
    public async Task MapErrorAsync_ResultWithAsyncMapper_Success_ReturnsSourceValueWithoutCallingMapper()
    {
        var errorMapper = Substitute.For<Func<Error, Task<String>>>();

        var result = await Result.Success(2).MapErrorAsync(errorMapper);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(2);
        await errorMapper.DidNotReceiveWithAnyArgs().Invoke(default!);
    }

    [Fact]
    public async Task MapErrorAsync_ResultWithAsyncMapper_MapperReturnsNull_ThrowsArgumentNullException()
    {
        Func<Error, Task<String>> errorMapper = _ => Task.FromResult<String>(null!);

        var exception = await Should.ThrowAsync<ArgumentNullException>(
            () => Result.Failure<Int32>(SourceError).MapErrorAsync(errorMapper));

        exception.ParamName.ShouldBe("error");
    }
}
