# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Structure

This is a .NET 10.0 solution with a single C# library, Tsikhanau.Railway (NuGet package of the same name), for railway-oriented programming:

- `Tsikhanau.Railway/Core/`: Result, Optional, Error, ValidationError, Unit, Empty
- `Tsikhanau.Railway/Extensions/`: extensions for Result and Optional (Bind, Map, Tap, Match, ...) and conversions between them
- `Tsikhanau.Railway/Internal/`: Guard (internal, argument checks)

All types live in the single namespace `Tsikhanau.Railway`.

## Build Commands

```bash
# Build entire solution
dotnet build

# Pack the NuGet package into ./nugets
dotnet pack Tsikhanau.Railway/Tsikhanau.Railway.csproj -c Release -o nugets

# Pack and publish to NuGet.org (needs NUGET_API_KEY)
./publish-nugets.sh
```

## Architecture Patterns

### Result Type Pattern
The codebase uses `Result<TData, Error>` for error handling instead of exceptions. All operations return Result types that can be Success or Failure.

### Railway-Oriented Programming
Fluent extension methods for chaining operations on Result and Optional:
- `Bind()` - Chain operations that return the monadic type
- `Map()` - Transform inner values
- `Tap()` - Side effects without changing the value
- `Match()` - Pattern match on success/failure states
- `Where()` - Filter Optional values
- `Or()` / `OrElse()` - Fallback for Optional
- `Combine()` - Combine multiple Results
- Cross-type conversions: `ToResult()`, `ToOptional()`

## Development Guidelines

- Target framework is .NET 10.0 with nullable reference types enabled and TreatWarningsAsErrors
- Use `Result<T, Error>` instead of throwing exceptions
- Guard against null values using the internal `Guard.AgainstNull()`
- Follow existing naming conventions (PascalCase for public members)
- Use `Unit` type for operations that don't return meaningful data
- Sync extension methods use `[MethodImpl(MethodImplOptions.AggressiveInlining)]`
