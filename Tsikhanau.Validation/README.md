# Tsikhanau.Validation

Fluent validation framework with builders and templates for functional validation patterns.

## Features

- **Fluent API** - Chainable validation builders
- **Validation Templates** - Reusable validation rules
- **Field Validators** - Type-safe field validation
- **Result integration** - Works seamlessly with Result type

## Installation

```bash
dotnet add package Tsikhanau.Validation
```

## Usage

```csharp
using Tsikhanau.Validation;

var validator = new ValidatorBuilder<User>()
    .AddField(u => u.Email)
        .NotEmpty()
        .Email()
    .AddField(u => u.Age)
        .GreaterThan(0)
        .LessThan(120)
    .Build();

var result = validator.Validate(user);
```

## License

MIT
