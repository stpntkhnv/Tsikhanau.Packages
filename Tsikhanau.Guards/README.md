# Tsikhanau.Guards

A comprehensive guard clause library for .NET that provides robust parameter validation with clear, expressive syntax. Guard clauses help enforce method contracts by validating arguments and throwing appropriate exceptions when invalid values are provided.

## Features

- **Comprehensive Coverage**: Guards for nulls, strings, collections, numbers, enums, types, dates, and more
- **Performance Optimized**: Uses `AggressiveInlining` for minimal runtime overhead
- **Caller Expression Support**: Automatically captures parameter names for better error messages
- **Fluent API**: Clean, readable syntax that integrates naturally with your code
- **Custom Exceptions**: Support for custom exception factories
- **Modern .NET**: Built for .NET 9 with nullable reference types and latest C# features

## Installation

```bash
dotnet add package Tsikhanau.Guards
```

## Usage

### Basic Null Guards

```csharp
using Tsikhanau.Guards;

public void ProcessUser(User user)
{
    Guard.AgainstNull(user); // Throws ArgumentNullException if user is null
    
    // Continue with processing...
}
```

### String Guards

```csharp
public void SetUsername(string username)
{
    Guard.AgainstNullOrWhiteSpace(username);
    Guard.AgainstTooLong(username, 50);
    Guard.AgainstTooShort(username, 3);
    
    // Validate format
    Guard.AgainstInvalidFormat(username, @"^[a-zA-Z0-9_]+$");
}

public void SetEmail(string email)
{
    Guard.AgainstNullOrEmpty(email);
    Guard.AgainstInvalidFormat(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
}
```

### Numeric Guards

```csharp
public void SetAge(int age)
{
    Guard.AgainstNegative(age);
    Guard.AgainstOutOfRange(age, 0, 150);
}

public void SetPrice(decimal price)
{
    Guard.AgainstNegativeOrZero(price);
    Guard.AgainstTooLarge(price, 1000000m);
}

public void Divide(int dividend, int divisor)
{
    Guard.AgainstZero(divisor); // Prevent division by zero
    return dividend / divisor;
}
```

### Collection Guards

```csharp
public void ProcessItems(IList<string> items)
{
    Guard.AgainstNullOrEmpty(items);
    Guard.AgainstTooManyItems(items, 1000);
    Guard.AgainstTooFewItems(items, 1);
}
```

### Enum Guards

```csharp
public void SetStatus(OrderStatus status)
{
    Guard.AgainstUndefinedEnum(status); // Ensures enum value is defined
}
```

### Date/Time Guards

```csharp
public void ScheduleEvent(DateTime eventDate)
{
    Guard.AgainstPastDate(eventDate); // Must be in future
}

public void RecordTransaction(DateTime transactionDate)
{
    Guard.AgainstFutureDate(transactionDate); // Cannot be in future
    Guard.AgainstDateOutOfRange(transactionDate, 
        DateTime.Now.AddYears(-10), 
        DateTime.Now);
}
```

### Conditional Guards

```csharp
public void ProcessPayment(decimal amount, bool isVipCustomer)
{
    Guard.AgainstNegativeOrZero(amount);
    
    // Custom condition validation
    Guard.Against(amount > 10000 && !isVipCustomer, 
        "Large payments require VIP status");
    
    // Ensure condition is true
    Guard.Ensure(amount <= GetDailyLimit(), 
        "Payment exceeds daily limit");
}
```

### Custom Exception Factories

```csharp
public void ProcessOrder(Order order)
{
    Guard.Against(order.Items.Count == 0, 
        () => new BusinessException("Order must contain at least one item"));
    
    Guard.Ensure(order.Total > 0, 
        () => new InvalidOperationException("Order total must be positive"));
}
```

### Type Guards

```csharp
public void ProcessDocument(object document)
{
    var pdfDoc = Guard.AgainstInvalidType<PdfDocument>(document);
    // Now safely use pdfDoc as PdfDocument
}
```

### Guid Guards

```csharp
public void FindUser(Guid userId)
{
    Guard.AgainstEmpty(userId); // Ensures Guid is not Guid.Empty
}
```

### File System Guards

```csharp
public void LoadConfiguration(string configPath)
{
    Guard.AgainstInvalidFilePath(configPath); // Validates path and file existence
}

public void ProcessDirectory(string directoryPath)
{
    Guard.AgainstInvalidDirectoryPath(directoryPath); // Validates path and directory existence
}
```

## Error Messages

The Guard class automatically captures parameter names and provides clear, descriptive error messages:

```csharp
// This code:
Guard.AgainstNull(user);

// Produces this error message:
// "Parameter 'user' cannot be null."

// With range validation:
Guard.AgainstOutOfRange(age, 0, 150);
// "Parameter 'age' must be between 0 and 150 (inclusive)."
```

## Performance

All guard methods are marked with `[MethodImpl(MethodImplOptions.AggressiveInlining)]` to ensure minimal performance overhead. The guards are designed to be used liberally throughout your codebase without impacting performance.

## Best Practices

1. **Use guards at public API boundaries**: Always validate public method parameters
2. **Place guards at the beginning of methods**: Fail fast with clear error messages
3. **Be specific with validation**: Use the most appropriate guard for your scenario
4. **Combine guards when needed**: Multiple guards can be chained for comprehensive validation

```csharp
public void CreateUser(string username, string email, int age)
{
    // Validate all parameters upfront
    Guard.AgainstNullOrWhiteSpace(username);
    Guard.AgainstTooLong(username, 50);
    Guard.AgainstInvalidFormat(username, @"^[a-zA-Z0-9_]+$");
    
    Guard.AgainstNullOrEmpty(email);
    Guard.AgainstInvalidFormat(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    
    Guard.AgainstNegative(age);
    Guard.AgainstOutOfRange(age, 13, 120);
    
    // Now safely proceed with user creation
}
```

## Integration with Other Tsikhanau Packages

This library works seamlessly with other packages in the Tsikhanau ecosystem:

- **Tsikhanau.Outcomes**: Use guards to validate inputs before creating Result/Optional types
- **Tsikhanau.Validation**: Combine with validation rules for comprehensive input validation
- **Tsikhanau.Flow**: Use guards in flow steps for robust pipeline validation

## License

MIT License - see LICENSE file for details.
