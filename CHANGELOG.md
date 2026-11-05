# Changelog

## 1.2.0 - Unreleased

### Tsikhanau.Railway

- `AsOptional()` turns a nullable value into an `Optional<T>`, for value types and reference types alike. `AsNullable()` turns an `Optional<T>` of a value type back into `T?`, and `AsEnumerable()` reads an `Optional<T>` as a sequence of zero or one value.

### Tsikhanau.Railway.AspNetCore

- `error.ToProblem<TSuccess>()` gives the same answer as `ToHttpResult`, typed as `Results<TSuccess, ValidationProblem, ProblemHttpResult>` for Minimal API endpoints that declare their results.
- `ErrorHttpExtensions.ExpectedKinds` lists the kinds that answer with their own status, and `error.IsExpected()` says whether an error answers that way or as a bare 500.
- `ProblemMembers` names the body members of the answers: `code`, `errors` and `traceId`.
- Changed: every error of kind `Validation` now answers `ValidationProblem`, not only a `ValidationError`. The answer is a 400 with the title "One or more validation errors occurred.", the messages in `errors` and no `detail`. The error's own message goes under the `""` key, except for an `AggregateError`, which writes only what its members give; the field errors of a `ValidationError` reached through `Inner` or an `AggregateError` go under their fields, and the message of every other expected member of an `AggregateError` goes under `""` too. Before, such an error answered a 400 problem with the title "Bad Request" and its message in `detail`. The change reaches every method that turns a failure into a response:
  - `Error.ToHttpResult()`
  - `Result<T>.ToHttpResult()` and `Result<T>.ToHttpResult(onSuccess)`
  - `Result<Unit>.ToHttpResult()`
  - `Task<Result<T>>.ToHttpResultAsync()` and `Task<Result<T>>.ToHttpResultAsync(onSuccess)`
  - `Task<Result<Unit>>.ToHttpResultAsync()`
- The message of an unexpected error (kind `Unexpected`, or any kind outside `ExpectedKinds`) found through `Inner` or an `AggregateError` is never written to `errors`; the field errors it wraps still are.
- `ToHttpResult` still returns the concrete `ValidationProblem` or `ProblemHttpResult`, as before.

## 1.1.0 - Unreleased

### Tsikhanau.Railway

- `Result.Collect(...).And(...)` collects up to eight results of different types; `Map` and `Bind` get every value by name, or one error with all failures merged the way `CombineAll` merges them.

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
