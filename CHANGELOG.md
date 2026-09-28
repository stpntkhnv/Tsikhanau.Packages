# Changelog

## 1.0.0 - 2026-09-28

First release of Tsikhanau.Railway and Tsikhanau.Railway.AspNetCore. They replace Tsikhanau.Foundation, Tsikhanau.RailwayExtensions, Tsikhanau.Outcomes, Tsikhanau.Validation and Tsikhanau.Flow, which are deprecated.

### Tsikhanau.Railway

- `Result<T>`: success with a value or failure with an `Error`.
- `Error` record with `Kind` (Failure, Unexpected, Validation, NotFound, Conflict, Unauthorized, Forbidden), `Code`, `Message`, `Inner` and `Metadata`. Custom errors are records derived from `Error`.
- `ValidationError` with field errors and `AggregateError` for several errors.
- `Optional<T>` and `Unit`.
- Extensions: `Bind`, `Map`, `MapError`, `Ensure`, `Tap`, `TapError`, `Match`, `GetValueOrDefault`, `Zip`, with async variants over `Task`.
- `Result.Combine`, `Result.CombineAll`, `Result.FirstSuccess`, `Result.Try`.
- Roslyn analyzers: TR0001 ignored Result, TR0002 Map whose mapper returns a Result (with a code fix to Bind), TR0003 `Value`/`Error` read without a check.

### Tsikhanau.Railway.AspNetCore

- `ToHttpResult` / `ToHttpResultAsync` for `Result<T>` and `Error`: ProblemDetails responses with status codes by `ErrorKind`, for Minimal APIs and controllers.

### Migrating from the old packages

- Everything lives in the single namespace `Tsikhanau.Railway`.
- `Result<TData, TError>` is now `Result<T>`; the error is always `Error`, and custom errors derive from it.
- `Either`, `IClock` and the Validation and Flow libraries are removed.
