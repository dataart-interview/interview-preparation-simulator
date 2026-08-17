# Reference Solution Simplification Design

## Goal

Keep the Cinema Seat Service reference solution slightly beyond what a strong candidate might complete in 90 minutes, while removing production ceremony that obscures the exercise's core lessons.

The solution will continue to demonstrate controllers, separation of API/domain/infrastructure concerns, a typed HTTP client, explicit result-based failures, resilience, a last-known-good cache, cancellation, and focused tests.

## Architecture

Retain the four existing projects and their dependency directions. Retain `FluentResults` and explicit mapper classes.

Remove `Snapshot<T>`. Cache age and staleness are infrastructure details and will no longer flow through domain-service methods or public response contracts. The provider and service interfaces will return:

- `Result<SeatMap>`
- `Result<Seat>`
- `Result<AdjacentSeatBlock?>`

The cache provider may serve a last-known-good `SeatMap`, but callers receive the same domain value whether it was fetched or cached.

## API Contracts

The seat-map and seat-availability responses will match the exercise examples and will not expose `asOfUtc` or `isStale`.

The adjacent-seat response will retain `Found`, `Row`, `StartNumber`, and `EndNumber`. The range fields will be `null` when no block exists and will use normal JSON serialization without per-property `JsonIgnore` attributes.

## Validation

Validate request values once at the API boundary:

- seat numbers must be positive;
- `minSeats` must be at least two.

Remove the single-ASCII-letter rule and both copies of `IsRow`. A row is a non-empty upstream key and lookup is case-insensitive.

Keep focused feed validation: exactly one seat map, required auditorium and film title, a valid Unix start time, at least one non-empty row, and seat strings containing only `0` and `1`.

## Errors and HTTP Mapping

Use standard `ProblemDetails` without a custom machine-readable error-code extension. HTTP status is the stable public failure signal for this exercise.

Keep a small set of typed errors for internal control flow:

- `SeatNotFoundError` maps to `404`;
- invalid JSON or contract-invalid feed content maps to `502`;
- timeout, network failure, transient upstream response, or unavailable data with no usable fallback maps to `503`.

ASP.NET Core's `[ApiController]` behavior produces `400` validation responses. `UseExceptionHandler` handles unexpected defects as `500` responses.

Remove `CinemaError.Code` and `InvalidSeatQueryError`. Put the repeated result-to-ProblemDetails translation in one small API-layer helper so controllers remain focused on orchestration.

## Cache and Resilience

Replace the unused FusionCache dependency and misleading `FusionSeatMapProvider` name with a small application-owned cache provider.

The provider stores the last successful map and fetch timestamp. It returns fresh cached data for five seconds. After that it attempts the upstream call and, if the call fails, may return the last-known-good map for a further 30 seconds. An older cached value is discarded as a fallback.

Keep `TimeProvider` injection so cache behavior is deterministic in tests. Define the five-second fresh duration and 30-second fallback window next to the provider as fixed exercise defaults; do not bind a cache options class.

Keep the typed `HttpClient`. Use the standard resilience handler, adjusting it to a four-second total timeout, one-second attempt timeout, and two retries while retaining its circuit breaker. A final upstream `404` is treated as unavailable so the cache can provide the fallback; it does not require a special retry predicate. The feed client maps final transport/HTTP outcomes to typed errors and never leaks response bodies or exception details.

## Configuration

Follow the exercise instruction to keep the known feed URL in code. Remove configuration classes that exist only to hold fixed exercise values. Keep adjustable configuration only where it materially improves the example; do not add validation for impossible states created solely by this repository's own constants/defaults.

## Tests

Retain focused unit tests for:

- valid feed expansion and malformed feed rejection;
- case-insensitive seat lookup and missing seats;
- adjacent-seat search without crossing rows;
- fresh cache reuse, last-known-good fallback, and expired fallback failure.

Replace the health-only integration test with a small public-contract smoke test using an overridden provider, while retaining a simple health check if it remains inexpensive. Do not add exhaustive validation, controller, concurrency, or failure-matrix tests.

## Expected Simplification

The refactor removes cross-layer cache wrappers, response freshness fields, per-property serialization attributes, duplicated row validation, unused cache machinery, custom error codes, and excessive options validation. It preserves the architectural and resilience choices that are useful for interview discussion.
