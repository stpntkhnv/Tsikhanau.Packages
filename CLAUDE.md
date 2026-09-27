# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Structure

This is a .NET 10.0 solution (Tsikhanau.Packages.slnx) around a single C# library, Tsikhanau.Railway (NuGet package of the same name), for railway-oriented programming:

- `Tsikhanau.Railway/Core/`: Result, Optional, Error, ValidationError, Unit, Empty
- `Tsikhanau.Railway/Extensions/`: extensions for Result and Optional (Bind, Map, Tap, Match, ...) and conversions between them
- `Tsikhanau.Railway/Internal/`: Guard (internal, argument checks)
- `Tsikhanau.Railway.Tests/`: xUnit v3 (Microsoft Testing Platform), Shouldly, NSubstitute; folders mirror the library
- `Tsikhanau.Railway.Benchmarks/`: BenchmarkDotNet with MemoryDiagnoser, one plain-code baseline per class

All library types live in the single namespace `Tsikhanau.Railway`.

Shared build settings are in `Directory.Build.props`; package versions are managed centrally in `Directory.Packages.props` (PackageReference without Version).

## Build Commands

```bash
dotnet build
dotnet test
dotnet test --coverage --coverage-output-format cobertura
dotnet run -c Release --project Tsikhanau.Railway.Benchmarks -- --filter '*'
dotnet pack Tsikhanau.Railway/Tsikhanau.Railway.csproj -c Release -o nugets
```

## Public API and Release

- The public API is tracked by PublicApiAnalyzers in `Tsikhanau.Railway/PublicAPI.Shipped.txt` and `PublicAPI.Unshipped.txt`. A new or changed public member fails the build until the files are updated. Missing entries (RS0016) are added by `dotnet format analyzers Tsikhanau.Railway/Tsikhanau.Railway.csproj --diagnostics RS0016 --severity info`; stale entries (RS0017) are removed by hand.
- RS0027 (optional parameter overloads) is suppressed in the library csproj until the GetValueOrDefault API is reworked.
- On release, move the Unshipped entries into Shipped.
- CI (`.github/workflows/ci.yml`) builds and tests every push and PR. A tag `v<Version>` matching `<Version>` in the library csproj packs and publishes to nuget.org through NuGet trusted publishing (no API key in the repo).

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
