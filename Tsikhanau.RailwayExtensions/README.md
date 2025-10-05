# Tsikhanau.RailwayExtensions

Railway-oriented programming extensions for Result type - Bind, Map, Combine, Tap, and async operations.

## Features

- **Bind** - Chain operations that return Result
- **Map** - Transform success values
- **MapError** - Transform error values
- **Ensure** - Conditional validation
- **Tap/TapError** - Side effects
- **Combine** - Aggregate multiple results
- **Full async support** - All operations have async variants

## Installation

```bash
dotnet add package Tsikhanau.RailwayExtensions
```

## Usage

```csharp
using Tsikhanau.RailwayExtensions.Result;

var result = await GetUser(123)
    .Ensure(u => u.IsActive, Error.Validation("User inactive"))
    .Bind(u => GetUserProfile(u.Id))
    .Map(profile => profile.DisplayName)
    .Tap(name => logger.LogInfo($"Retrieved: {name}"))
    .MapError(err => err with { Code = "USER_ERROR" });

return result.Match(
    onSuccess: name => Ok(name),
    onFailure: error => BadRequest(error)
);
```

## License

MIT
