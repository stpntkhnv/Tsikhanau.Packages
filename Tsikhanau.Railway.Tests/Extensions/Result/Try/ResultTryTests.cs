namespace Tsikhanau.Railway.Tests;

public class ResultTryTests
{
    private static readonly Error MappedError = Error.Create("MAPPED", "mapped failure");

    [Fact]
    public void Try_Success_ReturnsValue()
    {
        var result = ResultExtensions.Try(() => 5);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(5);
    }

    [Fact]
    public void Try_Throws_ReturnsErrorFromException()
    {
        var result = ResultExtensions.Try<Int32>(() => throw new InvalidOperationException("boom"));

        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldBe("InvalidOperationException");
        result.Error.Message.ShouldBe("boom");
        result.Error.InnerError.ShouldBeNull();
    }

    [Fact]
    public void Try_ThrowsWithInnerException_ReturnsErrorWithInnerError()
    {
        var result = ResultExtensions.Try<Int32>(
            () => throw new InvalidOperationException("outer", new FormatException("inner")));

        result.Error.GetAllCodes().ShouldBe(new[] { "InvalidOperationException", "FormatException" });
        result.Error.GetFullMessage().ShouldBe("outer -> inner");
    }

    [Fact]
    public void Try_FuncReturnsNull_ReturnsArgumentNullExceptionError()
    {
        var result = ResultExtensions.Try<String>(() => null!);

        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldBe("ArgumentNullException");
    }

    [Fact]
    public void Try_WithErrorMapper_Success_ReturnsValueWithoutCallingMapper()
    {
        var errorMapper = Substitute.For<Func<Exception, Error>>();

        var result = ResultExtensions.Try(() => 5, errorMapper);

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

        var result = ResultExtensions.Try<Int32, Error>(() => throw exception, errorMapper);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(MappedError);
        errorMapper.Received(1).Invoke(exception);
    }

    [Fact]
    public void Try_WithErrorMapperToCustomErrorType_Throws_ReturnsMappedError()
    {
        var result = ResultExtensions.Try<Int32, String>(
            () => throw new InvalidOperationException("boom"),
            ex => ex.Message);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe("boom");
    }

    [Fact]
    public void Try_WithErrorMapper_FuncReturnsNull_PassesArgumentNullExceptionToMapper()
    {
        var errorMapper = Substitute.For<Func<Exception, Error>>();
        errorMapper(Arg.Any<Exception>()).Returns(MappedError);

        var result = ResultExtensions.Try<String, Error>(() => null!, errorMapper);

        result.Error.ShouldBe(MappedError);
        errorMapper.Received(1).Invoke(Arg.Is<Exception>(e => e is ArgumentNullException));
    }

    [Fact]
    public void Try_WithErrorMapperReturningNull_Throws_ThrowsArgumentNullException()
    {
        var exception = Should.Throw<ArgumentNullException>(
            () => ResultExtensions.Try<Int32, Error>(() => throw new InvalidOperationException("boom"), _ => null!));

        exception.ParamName.ShouldBe("error");
    }

    [Fact]
    public async Task TryAsync_Success_ReturnsValue()
    {
        var result = await ResultExtensions.TryAsync(() => Task.FromResult(5));

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(5);
    }

    [Fact]
    public async Task TryAsync_TaskFaults_ReturnsErrorFromException()
    {
        var result = await ResultExtensions.TryAsync(
            () => Task.FromException<Int32>(new InvalidOperationException("boom")));

        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldBe("InvalidOperationException");
        result.Error.Message.ShouldBe("boom");
    }

    [Fact]
    public async Task TryAsync_ThrowsSynchronously_ReturnsErrorFromException()
    {
        var result = await ResultExtensions.TryAsync<Int32>(() => throw new InvalidOperationException("boom"));

        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldBe("InvalidOperationException");
        result.Error.Message.ShouldBe("boom");
    }

    [Fact]
    public async Task TryAsync_FuncReturnsNull_ReturnsArgumentNullExceptionError()
    {
        var result = await ResultExtensions.TryAsync(() => Task.FromResult<String>(null!));

        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldBe("ArgumentNullException");
    }

    [Fact]
    public async Task TryAsync_WithErrorMapper_Success_ReturnsValueWithoutCallingMapper()
    {
        var errorMapper = Substitute.For<Func<Exception, Error>>();

        var result = await ResultExtensions.TryAsync(() => Task.FromResult(5), errorMapper);

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

        var result = await ResultExtensions.TryAsync(() => Task.FromException<Int32>(exception), errorMapper);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(MappedError);
        errorMapper.Received(1).Invoke(exception);
    }

    [Fact]
    public async Task TryAsync_WithErrorMapperToCustomErrorType_TaskFaults_ReturnsMappedError()
    {
        var result = await ResultExtensions.TryAsync(
            () => Task.FromException<Int32>(new InvalidOperationException("boom")),
            ex => ex.Message);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe("boom");
    }

    [Fact]
    public async Task TryAsync_WithErrorMapper_FuncReturnsNull_PassesArgumentNullExceptionToMapper()
    {
        var errorMapper = Substitute.For<Func<Exception, Error>>();
        errorMapper(Arg.Any<Exception>()).Returns(MappedError);

        var result = await ResultExtensions.TryAsync(() => Task.FromResult<String>(null!), errorMapper);

        result.Error.ShouldBe(MappedError);
        errorMapper.Received(1).Invoke(Arg.Is<Exception>(e => e is ArgumentNullException));
    }

    [Fact]
    public async Task TryAsync_WithErrorMapperReturningNull_TaskFaults_ThrowsArgumentNullException()
    {
        var exception = await Should.ThrowAsync<ArgumentNullException>(
            () => ResultExtensions.TryAsync<Int32, Error>(
                () => Task.FromException<Int32>(new InvalidOperationException("boom")),
                _ => null!));

        exception.ParamName.ShouldBe("error");
    }
}
