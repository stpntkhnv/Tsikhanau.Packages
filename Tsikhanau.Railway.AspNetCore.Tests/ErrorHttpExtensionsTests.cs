using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace Tsikhanau.Railway.AspNetCore.Tests;

public class ErrorHttpExtensionsTests
{
    [Theory]
    [InlineData(ErrorKind.Failure, 422)]
    [InlineData(ErrorKind.Unexpected, 500)]
    [InlineData(ErrorKind.Validation, 400)]
    [InlineData(ErrorKind.NotFound, 404)]
    [InlineData(ErrorKind.Conflict, 409)]
    [InlineData(ErrorKind.Unauthorized, 401)]
    [InlineData(ErrorKind.Forbidden, 403)]
    [InlineData((ErrorKind)99, 500)]
    public void ToStatusCode_Kind_ReturnsHttpStatusCode(ErrorKind kind, Int32 statusCode)
    {
        kind.ToStatusCode().ShouldBe(statusCode);
    }

    [Theory]
    [InlineData(ErrorKind.Failure, 422)]
    [InlineData(ErrorKind.NotFound, 404)]
    [InlineData(ErrorKind.Conflict, 409)]
    [InlineData(ErrorKind.Unauthorized, 401)]
    [InlineData(ErrorKind.Forbidden, 403)]
    public async Task ToHttpResult_KnownKind_WritesProblemWithDetailAndCode(ErrorKind kind, Int32 statusCode)
    {
        var error = new Error(kind, "some.code", "Something went wrong");

        var response = await HttpTestHost.GetAsync(() => error.ToHttpResult());

        response.StatusCode.ShouldBe(statusCode);
        response.ContentType.ShouldBe("application/problem+json");
        var body = response.Body.ShouldNotBeNull();
        body.GetProperty("status").GetInt32().ShouldBe(statusCode);
        body.GetProperty("title").GetString().ShouldNotBeNullOrWhiteSpace();
        body.GetProperty("detail").GetString().ShouldBe("Something went wrong");
        body.GetProperty("code").GetString().ShouldBe("some.code");
        body.GetProperty("traceId").GetString().ShouldNotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task ToHttpResult_Unexpected_WritesProblemWithoutDetailAndCode()
    {
        var error = Error.Unexpected("db.timeout", "Connection string host=secret timed out");

        var response = await HttpTestHost.GetAsync(() => error.ToHttpResult());

        response.StatusCode.ShouldBe(500);
        response.ContentType.ShouldBe("application/problem+json");
        var body = response.Body.ShouldNotBeNull();
        body.TryGetProperty("detail", out _).ShouldBeFalse();
        body.TryGetProperty("code", out _).ShouldBeFalse();
        body.GetRawText().ShouldNotContain("secret");
    }

    [Fact]
    public async Task ToHttpResult_FromException_DoesNotLeakExceptionMessage()
    {
        var error = Error.FromException(new InvalidOperationException("password=hunter2"));

        var response = await HttpTestHost.GetAsync(() => error.ToHttpResult());

        response.StatusCode.ShouldBe(500);
        response.Body.ShouldNotBeNull().GetRawText().ShouldNotContain("hunter2");
    }

    [Fact]
    public async Task ToHttpResult_UndefinedKind_WritesInternalServerErrorWithoutDetail()
    {
        var error = new Error((ErrorKind)99, "odd.kind", "Odd kind");

        var response = await HttpTestHost.GetAsync(() => error.ToHttpResult());

        response.StatusCode.ShouldBe(500);
        response.Body.ShouldNotBeNull().TryGetProperty("detail", out _).ShouldBeFalse();
    }

    [Fact]
    public async Task ToHttpResult_ValidationError_WritesValidationProblemGroupedByField()
    {
        var error = ValidationError.From(
        [
            new FieldError("email", "Invalid"),
            new FieldError("name", "Required"),
            new FieldError("email", "Too long")
        ]);

        var response = await HttpTestHost.GetAsync(() => error.ToHttpResult());

        response.StatusCode.ShouldBe(400);
        response.ContentType.ShouldBe("application/problem+json");
        var body = response.Body.ShouldNotBeNull();
        body.GetProperty("code").GetString().ShouldBe("validation");
        var errors = body.GetProperty("errors");
        ReadStrings(errors.GetProperty("email")).ShouldBe(["Invalid", "Too long"]);
        ReadStrings(errors.GetProperty("name")).ShouldBe(["Required"]);
    }

    [Fact]
    public async Task ToHttpResult_ValidationErrorWithEmptyField_UsesEmptyKey()
    {
        var error = Error.Validation("", "Object is invalid");

        var response = await HttpTestHost.GetAsync(() => error.ToHttpResult());

        response.StatusCode.ShouldBe(400);
        ReadStrings(response.Body.ShouldNotBeNull().GetProperty("errors").GetProperty("")).ShouldBe(["Object is invalid"]);
    }

    [Fact]
    public async Task ToHttpResult_AggregateOfMixedKinds_WritesUnprocessableEntityWithJoinedDetail()
    {
        var error = AggregateError.From([Error.NotFound("a.missing", "A missing"), Error.Conflict("b.taken", "B taken")]);

        var response = await HttpTestHost.GetAsync(() => error.ToHttpResult());

        response.StatusCode.ShouldBe(422);
        var body = response.Body.ShouldNotBeNull();
        body.GetProperty("detail").GetString().ShouldBe("A missing; B taken");
        body.GetProperty("code").GetString().ShouldBe("aggregate");
    }

    [Fact]
    public async Task ToHttpResult_AggregateOfSameKind_WritesThatKindStatus()
    {
        var error = AggregateError.From([Error.NotFound("a.missing", "A missing"), Error.NotFound("b.missing", "B missing")]);

        var response = await HttpTestHost.GetAsync(() => error.ToHttpResult());

        response.StatusCode.ShouldBe(404);
    }

    [Fact]
    public async Task ToHttpResult_AggregateContainingUnexpected_WritesInternalServerErrorWithoutDetail()
    {
        var error = AggregateError.From([Error.NotFound("a.missing", "A missing"), Error.Unexpected("db.down", "Database secret down")]);

        var response = await HttpTestHost.GetAsync(() => error.ToHttpResult());

        response.StatusCode.ShouldBe(500);
        var body = response.Body.ShouldNotBeNull();
        body.TryGetProperty("detail", out _).ShouldBeFalse();
        body.GetRawText().ShouldNotContain("secret");
    }

    [Fact]
    public async Task ToHttpResult_CustomErrorRecord_UsesItsKindAndCode()
    {
        var error = new AccountLockedError(3);

        var response = await HttpTestHost.GetAsync(() => error.ToHttpResult());

        response.StatusCode.ShouldBe(403);
        response.Body.ShouldNotBeNull().GetProperty("code").GetString().ShouldBe("account.locked");
    }

    [Fact]
    public async Task ToHttpResult_ErrorWithMetadata_DoesNotExposeMetadata()
    {
        var error = Error.Conflict("order.duplicate", "Order exists").WithMetadata("internalId", "db-42");

        var response = await HttpTestHost.GetAsync(() => error.ToHttpResult());

        response.StatusCode.ShouldBe(409);
        var body = response.Body.ShouldNotBeNull();
        body.TryGetProperty("internalId", out _).ShouldBeFalse();
        body.GetRawText().ShouldNotContain("db-42");
    }

    [Fact]
    public void ToHttpResult_NullError_ThrowsArgumentNullException()
    {
        Should.Throw<ArgumentNullException>(() => ((Error)null!).ToHttpResult());
    }

    private static String[] ReadStrings(JsonElement array) =>
        array.EnumerateArray().Select(static e => e.GetString()!).ToArray();

    private sealed record AccountLockedError(Int32 Attempts)
        : Error(ErrorKind.Forbidden, "account.locked", $"Account locked after {Attempts} attempts");
}
