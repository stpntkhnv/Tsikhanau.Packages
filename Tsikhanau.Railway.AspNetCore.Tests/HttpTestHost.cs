using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Tsikhanau.Railway.AspNetCore.Tests;

internal sealed record HttpTestResponse(Int32 StatusCode, String? ContentType, String? Location, JsonElement? Body);

internal static class HttpTestHost
{
    public static async Task<HttpTestResponse> GetAsync(Delegate handler)
    {
        var builder = WebApplication.CreateSlimBuilder();
        builder.WebHost.UseTestServer();
        builder.Logging.ClearProviders();
        builder.Services.AddProblemDetails();

        await using var app = builder.Build();
        app.MapGet("/", handler);
        await app.StartAsync(TestContext.Current.CancellationToken);

        return await SendAsync(app, "/");
    }

    public static async Task<HttpTestResponse> GetFromControllerAsync(String path)
    {
        var builder = WebApplication.CreateSlimBuilder();
        builder.WebHost.UseTestServer();
        builder.Logging.ClearProviders();
        builder.Services.AddProblemDetails();
        builder.Services.AddControllers().AddApplicationPart(typeof(HttpTestHost).Assembly);

        await using var app = builder.Build();
        app.MapControllers();
        await app.StartAsync(TestContext.Current.CancellationToken);

        return await SendAsync(app, path);
    }

    private static async Task<HttpTestResponse> SendAsync(WebApplication app, String path)
    {
        using var client = app.GetTestClient();
        using var response = await client.GetAsync(path, TestContext.Current.CancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        return new HttpTestResponse(
            (Int32)response.StatusCode,
            response.Content.Headers.ContentType?.MediaType,
            response.Headers.Location?.ToString(),
            content.Length == 0 ? null : JsonDocument.Parse(content).RootElement.Clone());
    }
}
