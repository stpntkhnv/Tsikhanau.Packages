namespace Tsikhanau.Railway.Tests;

public class ResultMapErrorTests
{
    private static readonly Error SourceError = Error.Failure("source.failed", "Source failed");
    private static readonly Error MappedError = Error.NotFound("mapped.not_found", "Mapped error");

    [Fact]
    public void MapError_Failure_ReturnsMappedError()
    {
        var errorMapper = Substitute.For<Func<Error, Error>>();
        errorMapper(SourceError).Returns(MappedError);

        var result = Result.Failure<Int32>(SourceError).MapError(errorMapper);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(MappedError);
        errorMapper.Received(1).Invoke(SourceError);
    }

    [Fact]
    public void MapError_Success_ReturnsSourceValueWithoutCallingMapper()
    {
        var errorMapper = Substitute.For<Func<Error, Error>>();

        var result = Result.Success(2).MapError(errorMapper);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(2);
        errorMapper.DidNotReceiveWithAnyArgs().Invoke(default!);
    }

    [Fact]
    public void MapError_Failure_MapperReturnsNull_ThrowsArgumentNullException()
    {
        Func<Error, Error> errorMapper = _ => null!;

        var exception = Should.Throw<ArgumentNullException>(
            () => Result.Failure<Int32>(SourceError).MapError(errorMapper));

        exception.ParamName.ShouldBe("error");
    }

    [Fact]
    public async Task MapErrorAsync_TaskWithSyncMapper_Failure_ReturnsMappedError()
    {
        var errorMapper = Substitute.For<Func<Error, Error>>();
        errorMapper(SourceError).Returns(MappedError);

        var result = await Task.FromResult(Result.Failure<Int32>(SourceError)).MapErrorAsync(errorMapper);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(MappedError);
        errorMapper.Received(1).Invoke(SourceError);
    }

    [Fact]
    public async Task MapErrorAsync_TaskWithSyncMapper_Success_ReturnsSourceValueWithoutCallingMapper()
    {
        var errorMapper = Substitute.For<Func<Error, Error>>();

        var result = await Task.FromResult(Result.Success(2)).MapErrorAsync(errorMapper);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(2);
        errorMapper.DidNotReceiveWithAnyArgs().Invoke(default!);
    }

    [Fact]
    public async Task MapErrorAsync_TaskWithSyncMapper_MapperReturnsNull_ThrowsArgumentNullException()
    {
        Func<Error, Error> errorMapper = _ => null!;

        var exception = await Should.ThrowAsync<ArgumentNullException>(
            () => Task.FromResult(Result.Failure<Int32>(SourceError)).MapErrorAsync(errorMapper));

        exception.ParamName.ShouldBe("error");
    }

    [Fact]
    public async Task MapErrorAsync_TaskWithAsyncMapper_Failure_ReturnsMappedError()
    {
        var errorMapper = Substitute.For<Func<Error, Task<Error>>>();
        errorMapper(SourceError).Returns(Task.FromResult(MappedError));

        var result = await Task.FromResult(Result.Failure<Int32>(SourceError)).MapErrorAsync(errorMapper);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(MappedError);
        await errorMapper.Received(1).Invoke(SourceError);
    }

    [Fact]
    public async Task MapErrorAsync_TaskWithAsyncMapper_Success_ReturnsSourceValueWithoutCallingMapper()
    {
        var errorMapper = Substitute.For<Func<Error, Task<Error>>>();

        var result = await Task.FromResult(Result.Success(2)).MapErrorAsync(errorMapper);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(2);
        await errorMapper.DidNotReceiveWithAnyArgs().Invoke(default!);
    }

    [Fact]
    public async Task MapErrorAsync_TaskWithAsyncMapper_MapperReturnsNull_ThrowsArgumentNullException()
    {
        Func<Error, Task<Error>> errorMapper = _ => Task.FromResult<Error>(null!);

        var exception = await Should.ThrowAsync<ArgumentNullException>(
            () => Task.FromResult(Result.Failure<Int32>(SourceError)).MapErrorAsync(errorMapper));

        exception.ParamName.ShouldBe("error");
    }

    [Fact]
    public async Task MapErrorAsync_ResultWithAsyncMapper_Failure_ReturnsMappedError()
    {
        var errorMapper = Substitute.For<Func<Error, Task<Error>>>();
        errorMapper(SourceError).Returns(Task.FromResult(MappedError));

        var result = await Result.Failure<Int32>(SourceError).MapErrorAsync(errorMapper);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(MappedError);
        await errorMapper.Received(1).Invoke(SourceError);
    }

    [Fact]
    public async Task MapErrorAsync_ResultWithAsyncMapper_Success_ReturnsSourceValueWithoutCallingMapper()
    {
        var errorMapper = Substitute.For<Func<Error, Task<Error>>>();

        var result = await Result.Success(2).MapErrorAsync(errorMapper);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(2);
        await errorMapper.DidNotReceiveWithAnyArgs().Invoke(default!);
    }

    [Fact]
    public async Task MapErrorAsync_ResultWithAsyncMapper_MapperReturnsNull_ThrowsArgumentNullException()
    {
        Func<Error, Task<Error>> errorMapper = _ => Task.FromResult<Error>(null!);

        var exception = await Should.ThrowAsync<ArgumentNullException>(
            () => Result.Failure<Int32>(SourceError).MapErrorAsync(errorMapper));

        exception.ParamName.ShouldBe("error");
    }
}
