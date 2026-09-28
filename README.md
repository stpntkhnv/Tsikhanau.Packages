# Tsikhanau.Packages

[![CI](https://github.com/stpntkhnv/Tsikhanau.Packages/actions/workflows/ci.yml/badge.svg)](https://github.com/stpntkhnv/Tsikhanau.Packages/actions/workflows/ci.yml)

Railway-oriented programming for .NET 10: `Result<T>` and `Error` instead of exceptions, fluent chaining with full async support, ASP.NET Core integration and analyzers that catch common mistakes at compile time.

| Package | NuGet | |
|---|---|---|
| Tsikhanau.Railway | [![NuGet](https://img.shields.io/nuget/v/Tsikhanau.Railway)](https://www.nuget.org/packages/Tsikhanau.Railway) | Result, Optional, Error, extensions, analyzers ([README](Tsikhanau.Railway/README.md)) |
| Tsikhanau.Railway.AspNetCore | [![NuGet](https://img.shields.io/nuget/v/Tsikhanau.Railway.AspNetCore)](https://www.nuget.org/packages/Tsikhanau.Railway.AspNetCore) | Result and Error as HTTP results with ProblemDetails ([README](Tsikhanau.Railway.AspNetCore/README.md)) |

```bash
dotnet add package Tsikhanau.Railway
dotnet add package Tsikhanau.Railway.AspNetCore
```

```csharp
using Tsikhanau.Railway;

public sealed record UserBannedError(DateTime Until)
    : Error(ErrorKind.Forbidden, "user.banned", $"User is banned until {Until:yyyy-MM-dd}");

public async Task<Result<User>> GetUser(Guid id)
{
    var user = await db.Users.FindAsync(id);
    if (user is null)
        return Error.NotFound("user.not_found", "User not found");

    if (user.BannedUntil > DateTime.UtcNow)
        return new UserBannedError(user.BannedUntil.Value);

    return user;
}

app.MapGet("/users/{id}", (Guid id) =>
    GetUser(id)
        .MapAsync(u => new UserDto(u.Id, u.Name))
        .ToHttpResultAsync());
```

A missing user gives `404` and a banned one `403`, both as `application/problem+json` with the error code.

## Build

```bash
dotnet build
dotnet test
dotnet run -c Release --project Tsikhanau.Railway.Benchmarks -- --filter '*'
```

Changes are listed in [CHANGELOG.md](CHANGELOG.md).

## License

[MIT](LICENSE)
