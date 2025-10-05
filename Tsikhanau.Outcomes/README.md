# Tsikhanau.Outcomes

Result and Error types for railway-oriented programming in .NET - functional error handling without exceptions.

## Features

- **Result&lt;TData, TError&gt;** - Encapsulates success or failure
- **Error** - Rich error type with codes and messages
- **Implicit conversions** - Natural syntax

## Installation

```bash
dotnet add package Tsikhanau.Outcomes
```

## Usage

```csharp
using Tsikhanau.Outcomes.Result;
using Tsikhanau.Outcomes.Errors;

public Result<User, Error> GetUser(int id)
{
    if (id <= 0)
        return Error.Validation("Invalid user ID");
    
    var user = database.Find(id);
    return user ?? Error.NotFound("User not found");
}

// Pattern matching
var result = GetUser(123);
result.Match(
    onSuccess: user => Console.WriteLine(user.Name),
    onFailure: error => Console.WriteLine(error.Message)
);
```

## License

MIT
