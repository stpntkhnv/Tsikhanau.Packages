using Microsoft.AspNetCore.Http.HttpResults;

namespace Tsikhanau.Railway.AspNetCore.Tests;

public class ErrorProblemTests
{
    [Fact]
    public void ExpectedKinds_ListsTheKindsThatAnswerWithTheirOwnStatus()
    {
        ErrorHttpExtensions.ExpectedKinds.ShouldBe(
        [
            ErrorKind.Validation,
            ErrorKind.Unauthorized,
            ErrorKind.Forbidden,
            ErrorKind.NotFound,
            ErrorKind.Conflict,
            ErrorKind.Failure
        ]);
    }

    [Theory]
    [InlineData(ErrorKind.Validation)]
    [InlineData(ErrorKind.Unauthorized)]
    [InlineData(ErrorKind.Forbidden)]
    [InlineData(ErrorKind.NotFound)]
    [InlineData(ErrorKind.Conflict)]
    [InlineData(ErrorKind.Failure)]
    public void IsExpected_ExpectedKind_ReturnsTrue(ErrorKind kind)
    {
        var error = new Error(kind, "some.code", "Something went wrong");

        var isExpected = error.IsExpected();

        isExpected.ShouldBeTrue();
    }

    [Theory]
    [InlineData(ErrorKind.Unexpected)]
    [InlineData((ErrorKind)99)]
    public void IsExpected_OtherKind_ReturnsFalse(ErrorKind kind)
    {
        var error = new Error(kind, "some.code", "Something went wrong");

        var isExpected = error.IsExpected();

        isExpected.ShouldBeFalse();
    }

    [Fact]
    public void IsExpected_AggregateOfExpectedErrors_ReturnsTrue()
    {
        var error = AggregateError.From(
            [Error.NotFound("a.missing", "A missing"), Error.Conflict("b.taken", "B taken")]);

        var isExpected = error.IsExpected();

        isExpected.ShouldBeTrue();
    }

    [Fact]
    public void IsExpected_AggregateHoldingUnexpectedDeepInside_ReturnsFalse()
    {
        var inner = AggregateError.From([Error.Conflict("b.taken", "B taken"), Error.Unexpected("db.down", "DB down")]);
        var error = AggregateError.From([Error.NotFound("a.missing", "A missing"), inner]);

        var isExpected = error.IsExpected();

        isExpected.ShouldBeFalse();
    }

    [Fact]
    public void IsExpected_ExpectedErrorWrappingUnexpected_ReturnsTrue()
    {
        var error = new Error(ErrorKind.Conflict, "order.taken", "Order is taken")
        {
            Inner = Error.Unexpected("db.down", "DB down")
        };

        var isExpected = error.IsExpected();

        isExpected.ShouldBeTrue();
    }

    [Fact]
    public void IsExpected_NullError_ThrowsArgumentNullException()
    {
        Should.Throw<ArgumentNullException>(() => ((Error)null!).IsExpected());
    }

    [Theory]
    [InlineData(ErrorKind.Failure, 422, "https://tools.ietf.org/html/rfc4918#section-11.2", "Unprocessable Entity")]
    [InlineData(ErrorKind.NotFound, 404, "https://tools.ietf.org/html/rfc9110#section-15.5.5", "Not Found")]
    [InlineData(ErrorKind.Conflict, 409, "https://tools.ietf.org/html/rfc9110#section-15.5.10", "Conflict")]
    [InlineData(ErrorKind.Unauthorized, 401, "https://tools.ietf.org/html/rfc9110#section-15.5.2", "Unauthorized")]
    [InlineData(ErrorKind.Forbidden, 403, "https://tools.ietf.org/html/rfc9110#section-15.5.4", "Forbidden")]
    public async Task ToProblem_ExpectedKind_WritesProblemWithDetailAndCode(
        ErrorKind kind,
        Int32 statusCode,
        String type,
        String title)
    {
        var error = new Error(kind, "some.code", "Something went wrong");
        var expectedBody = $$"""
            {"type":"{{type}}","title":"{{title}}","status":{{statusCode}},"detail":"Something went wrong","code":"some.code","traceId":"*"}
            """;

        var response = await HttpTestHost.GetAsync(() => error.ToProblem<Ok<String>>());
        var body = ProblemBody.WithAnyTraceId(response);

        response.StatusCode.ShouldBe(statusCode);
        body.ShouldBe(expectedBody);
    }

    [Theory]
    [InlineData(ErrorKind.Unexpected)]
    [InlineData((ErrorKind)99)]
    public async Task ToProblem_OtherKind_WritesInternalServerErrorWithoutDetailAndCode(ErrorKind kind)
    {
        var error = new Error(kind, "db.timeout", "Connection string host=secret timed out");
        const String expectedBody = """
            {"type":"https://tools.ietf.org/html/rfc9110#section-15.6.1","title":"An error occurred while processing your request.","status":500,"traceId":"*"}
            """;

        var response = await HttpTestHost.GetAsync(() => error.ToProblem<Ok<String>>());
        var body = ProblemBody.WithAnyTraceId(response);

        response.StatusCode.ShouldBe(500);
        body.ShouldBe(expectedBody);
    }

