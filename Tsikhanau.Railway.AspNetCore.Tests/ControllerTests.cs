using Microsoft.AspNetCore.Mvc;

namespace Tsikhanau.Railway.AspNetCore.Tests;

public class ControllerTests
{
    [Fact]
    public async Task Action_Success_WritesOkWithJsonValue()
    {
        var response = await HttpTestHost.GetFromControllerAsync("/users/1");

        response.StatusCode.ShouldBe(200);
        response.Body.ShouldNotBeNull().GetProperty("name").GetString().ShouldBe("bob");
    }

    [Fact]
    public async Task Action_Failure_WritesProblem()
    {
        var response = await HttpTestHost.GetFromControllerAsync("/users/0");

        response.StatusCode.ShouldBe(404);
        response.ContentType.ShouldBe("application/problem+json");
        response.Body.ShouldNotBeNull().GetProperty("code").GetString().ShouldBe("user.not_found");
    }

    [Fact]
    public async Task AsyncAction_Failure_WritesValidationProblem()
    {
        var response = await HttpTestHost.GetFromControllerAsync("/users/validate");

        response.StatusCode.ShouldBe(400);
        response.Body.ShouldNotBeNull().GetProperty("errors").GetProperty("email")[0].GetString().ShouldBe("Invalid");
    }
}

[ApiController]
[Route("users")]
public class UsersTestController : ControllerBase
{
    [HttpGet("{id:int}")]
    public Microsoft.AspNetCore.Http.IResult Get(Int32 id) =>
        Find(id).Map(static name => new UserDto(name)).ToHttpResult();

    [HttpGet("validate")]
    public Task<Microsoft.AspNetCore.Http.IResult> Validate() =>
        ValidateAsync().ToHttpResultAsync();

    private static Result<String> Find(Int32 id) =>
        id == 0 ? Error.NotFound("user.not_found", "User not found") : "bob";

    private static async Task<Result<UserDto>> ValidateAsync()
    {
        await Task.Yield();
        return Error.Validation("email", "Invalid");
    }

    public sealed record UserDto(String Name);
}
