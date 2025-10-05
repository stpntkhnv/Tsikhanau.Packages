# Tsikhanau.ValueObjects

Validated value objects for domain modeling - NotEmptyString, RequiredGuid, and more.

## Features

- **NotEmptyString** - String that cannot be null or empty
- **RequiredGuid** - Guid that cannot be empty
- **Validation** - Built-in validation on construction
- **Immutability** - Read-only value objects

## Installation

```bash
dotnet add package Tsikhanau.ValueObjects
```

## Usage

```csharp
using Tsikhanau.Packages.ValueObjects;

// NotEmptyString ensures non-empty strings
var result = NotEmptyString.Create("test");
if (result.IsSuccess)
{
    NotEmptyString name = result.Value;
    Console.WriteLine(name.Value);
}

// RequiredGuid ensures non-empty GUIDs
var guidResult = RequiredGuid.Create(Guid.NewGuid());
```

## License

MIT
