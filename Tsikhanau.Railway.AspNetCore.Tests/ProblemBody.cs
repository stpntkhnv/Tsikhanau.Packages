using System.Text.RegularExpressions;

namespace Tsikhanau.Railway.AspNetCore.Tests;

internal static partial class ProblemBody
{
    public static String WithAnyTraceId(HttpTestResponse response) =>
        TraceId().Replace(response.Body.ShouldNotBeNull().GetRawText(), "\"traceId\":\"*\"");

    [GeneratedRegex("\"traceId\":\"[^\"]+\"")]
    private static partial Regex TraceId();
}
