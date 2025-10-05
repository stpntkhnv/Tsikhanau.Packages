# Tsikhanau.Guards

A comprehensive guard clause library for .NET that provides robust parameter validation with clear, expressive syntax.

## Features

- **Comprehensive Guards** - Null checks, range validation, string validation
- **Expressive API** - Clear, readable guard clauses
- **Exception Messages** - Detailed error information
- **Performance** - Optimized for minimal overhead

## Installation

```bash
dotnet add package Tsikhanau.Guards
```

## Usage

```csharp
using Tsikhanau.Guards;

public class UserService
{
    public void CreateUser(string name, int age)
    {
        Guard.Against.Null(name, nameof(name));
        Guard.Against.NullOrEmpty(name, nameof(name));
        Guard.Against.NegativeOrZero(age, nameof(age));
    }
}
```

## License

MIT
