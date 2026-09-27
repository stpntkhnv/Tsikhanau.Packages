# Tsikhanau.Railway

Result and Optional types for railway-oriented programming in .NET: functional error handling without exceptions.

## Features

- **Result&lt;T&gt;** - success with a value or failure with an `Error`
- **Optional&lt;T&gt;** - a value or nothing
- **Error** - record with `Kind` (Failure, Unexpected, Validation, NotFound, Conflict, Unauthorized, Forbidden), `Code`, `Message`, `Inner` and `Metadata`; derive your own errors from it
- **ValidationError** - field errors, merged automatically by `CombineAll`
- **Bind**, **Map**, **MapError** - chain and transform
- **Ensure** - conditional checks with a fixed error or an error built from the value
- **Tap**, **TapError** - side effects
- **Result.Combine**, **Result.CombineAll**, **Result.FirstSuccess**, **Zip** - work with several results
- **Result.Try** - turn exceptions into errors
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

var message = await GetUser(id)
    .EnsureAsync(u => u.IsActive, u => Error.Forbidden("user.inactive", $"{u.Name} is inactive"))
    .MapAsync(u => u.DisplayName)
    .TapAsync(name => logger.LogInformation("Retrieved: {Name}", name))
    .MatchAsync(
        onSuccess: name => $"Hello, {name}",
        onFailure: error => error switch
        {
            UserBannedError banned => $"Come back after {banned.Until:d}",
            _ => error.Message
        });

var validated = Result.CombineAll(
    ValidateEmail(request.Email),
    ValidateName(request.Name));
```

## License

MIT
