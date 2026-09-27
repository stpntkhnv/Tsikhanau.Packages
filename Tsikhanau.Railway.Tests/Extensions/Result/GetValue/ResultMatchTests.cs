namespace Tsikhanau.Railway.Tests;

public class ResultMatchTests
{
    private static readonly Error SourceError = Error.Create("SOURCE", "source failed");

    [Fact]
    public void Match_Success_ReturnsOnSuccessResultWithoutCallingOnFailure()
    {
        var onFailure = Substitute.For<Func<Error, String>>();

        var result = Result.Success(2).Match(x => x.ToString(), onFailure);

        result.ShouldBe("2");
        onFailure.DidNotReceiveWithAnyArgs().Invoke(default!);
    }

    [Fact]
    public void Match_Failure_ReturnsOnFailureResultWithoutCallingOnSuccess()
    {
        var onSuccess = Substitute.For<Func<Int32, String>>();

        var result = Result.Failure<Int32>(SourceError).Match(onSuccess, e => e.Code);

        result.ShouldBe("SOURCE");
        onSuccess.DidNotReceiveWithAnyArgs().Invoke(default);
    }

    [Fact]
    public async Task MatchAsync_TaskWithSyncHandlers_Success_ReturnsOnSuccessResultWithoutCallingOnFailure()
    {
        var onFailure = Substitute.For<Func<Error, String>>();

        var result = await Task.FromResult(Result.Success(2)).MatchAsync(x => x.ToString(), onFailure);

        result.ShouldBe("2");
        onFailure.DidNotReceiveWithAnyArgs().Invoke(default!);
    }

    [Fact]
    public async Task MatchAsync_TaskWithSyncHandlers_Failure_ReturnsOnFailureResultWithoutCallingOnSuccess()
    {
        var onSuccess = Substitute.For<Func<Int32, String>>();

        var result = await Task.FromResult(Result.Failure<Int32>(SourceError)).MatchAsync(onSuccess, e => e.Code);

        result.ShouldBe("SOURCE");
        onSuccess.DidNotReceiveWithAnyArgs().Invoke(default);
    }

    [Fact]
    public async Task MatchAsync_TaskWithAsyncHandlers_Success_ReturnsOnSuccessResultWithoutCallingOnFailure()
    {
        var onFailure = Substitute.For<Func<Error, Task<String>>>();

        var result = await Task.FromResult(Result.Success(2))
            .MatchAsync(x => Task.FromResult(x.ToString()), onFailure);

        result.ShouldBe("2");
        await onFailure.DidNotReceiveWithAnyArgs().Invoke(default!);
    }

    [Fact]
    public async Task MatchAsync_TaskWithAsyncHandlers_Failure_ReturnsOnFailureResultWithoutCallingOnSuccess()
    {
        var onSuccess = Substitute.For<Func<Int32, Task<String>>>();

        var result = await Task.FromResult(Result.Failure<Int32>(SourceError))
            .MatchAsync(onSuccess, e => Task.FromResult(e.Code));

        result.ShouldBe("SOURCE");
        await onSuccess.DidNotReceiveWithAnyArgs().Invoke(default);
    }

    [Fact]
    public async Task MatchAsync_ResultWithAsyncHandlers_Success_ReturnsOnSuccessResultWithoutCallingOnFailure()
    {
        var onFailure = Substitute.For<Func<Error, Task<String>>>();

        var result = await Result.Success(2).MatchAsync(x => Task.FromResult(x.ToString()), onFailure);

        result.ShouldBe("2");
        await onFailure.DidNotReceiveWithAnyArgs().Invoke(default!);
    }

    [Fact]
    public async Task MatchAsync_ResultWithAsyncHandlers_Failure_ReturnsOnFailureResultWithoutCallingOnSuccess()
    {
        var onSuccess = Substitute.For<Func<Int32, Task<String>>>();

        var result = await Result.Failure<Int32>(SourceError).MatchAsync(onSuccess, e => Task.FromResult(e.Code));

        result.ShouldBe("SOURCE");
        await onSuccess.DidNotReceiveWithAnyArgs().Invoke(default);
    }
}
