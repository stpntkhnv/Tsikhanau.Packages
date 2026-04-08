# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Structure

This is a .NET 9.0 solution containing C# libraries that implement functional programming patterns:

- **Tsikhanau.Foundation**: Base utilities including validation (Guard, Ensure), clock abstraction, and Unit type
- **Tsikhanau.Monads**: Monadic types (Result, Optional, Either) and Error types for railway-oriented programming
- **Tsikhanau.RailwayExtensions**: Extension methods for Result, Optional, Either (Bind, Map, Tap, Match, etc.) + cross-type conversions
- **Tsikhanau.Validation**: Fluent validation framework with builders, templates, sync and async support

## Build Commands

```bash
# Build entire solution
dotnet build

# Build specific project
dotnet build Tsikhanau.Foundation/Tsikhanau.Foundation.csproj
```

## Architecture Patterns

### Result Type Pattern
The codebase uses `Result<TData, Error>` for error handling instead of exceptions. All operations return Result types that can be Success or Failure.

### Railway-Oriented Programming
The RailwayExtensions library provides fluent methods for chaining operations on Result, Optional, and Either:
- `Bind()` - Chain operations that return the monadic type
- `Map()` - Transform inner values
- `Tap()` - Side effects without changing the value
- `Match()` - Pattern match on success/failure states
- `Where()` - Filter Optional values
- `Or()` / `OrElse()` - Fallback for Optional
- `Combine()` - Combine multiple Results
- Cross-type conversions: `ToResult()`, `ToOptional()`, `ToEither()`

### Validation
Fluent builder DSL for validation with `Must()` / `MustAsync()` rules. Returns `Result<T, Error>` for seamless integration with railway chains.

## Development Guidelines

- Target framework is .NET 9.0 with nullable reference types enabled
- Use `Result<T, Error>` instead of throwing exceptions
- Guard against null values using `Guard.AgainstNull()`
- Follow existing naming conventions (PascalCase for public members)
- Use `Unit` type for operations that don't return meaningful data
- Sync extension methods use `[MethodImpl(MethodImplOptions.AggressiveInlining)]`
