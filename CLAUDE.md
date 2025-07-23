# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Structure

This is a .NET 9.0 solution containing multiple C# libraries that implement functional programming patterns:

- **Tsikhanau.Outcomes**: Core Result and Error types for railway-oriented programming
- **Tsikhanau.Foundation**: Base utilities including validation (Guard, Ensure), clock abstraction, and Unit type
- **Tsikhanau.RailwayExtensions**: Extension methods for Result type (Bind, Map, Combine, etc.)
- **Tsikhanau.Flow**: Workflow execution engine with step-by-step processing and error handling
- **Tsikhanau.Validation**: Field validation framework with builders and templates
- **Tsikhanau.Packages.ValueObjects**: Value object implementations (NotEmptyString, RequiredGuid, etc.)
- **TestConsole**: Console application for testing the libraries

## Build Commands

```bash
# Build entire solution
dotnet build

# Build specific project
dotnet build Tsikhanau.Foundation/Tsikhanau.Foundation.csproj

# Run the test console
dotnet run --project TestConsole
```

## Architecture Patterns

### Result Type Pattern
The codebase heavily uses `Result<TData, TError>` for error handling instead of exceptions. All operations return Result types that can be Success or Failure.

### Railway-Oriented Programming
The RailwayExtensions library provides fluent methods for chaining operations:
- `Bind()` - Chain operations that return Result
- `Map()` - Transform success values
- `Combine()` - Combine multiple Results
- `Tap()` - Side effects on success
- `TapError()` - Side effects on failure

### Flow Pattern
The Flow library implements a workflow execution pattern where complex operations are broken into sequential steps. Each step can access a shared FlowContext and the flow tracks execution metadata.

### Value Objects
Value objects are implemented with validation and immutability. They use the Foundation validation utilities.

## Development Guidelines

- Target framework is .NET 9.0 with nullable reference types enabled
- Use `Result<T, Error>` instead of throwing exceptions
- Guard against null values using `Guard.AgainstNull()`
- Follow existing naming conventions (PascalCase for public members)
- Value objects should be readonly structs with validation
- Use `Unit` type for operations that don't return meaningful data