    [Fact]
    public async Task ToProblem_ValidationError_WritesMessagesGroupedByField()
    {
        var error = ValidationError.From(
        [
            new FieldError("email", "Invalid"),
            new FieldError("name", "Required"),
            new FieldError("email", "Too long")
        ]);

        const String expectedBody = """
            {"type":"https://tools.ietf.org/html/rfc9110#section-15.5.1","title":"One or more validation errors occurred.","status":400,"errors":{"email":["Invalid","Too long"],"name":["Required"]},"code":"validation","traceId":"*"}
            """;

        var response = await HttpTestHost.GetAsync(() => error.ToProblem<Ok<String>>());
        var body = ProblemBody.WithAnyTraceId(response);

        response.StatusCode.ShouldBe(400);
        body.ShouldBe(expectedBody);
    }

    [Fact]
    public async Task ToProblem_ValidationKindWithoutFields_WritesMessageUnderEmptyKey()
    {
        var error = new Error(ErrorKind.Validation, "name.invalid", "Name is invalid");
        const String expectedBody = """
            {"type":"https://tools.ietf.org/html/rfc9110#section-15.5.1","title":"One or more validation errors occurred.","status":400,"errors":{"":["Name is invalid"]},"code":"name.invalid","traceId":"*"}
            """;

        var response = await HttpTestHost.GetAsync(() => error.ToProblem<Ok<String>>());
        var body = ProblemBody.WithAnyTraceId(response);

        response.StatusCode.ShouldBe(400);
        body.ShouldBe(expectedBody);
    }

    [Fact]
    public async Task ToProblem_WrappedValidationError_WritesOuterMessageAndInnerFields()
    {
        var error = ValidationError
            .From([new FieldError("email", "Invalid")])
            .WithContext("signup.invalid", "Signup is invalid");

        const String expectedBody = """
            {"type":"https://tools.ietf.org/html/rfc9110#section-15.5.1","title":"One or more validation errors occurred.","status":400,"errors":{"":["Signup is invalid"],"email":["Invalid"]},"code":"signup.invalid","traceId":"*"}
            """;

        var response = await HttpTestHost.GetAsync(() => error.ToProblem<Ok<String>>());
        var body = ProblemBody.WithAnyTraceId(response);

        response.StatusCode.ShouldBe(400);
        body.ShouldBe(expectedBody);
    }

    [Fact]
    public async Task ToProblem_ValidationErrorBehindAnotherError_WritesOuterMessageAndDeepestFields()
    {
        var lines = Error.Failure("order.lines", "Order lines are wrong") with
        {
            Inner = ValidationError.From([new FieldError("lines[0].qty", "Must be positive")])
        };

        var error = new Error(ErrorKind.Validation, "order.invalid", "Order is invalid") { Inner = lines };
        const String expectedBody = """
            {"type":"https://tools.ietf.org/html/rfc9110#section-15.5.1","title":"One or more validation errors occurred.","status":400,"errors":{"":["Order is invalid"],"lines[0].qty":["Must be positive"]},"code":"order.invalid","traceId":"*"}
            """;

        var response = await HttpTestHost.GetAsync(() => error.ToProblem<Ok<String>>());
        var body = ProblemBody.WithAnyTraceId(response);

        response.StatusCode.ShouldBe(400);
        body.ShouldBe(expectedBody);
    }

    [Fact]
    public async Task ToProblem_AggregateOfValidationErrors_WritesTheirFields()
    {
        var error = AggregateError.From([Error.Validation("email", "Invalid"), Error.Validation("name", "Required")]);
        const String expectedBody = """
            {"type":"https://tools.ietf.org/html/rfc9110#section-15.5.1","title":"One or more validation errors occurred.","status":400,"errors":{"email":["Invalid"],"name":["Required"]},"code":"aggregate","traceId":"*"}
            """;

        var response = await HttpTestHost.GetAsync(() => error.ToProblem<Ok<String>>());
        var body = ProblemBody.WithAnyTraceId(response);

        response.StatusCode.ShouldBe(400);
        body.ShouldBe(expectedBody);
    }

    [Fact]
    public async Task ToProblem_AggregateOfValidationKindErrors_WritesFieldlessMessagesUnderEmptyKey()
    {
        var error = AggregateError.From(
            [new Error(ErrorKind.Validation, "a.bad", "A is bad"), Error.Validation("b", "B is bad")]);

        const String expectedBody = """
            {"type":"https://tools.ietf.org/html/rfc9110#section-15.5.1","title":"One or more validation errors occurred.","status":400,"errors":{"":["A is bad"],"b":["B is bad"]},"code":"aggregate","traceId":"*"}
            """;

        var response = await HttpTestHost.GetAsync(() => error.ToProblem<Ok<String>>());
        var body = ProblemBody.WithAnyTraceId(response);

        response.StatusCode.ShouldBe(400);
        body.ShouldBe(expectedBody);
    }

