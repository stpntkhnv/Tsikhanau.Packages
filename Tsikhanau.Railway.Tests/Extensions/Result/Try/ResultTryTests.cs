namespace Tsikhanau.Railway.Tests;

public class ResultTryTests
{
    private static readonly Error MappedError = Error.Conflict("mapped.failed", "Mapped failure");

    [Fact]
    public void Try_Success_ReturnsValue()
    {
        var result = Result.Try(() => 5);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(5);
    }

    [Fact]
    public void Try_Throws_ReturnsErrorFromException()
    {
        var exception = new InvalidOperationException("boom");

        var result = Result.Try<Int32>(() => throw exception);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(Error.FromException(exception));
        result.Error.Kind.ShouldBe(ErrorKind.Unexpected);
        result.Error.Code.ShouldBe("InvalidOperationException");
        result.Error.Message.ShouldBe("boom");
        result.Error.Inner.ShouldBeNull();
    }

    [Fact]
    public void Try_ThrowsWithInnerException_ReturnsErrorWithInner()
    {
        var result = Result.Try<Int32>(
            () => throw new InvalidOperationException("outer", new FormatException("inner")));

        result.Error.GetAllCodes().ShouldBe(new[] { "InvalidOperationException", "FormatException" });
        result.Error.GetFullMessage().ShouldBe("outer -> inner");
        result.Error.Inner.ShouldNotBeNull().Kind.ShouldBe(ErrorKind.Unexpected);
    }

    [Fact]
    public void Try_FuncReturnsNull_ThrowsArgumentNullException()
    {
        Should.Throw<ArgumentNullException>(() => Result.Try<String>(() => null!));
    }

    [Fact]
    public void Try_NullFunc_ThrowsArgumentNullException()
    {
        var exception = Should.Throw<ArgumentNullException>(() => Result.Try<Int32>(null!));

        exception.ParamName.ShouldBe("func");
    }

    [Fact]
    public void Try_WithErrorMapper_Success_ReturnsValueWithoutCallingMapper()
    {
        var errorMapper = Substitute.For<Func<Exception, Error>>();

        var result = Result.Try(() => 5, errorMapper);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(5);
        errorMapper.DidNotReceiveWithAnyArgs().Invoke(default!);
    }

    [Fact]
    public void Try_WithErrorMapper_Throws_ReturnsMappedError()
    {
        var exception = new InvalidOperationException("boom");
        var errorMapper = Substitute.For<Func<Exception, Error>>();
        errorMapper(exception).Returns(MappedError);

        var result = Result.Try<Int32>(() => throw exception, errorMapper);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(MappedError);
        errorMapper.Received(1).Invoke(exception);
    }

    [Fact]
    public void Try_WithErrorMapper_FuncReturnsNull_ThrowsArgumentNullExceptionWithoutCallingMapper()
    {
        var errorMapper = Substitute.For<Func<Exception, Error>>();

        Should.Throw<ArgumentNullException>(() => Result.Try<String>(() => null!, errorMapper));

        errorMapper.DidNotReceiveWithAnyArgs().Invoke(default!);
    }

    [Fact]
    public void Try_WithErrorMapperReturningNull_Throws_ThrowsArgumentNullException()
    {
        var exception = Should.Throw<ArgumentNullException>(
            () => Result.Try<Int32>(() => throw new InvalidOperationException("boom"), _ => null!));

        exception.ParamName.ShouldBe("error");
    }

    [Fact]
    public void Try_WithErrorMapper_NullFunc_ThrowsArgumentNullException()
    {
        var exception = Should.Throw<ArgumentNullException>(() => Result.Try<Int32>(null!, Error.FromException));

        exception.ParamName.ShouldBe("func");
    }

    [Fact]
    public void Try_NullErrorMapper_ThrowsArgumentNullException()
    {
        var exception = Should.Throw<ArgumentNullException>(() => Result.Try(() => 5, null!));

        exception.ParamName.ShouldBe("errorMapper");
    }

    [Fact]
    public async Task TryAsync_Success_ReturnsValue()
    {
        var result = await Result.TryAsync(() => Task.FromResult(5));

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(5);
    }

    [Fact]
    public async Task TryAsync_TaskFaults_ReturnsErrorFromException()
    {
        var exception = new InvalidOperationException("boom");

        var result = await Result.TryAsync(() => Task.FromException<Int32>(exception));

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(Error.FromException(exception));
        result.Error.Kind.ShouldBe(ErrorKind.Unexpected);
        result.Error.Code.ShouldBe("InvalidOperationException");
        result.Error.Message.ShouldBe("boom");
    }

