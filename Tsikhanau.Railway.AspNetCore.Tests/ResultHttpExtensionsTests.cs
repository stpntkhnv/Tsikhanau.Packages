using Microsoft.AspNetCore.Http;

namespace Tsikhanau.Railway.AspNetCore.Tests;

public class ResultHttpExtensionsTests
{
    private static readonly Error NotFound = Error.NotFound("item.not_found", "Item not found");

    [Fact]
    public async Task ToHttpResult_Success_WritesOkWithJsonValue()
    {
        var response = await HttpTestHost.GetAsync(() => Result.Success(new ItemDto(1, "Book")).ToHttpResult());

        response.StatusCode.ShouldBe(200);
        response.ContentType.ShouldBe("application/json");
        var body = response.Body.ShouldNotBeNull();
        body.GetProperty("id").GetInt32().ShouldBe(1);
        body.GetProperty("name").GetString().ShouldBe("Book");
    }

    [Fact]
    public async Task ToHttpResult_Failure_WritesProblem()
    {
        var response = await HttpTestHost.GetAsync(() => Result.Failure<ItemDto>(NotFound).ToHttpResult());

        response.StatusCode.ShouldBe(404);
        response.ContentType.ShouldBe("application/problem+json");
        response.Body.ShouldNotBeNull().GetProperty("code").GetString().ShouldBe("item.not_found");
    }

    [Fact]
    public async Task ToHttpResult_UnitSuccess_WritesNoContent()
    {
        var response = await HttpTestHost.GetAsync(() => Result.Success().ToHttpResult());

        response.StatusCode.ShouldBe(204);
        response.Body.ShouldBeNull();
    }

    [Fact]
    public async Task ToHttpResult_UnitFailure_WritesProblem()
    {
        var response = await HttpTestHost.GetAsync(() => Result.Failure(NotFound).ToHttpResult());

        response.StatusCode.ShouldBe(404);
        response.ContentType.ShouldBe("application/problem+json");
    }

    [Fact]
    public async Task ToHttpResult_WithOnSuccess_Success_WritesCustomResult()
    {
        var response = await HttpTestHost.GetAsync(() =>
            Result.Success(new ItemDto(7, "Pen")).ToHttpResult(item => TypedResults.Created($"/items/{item.Id}", item)));

        response.StatusCode.ShouldBe(201);
        response.Location.ShouldBe("/items/7");
        response.Body.ShouldNotBeNull().GetProperty("name").GetString().ShouldBe("Pen");
    }

    [Fact]
    public async Task ToHttpResult_WithOnSuccess_Failure_WritesProblemWithoutCallingOnSuccess()
    {
        var onSuccess = Substitute.For<Func<ItemDto, IResult>>();

        var response = await HttpTestHost.GetAsync(() => Result.Failure<ItemDto>(NotFound).ToHttpResult(onSuccess));

        response.StatusCode.ShouldBe(404);
        onSuccess.DidNotReceiveWithAnyArgs().Invoke(default!);
    }

    [Fact]
    public void ToHttpResult_NullOnSuccess_ThrowsArgumentNullException()
    {
        Should.Throw<ArgumentNullException>(() => Result.Success(1).ToHttpResult(null!));
    }

    [Fact]
    public async Task ToHttpResultAsync_Success_WritesOkWithJsonValue()
    {
        var response = await HttpTestHost.GetAsync(() => LoadAsync(Result.Success(new ItemDto(2, "Cup"))).ToHttpResultAsync());

        response.StatusCode.ShouldBe(200);
        response.Body.ShouldNotBeNull().GetProperty("name").GetString().ShouldBe("Cup");
    }

    [Fact]
    public async Task ToHttpResultAsync_Failure_WritesProblem()
    {
        var response = await HttpTestHost.GetAsync(() => LoadAsync(Result.Failure<ItemDto>(NotFound)).ToHttpResultAsync());

        response.StatusCode.ShouldBe(404);
        response.ContentType.ShouldBe("application/problem+json");
    }

    [Fact]
    public async Task ToHttpResultAsync_UnitSuccess_WritesNoContent()
    {
        var response = await HttpTestHost.GetAsync(() => LoadAsync(Result.Success()).ToHttpResultAsync());

        response.StatusCode.ShouldBe(204);
        response.Body.ShouldBeNull();
    }

    [Fact]
    public async Task ToHttpResultAsync_WithOnSuccess_Success_WritesCustomResult()
    {
        var response = await HttpTestHost.GetAsync(() =>
            LoadAsync(Result.Success(new ItemDto(9, "Lamp")))
                .ToHttpResultAsync(item => TypedResults.Created($"/items/{item.Id}", item)));

        response.StatusCode.ShouldBe(201);
        response.Location.ShouldBe("/items/9");
    }

    [Fact]
    public async Task ToHttpResultAsync_WithOnSuccess_Failure_WritesProblemWithoutCallingOnSuccess()
    {
        var onSuccess = Substitute.For<Func<ItemDto, IResult>>();

        var response = await HttpTestHost.GetAsync(() => LoadAsync(Result.Failure<ItemDto>(NotFound)).ToHttpResultAsync(onSuccess));

        response.StatusCode.ShouldBe(404);
        onSuccess.DidNotReceiveWithAnyArgs().Invoke(default!);
    }

    [Fact]
    public async Task ToHttpResultAsync_NullOnSuccess_ThrowsArgumentNullExceptionWhenAwaited()
    {
        var task = LoadAsync(Result.Success(1)).ToHttpResultAsync(null!);

        await Should.ThrowAsync<ArgumentNullException>(task);
    }

    private static async Task<Result<T>> LoadAsync<T>(Result<T> result)
        where T : notnull
    {
        await Task.Yield();
        return result;
    }

    private sealed record ItemDto(Int32 Id, String Name);
}