    [Fact]
    public async Task ToProblem_AggregateOfMixedKinds_WritesUnprocessableEntityWithJoinedDetail()
    {
        var error = AggregateError.From(
            [Error.NotFound("a.missing", "A missing"), Error.Conflict("b.taken", "B taken")]);

        const String expectedBody = """
            {"type":"https://tools.ietf.org/html/rfc4918#section-11.2","title":"Unprocessable Entity","status":422,"detail":"A missing; B taken","code":"aggregate","traceId":"*"}
            """;

        var response = await HttpTestHost.GetAsync(() => error.ToProblem<Ok<String>>());
        var body = ProblemBody.WithAnyTraceId(response);

        response.StatusCode.ShouldBe(422);
        body.ShouldBe(expectedBody);
    }

    [Fact]
    public async Task ToProblem_AggregateHoldingUnexpected_WritesInternalServerErrorWithoutDetail()
    {
        var error = AggregateError.From(
            [Error.NotFound("a.missing", "A missing"), Error.Unexpected("db.down", "Database secret down")]);

        const String expectedBody = """
            {"type":"https://tools.ietf.org/html/rfc9110#section-15.6.1","title":"An error occurred while processing your request.","status":500,"traceId":"*"}
            """;

        var response = await HttpTestHost.GetAsync(() => error.ToProblem<Ok<String>>());
        var body = ProblemBody.WithAnyTraceId(response);

        response.StatusCode.ShouldBe(500);
        body.ShouldBe(expectedBody);
    }

    [Fact]
    public async Task ToProblem_ValidationWrappingAggregateOfExpectedErrors_WritesEveryMessage()
    {
        var error = new Error(ErrorKind.Validation, "order.invalid", "Order is invalid")
        {
            Inner = AggregateError.From(
                [Error.Validation("email", "Invalid"), Error.Conflict("order.taken", "Order is taken")])
        };

        const String expectedBody = """
            {"type":"https://tools.ietf.org/html/rfc9110#section-15.5.1","title":"One or more validation errors occurred.","status":400,"errors":{"":["Order is invalid","Order is taken"],"email":["Invalid"]},"code":"order.invalid","traceId":"*"}
            """;

        var response = await HttpTestHost.GetAsync(() => error.ToProblem<Ok<String>>());
        var body = ProblemBody.WithAnyTraceId(response);

        response.StatusCode.ShouldBe(400);
        body.ShouldBe(expectedBody);
    }

    [Fact]
    public async Task ToProblem_ValidationWrappingUnexpected_KeepsTheUnexpectedMessageOut()
    {
        var error = new Error(ErrorKind.Validation, "order.invalid", "Order is invalid")
        {
            Inner = AggregateError.From(
                [Error.Validation("email", "Invalid"), Error.Unexpected("db.down", "Database secret down")])
        };

        const String expectedBody = """
            {"type":"https://tools.ietf.org/html/rfc9110#section-15.5.1","title":"One or more validation errors occurred.","status":400,"errors":{"":["Order is invalid"],"email":["Invalid"]},"code":"order.invalid","traceId":"*"}
            """;

        var response = await HttpTestHost.GetAsync(() => error.ToProblem<Ok<String>>());
        var body = ProblemBody.WithAnyTraceId(response);

        response.StatusCode.ShouldBe(400);
        body.ShouldBe(expectedBody);
    }

    [Fact]
    public async Task ToProblem_UnexpectedWrappingValidationError_KeepsTheFieldsAndDropsItsMessage()
    {
        var unexpected = Error.Unexpected("db.down", "Database secret down") with
        {
            Inner = Error.Validation("name", "Required")
        };

        var error = new Error(ErrorKind.Validation, "order.invalid", "Order is invalid")
        {
            Inner = AggregateError.From([Error.Validation("email", "Invalid"), unexpected])
        };

        const String expectedBody = """
            {"type":"https://tools.ietf.org/html/rfc9110#section-15.5.1","title":"One or more validation errors occurred.","status":400,"errors":{"":["Order is invalid"],"email":["Invalid"],"name":["Required"]},"code":"order.invalid","traceId":"*"}
            """;

        var response = await HttpTestHost.GetAsync(() => error.ToProblem<Ok<String>>());
        var body = ProblemBody.WithAnyTraceId(response);

        response.StatusCode.ShouldBe(400);
        body.ShouldBe(expectedBody);
    }

    [Fact]
    public void ToProblem_ExpectedKind_WrapsProblemHttpResult()
    {
        var error = Error.NotFound("item.not_found", "Item not found");

        var problem = error.ToProblem<Ok<String>>();

        problem.Result.ShouldBeOfType<ProblemHttpResult>();
    }

    [Fact]
    public void ToProblem_ValidationKind_WrapsValidationProblem()
    {
        var error = new Error(ErrorKind.Validation, "name.invalid", "Name is invalid");

        var problem = error.ToProblem<Ok<String>>();

        problem.Result.ShouldBeOfType<ValidationProblem>();
    }

    [Fact]
    public void ToProblem_NullError_ThrowsArgumentNullException()
    {
        Should.Throw<ArgumentNullException>(() => ((Error)null!).ToProblem<Ok<String>>());
    }
}