    [Fact]
    public async Task TryAsync_ThrowsSynchronously_ReturnsErrorFromException()
    {
        var result = await Result.TryAsync<Int32>(() => throw new InvalidOperationException("boom"));

        result.IsFailure.ShouldBeTrue();
        result.Error.Kind.ShouldBe(ErrorKind.Unexpected);
        result.Error.Code.ShouldBe("InvalidOperationException");
        result.Error.Message.ShouldBe("boom");
    }

    [Fact]
    public async Task TryAsync_FuncReturnsNull_ThrowsArgumentNullExceptionWhenAwaited()
    {
        var task = Result.TryAsync(() => Task.FromResult<String>(null!));

        await Should.ThrowAsync<ArgumentNullException>(task);
    }

    [Fact]
    public async Task TryAsync_NullFunc_ThrowsArgumentNullExceptionWhenAwaited()
    {
        var task = Result.TryAsync<Int32>(null!);

        var exception = await Should.ThrowAsync<ArgumentNullException>(task);

        exception.ParamName.ShouldBe("funcAsync");
    }

    [Fact]
    public async Task TryAsync_WithErrorMapper_Success_ReturnsValueWithoutCallingMapper()
    {
        var errorMapper = Substitute.For<Func<Exception, Error>>();

        var result = await Result.TryAsync(() => Task.FromResult(5), errorMapper);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(5);
        errorMapper.DidNotReceiveWithAnyArgs().Invoke(default!);
    }

    [Fact]
    public async Task TryAsync_WithErrorMapper_TaskFaults_ReturnsMappedError()
    {
        var exception = new InvalidOperationException("boom");
        var errorMapper = Substitute.For<Func<Exception, Error>>();
        errorMapper(exception).Returns(MappedError);

        var result = await Result.TryAsync(() => Task.FromException<Int32>(exception), errorMapper);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(MappedError);
        errorMapper.Received(1).Invoke(exception);
    }

    [Fact]
    public async Task TryAsync_WithErrorMapper_FuncReturnsNull_ThrowsArgumentNullExceptionWithoutCallingMapper()
    {
        var errorMapper = Substitute.For<Func<Exception, Error>>();

        var task = Result.TryAsync(() => Task.FromResult<String>(null!), errorMapper);

        await Should.ThrowAsync<ArgumentNullException>(task);
        errorMapper.DidNotReceiveWithAnyArgs().Invoke(default!);
    }

    [Fact]
    public async Task TryAsync_WithErrorMapperReturningNull_TaskFaults_ThrowsArgumentNullException()
    {
        var exception = await Should.ThrowAsync<ArgumentNullException>(
            () => Result.TryAsync(
                () => Task.FromException<Int32>(new InvalidOperationException("boom")),
                _ => null!));

        exception.ParamName.ShouldBe("error");
    }

    [Fact]
    public async Task TryAsync_WithErrorMapper_NullFunc_ThrowsArgumentNullExceptionWhenAwaited()
    {
        var task = Result.TryAsync<Int32>(null!, Error.FromException);

        var exception = await Should.ThrowAsync<ArgumentNullException>(task);

        exception.ParamName.ShouldBe("funcAsync");
    }

    [Fact]
    public async Task TryAsync_NullErrorMapper_ThrowsArgumentNullExceptionWhenAwaited()
    {
        var task = Result.TryAsync(() => Task.FromResult(5), null!);

        var exception = await Should.ThrowAsync<ArgumentNullException>(task);

        exception.ParamName.ShouldBe("errorMapper");
    }

    [Fact]
    public void Try_ThrowsOperationCanceled_Rethrows()
    {
        Should.Throw<OperationCanceledException>(() => Result.Try<Int32>(() => throw new OperationCanceledException()));
    }

    [Fact]
    public void Try_WithMapper_ThrowsTaskCanceled_RethrowsWithoutCallingMapper()
    {
        var errorMapper = Substitute.For<Func<Exception, Error>>();

        Should.Throw<TaskCanceledException>(() => Result.Try<Int32>(() => throw new TaskCanceledException(), errorMapper));

        errorMapper.DidNotReceiveWithAnyArgs().Invoke(default!);
    }

    [Fact]
    public async Task TryAsync_ThrowsOperationCanceled_Rethrows()
    {
        using var cancellation = new CancellationTokenSource();
        await cancellation.CancelAsync();

        await Should.ThrowAsync<OperationCanceledException>(
            () => Result.TryAsync(() => Task.FromCanceled<Int32>(cancellation.Token)));
    }

    [Fact]
    public async Task TryAsync_WithMapper_ThrowsOperationCanceled_RethrowsWithoutCallingMapper()
    {
        var errorMapper = Substitute.For<Func<Exception, Error>>();

        await Should.ThrowAsync<OperationCanceledException>(
            () => Result.TryAsync<Int32>(() => throw new OperationCanceledException(), errorMapper));

        errorMapper.DidNotReceiveWithAnyArgs().Invoke(default!);
    }
}
