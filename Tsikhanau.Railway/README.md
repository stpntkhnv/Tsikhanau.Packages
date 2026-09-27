# Tsikhanau.Railway

Result and Optional types for railway-oriented programming in .NET: functional error handling without exceptions.

## Features

- **Result&lt;TData, TError&gt;** - success or failure
- **Optional&lt;T&gt;** - a value or nothing
- **Error**, **ValidationError** - errors with codes, messages and inner errors
- **Unit** - "no value" for operations that return nothing
- **Bind**, **Map**, **MapError** - chain and transform
- **Ensure** - conditional checks
- **Tap**, **TapError**, **OnSuccess**, **OnFailure** - side effects
- **Combine**, **CombineAll**, **Zip**, **FirstSuccess** - work with several results
- **Try** - turn exceptions into errors
- **Match**, **GetValueOrDefault** - get the value out
- **Where**, **Or**, **OrElse** - filter and fallback for Optional
- **ToResult**, **ToOptional** - conversions between Result and Optional
- Async variants for all operations

## Installation

```bash
dotnet add package Tsikhanau.Railway
```

## Usage

```csharp
using Tsikhanau.Railway;

public Result<User, Error> GetUser(int id)
{
    if (id <= 0)
        return Error.Validation("Invalid user ID");

    var user = database.Find(id);
    if (user is null)
        return Error.Create("NOT_FOUND", "User not found");

    return user;
}

var result = GetUser(123)
    .Ensure(u => u.IsActive, Error.Validation("User inactive"))
    .Map(u => u.DisplayName)
    .Tap(name => logger.LogInformation("Retrieved: {Name}", name))
    .MapError(err => err.WithContext("USER_ERROR", "Failed to load user"));

var message = result.Match(
    onSuccess: name => $"Hello, {name}",
    onFailure: error => error.GetFullMessage());
```

## License

MIT
