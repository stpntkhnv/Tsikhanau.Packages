namespace Tsikhanau.Railway.AspNetCore.Tests;

public class ProblemMembersTests
{
    [Fact]
    public void ProblemMembers_NameTheBodyMembers()
    {
        ProblemMembers.Code.ShouldBe("code");
        ProblemMembers.Errors.ShouldBe("errors");
        ProblemMembers.TraceId.ShouldBe("traceId");
    }

    [Fact]
    public async Task ProblemMembers_AreTheMembersAValidationAnswerCarries()
    {
        var error = Error.Validation("email", "Invalid");

        var response = await HttpTestHost.GetAsync(() => error.ToHttpResult());
        var memberNames = response
            .Body
            .ShouldNotBeNull()
            .EnumerateObject()
            .Select(member => member.Name);

        memberNames.ShouldContain(ProblemMembers.Code);
        memberNames.ShouldContain(ProblemMembers.Errors);
        memberNames.ShouldContain(ProblemMembers.TraceId);
    }
}
