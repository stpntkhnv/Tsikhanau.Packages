# Tsikhanau.Foundation

Core building blocks for functional programming in .NET.

## Features

- **Guard** - Parameter validation utilities
- **Ensure** - Condition assertion helpers  
- **Unit** - Represents void in functional contexts
- **Clock Abstraction** - Testable time operations

## Installation

```bash
dotnet add package Tsikhanau.Foundation
```

## Usage

```csharp
using Tsikhanau.Foundation.Validation;
using Tsikhanau.Foundation.General;

// Guard against null
Guard.AgainstNull(value, nameof(value));

// Use Unit for void operations
public Result<Unit, Error> DoSomething()
{
    // ... perform operation
    return Unit.Value;
}
```

## License

MIT
