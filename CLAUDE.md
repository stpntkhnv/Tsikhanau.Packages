# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Structure

This is a .NET 10.0 solution (Tsikhanau.Packages.slnx) with two NuGet packages for railway-oriented programming: the core library Tsikhanau.Railway (with Roslyn analyzers packed inside) and its ASP.NET Core integration Tsikhanau.Railway.AspNetCore.

- `Tsikhanau.Railway/Core/`: Result<T>, Optional<T>, Error (record) with ErrorKind, ValidationError, FieldError, AggregateError, Unit
- `Tsikhanau.Railway/Extensions/`: extensions for Result and Optional (Bind, Map, Tap, Match, ...), conversions between them, and the static `Result` aggregation/Try helpers (Combine, CombineAll, FirstSuccess, Try)
- `Tsikhanau.Railway/Internal/`: Guard (internal, argument checks)
- `Tsikhanau.Railway.AspNetCore/`: `ToHttpResult` / `ToHttpResultAsync` for Result<T> and Error (ProblemDetails, ErrorKind to status code); works in Minimal APIs and controllers
- `Tsikhanau.Railway.Analyzers/`: Roslyn analyzers (netstandard2.0, Microsoft.CodeAnalysis.CSharp 5.0.0): TR0001 ignored Result, TR0002 Map whose mapper returns a Result, TR0003 Value/Error read without an IsSuccess/IsFailure check (flow analysis over the control flow graph in `ResultFlowAnalysis`)
- `Tsikhanau.Railway.Analyzers.CodeFixes/`: code fix TR0002 Map -> Bind (separate assembly because it needs Workspaces)
- `Tsikhanau.Railway.Tests/`: xUnit v3 (Microsoft Testing Platform), Shouldly, NSubstitute; folders mirror the library
- `Tsikhanau.Railway.AspNetCore.Tests/`: end-to-end tests through TestServer, including an MVC controller
- `Tsikhanau.Railway.Analyzers.Tests/`: Microsoft.CodeAnalysis.Testing with `{|TR0001:...|}` markup; net10.0 reference assemblies are downloaded from nuget.org on the first run
- `Tsikhanau.Railway.Benchmarks/`: BenchmarkDotNet with MemoryDiagnoser, one plain-code baseline per class

All public types of both packages live in the single namespace `Tsikhanau.Railway` (analyzer types live in `Tsikhanau.Railway.Analyzers` and are not part of the library API).

Both analyzer DLLs are packed into Tsikhanau.Railway under `analyzers/dotnet/cs`. The core and AspNetCore projects run the analyzers on their own code. AspNetCore references the core with `PrivateAssets="none"` so the analyzers reach users who install only Tsikhanau.Railway.AspNetCore.

Shared build settings and package metadata (including the common `<Version>` of both packages) are in `Directory.Build.props`; package versions are managed centrally in `Directory.Packages.props` (PackageReference without Version).

## Build Commands

```bash
dotnet build
dotnet test
dotnet test --coverage --coverage-output-format cobertura
dotnet run -c Release --project Tsikhanau.Railway.Benchmarks -- --filter '*'
dotnet pack -c Release -o nugets
```

## Public API and Release

- The public API of each package is tracked by PublicApiAnalyzers in its `PublicAPI.Shipped.txt` and `PublicAPI.Unshipped.txt`. A new or changed public member fails the build until the files are updated. Missing entries (RS0016) are added by `dotnet format analyzers <project>.csproj --diagnostics RS0016 --severity info`; stale entries (RS0017) are removed by hand.
- Analyzer rules are tracked in `Tsikhanau.Railway.Analyzers/AnalyzerReleases.Unshipped.md` / `AnalyzerReleases.Shipped.md` (RS2008); a new or changed rule needs an entry there.
- On release, move the Unshipped entries (public API and analyzer rules) into Shipped.
- CI (`.github/workflows/ci.yml`) builds and tests every push and PR. A tag `v<Version>` matching `<Version>` in Directory.Build.props packs both packages and publishes them to nuget.org through NuGet trusted publishing (no API key in the repo).

## Architecture Patterns

### Result Type Pattern
Operations return `Result<T>` instead of throwing. The error is always `Error`; custom errors are records derived from `Error` and are recognized with pattern matching. `Error.Kind` says what kind of failure it is (NotFound, Validation, Conflict, ...), which Tsikhanau.Railway.AspNetCore maps to HTTP status codes.

### Railway-Oriented Programming
Fluent extension methods for chaining operations on Result and Optional:
- `Bind()` - Chain operations that return the monadic type
- `Map()` / `MapError()` - Transform the value or the error
- `Ensure()` - Turn a failed check into an error
- `Tap()` / `TapError()` - Side effects without changing the value
- `Match()` - Pattern match on success/failure states
- `Where()` - Filter Optional values
- `Or()` / `OrElse()` - Fallback for Optional
- `Result.Combine()` (first error wins) / `Result.CombineAll()` (all errors merged)
- Cross-type conversions: `ToResult()`, `ToOptional()`

Each operation has async variants over `Task`: Task source with a sync func, Task source with an async func, and a plain source with an async func.

## Development Guidelines

- Target framework is .NET 10.0 with nullable reference types enabled and TreatWarningsAsErrors
- Use `Result<T>` instead of throwing exceptions
- Guard against null values using the internal `Guard.AgainstNull()`
- Follow existing naming conventions (PascalCase for public members)
- Use `Unit` type for operations that don't return meaningful data
- Sync extension methods use `[MethodImpl(MethodImplOptions.AggressiveInlining)]`
