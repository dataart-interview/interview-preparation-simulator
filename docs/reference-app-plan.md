# Cinema Seat Service Reference App: Technical Decisions

## Intent

This document captures the technical decisions behind `reference-solution`. It is not a task-by-task implementation plan and not a production checklist.

The goal is a complete reference for the interview exercise that remains easy to explain. It is intentionally a little beyond a strong 90-minute implementation: the core flow is small, while resilience, last-known-good caching, OpenAPI, and Docker create useful discussion points.

The current application is the source of truth. The decisions below explain why it has this shape, which compromises are intentional, and which parts should be deferred during live coding.

## Requirements interpreted

The service reads a flaky external cinema feed and exposes:

| Method | Route | Meaning |
|---|---|---|
| `GET` | `/api/v1/seat-map` | Return the complete map as individual seats |
| `GET` | `/api/v1/seats/{row}/{number}` | Return whether one seat is available |
| `GET` | `/api/v1/adjacent-seats?minSeats=2` | Find the first qualifying available block |
| `GET` | `/health/live` | Confirm that the process is running |

The important non-functional concerns are:

- the upstream may delay, time out, or return an error;
- the known JSON contains a trailing comma;
- callers need stable HTTP semantics rather than leaked infrastructure exceptions;
- cancellation should flow from the request to the upstream call;
- the solution should demonstrate clear boundaries without looking production-generated.

## Solution shape

### Project boundaries

Use four projects with inward dependencies:

```text
Cinema.Host ──> Cinema.Api ──> Cinema.Domain
      │                            ▲
      └────> Cinema.Infrastructure ┘
```

| Project | Responsibility |
|---|---|
| `Cinema.Host` | Composition root, middleware, health, OpenAPI, Scalar |
| `Cinema.Api` | Controllers, public contracts, HTTP result mapping |
| `Cinema.Domain` | Models, expected errors, seat lookup and adjacency logic |
| `Cinema.Infrastructure` | Upstream DTO, JSON mapping, HTTP client, resilience, cache |

This is more separation than a minimal live-coding solution needs, but it makes the reference app's dependency direction and integration boundaries explicit. Domain has no ASP.NET Core, JSON, HTTP, or caching dependency.

Do not add an application project just to hold one service. `SeatMapService` is small enough to remain in Domain for this exercise.

### Data boundaries

Keep three different representations:

```text
upstream JSON DTO -> domain model -> public API response
```

- The feed DTO mirrors the external JSON contract.
- Domain records use meaningful types such as `SeatStatus` and `DateTimeOffset`.
- API records control the public JSON shape independently of the feed.

Use small handwritten mappers. AutoMapper or Mapster would hide the most relevant transformation in this exercise and add setup without reducing meaningful code.

## API decisions

### Controllers over Minimal APIs

Use controllers because the exercise naturally benefits from route/query validation, `ProducesResponseType`, and standard `ProblemDetails`. Minimal APIs would also work, but they would not materially simplify three endpoints once validation and response metadata are included.

Controllers remain orchestration-only:

1. accept validated input and a cancellation token;
2. call `ISeatMapService`;
3. map a successful domain value;
4. translate typed errors to an HTTP problem.

No controller contains feed parsing, caching, adjacency logic, or HTTP-client behavior.

### Public contracts

Return unwrapped success payloads. Do not add `Result`, `Snapshot<T>`, `data`, or metadata envelopes to successful responses.

- Seat-map responses contain auditorium, film title, UTC start time formatted as `HH:mm`, and expanded seats.
- Seat availability contains only `available`.
- Adjacent search contains `found` and nullable row/range values.
- Seat status is serialized as lowercase text by the API mapper.

Use ordinary response records without per-property `JsonIgnore` attributes. A few nullable values in the no-match response are simpler than customizing serialization for a minor cosmetic difference.

### Validation ownership

Validate request constraints at the API boundary:

- seat number must be positive;
- `minSeats` must be at least two.

Do not repeat these checks inside Domain unless the domain service will be called from another unvalidated entry point. Row values are not restricted to one letter because the upstream contract may contain descriptive names such as `Balcony`; lookup is case-insensitive.

Configuration validation is separate. `CinemaFeedOptions` owns the `CinemaFeed` section name, marks `BaseAddress` as required, and is validated at startup through data annotations. The reference only checks presence; it does not add a custom absolute-URI validator.

## Domain decisions

Use immutable records for `Seat`, `SeatMap`, and `AdjacentSeatBlock`, plus a small `SeatStatus` enum.

`ISeatMapProvider` abstracts obtaining the current map. `ISeatMapService` adds lookup and adjacent-seat behavior. Both return `FluentResults.Result<T>` so expected dependency and lookup failures remain explicit without turning normal failures into exceptions.

Seat lookup:

- compares row names using `OrdinalIgnoreCase`;
- returns the canonical row value from the feed;
- returns `SeatNotFoundError` when the requested seat is absent.

Adjacent search:

- processes one row at a time and never carries a run across rows;
- returns the first run of the requested size;
- returns a successful `null` when no block exists;
- relies on the feed mapper producing seats in deterministic row and position order.

The algorithm is intentionally direct. More elaborate allocation rules, seat preferences, and optimization are outside the exercise.

## Feed mapping decisions

The feed mapper performs only validation required to create a usable map:

- exactly one map object;
- non-empty auditorium and film title;
- Unix-seconds start time;
- at least one row;
- non-empty row names and seat strings;
- seat characters limited to `0` and `1`.

Rows are ordered ordinally and each character becomes a one-based seat number. `0` maps to available and `1` to booked.

Use `JsonSerializerDefaults.Web` with `AllowTrailingCommas = true` only in the feed client. Do not relax MVC JSON rules globally to accommodate an external payload.

## Upstream HTTP decisions

### Typed client

Register `ICinemaFeedClient` with `AddHttpClient`. Configuration contains only the base address; the known repository path remains a private constant in the client.

This keeps environment-specific host configuration outside code without creating options for a path that is part of the exercise contract.

The client:

- requests headers first with `ResponseHeadersRead`;
- passes the caller's cancellation token to every asynchronous operation;
- disposes the response and content stream;
- logs final non-success status codes but never response bodies;
- lets caller cancellation propagate;
- converts known transport, timeout, JSON, and HTTP failures to typed errors.

### Resilience

Use `AddStandardResilienceHandler` instead of building a custom Polly pipeline.

| Setting | Choice |
|---|---|
| Total request timeout | 3 seconds |
| Maximum retries | 2 |
| Circuit breaker | Standard handler default |
| Attempt timeout/backoff | Standard handler defaults |

The purpose is to demonstrate the standard .NET resilience integration, not tune a production policy from insufficient traffic data.

The final response is classified separately by the client:

- `404`, `408`, `429`, and `5xx` become `SeatMapUnavailableError`;
- network and resilience timeout failures become `SeatMapUnavailableError`;
- other non-success responses become `UpstreamResponseError`;
- invalid JSON or feed content becomes `InvalidSeatMapError`.

The standard retry predicate is not customized to retry upstream `404`; it is still treated as unavailable if it is the final response. Custom retry predicates are a reasonable follow-up discussion, not required here.

## Cache decisions

Use memory-only FusionCache behind `ISeatMapProvider`.

| Behavior | Value |
|---|---:|
| Fresh duration | 5 seconds |
| Maximum fail-safe duration | 35 seconds total |
| Additional stale window | Up to 30 seconds |

Only successful `SeatMap` values are cached. A failed feed result must be presented to FusionCache as a factory failure so fail-safe can return the last-known-good map. The provider therefore uses one small private exception to bridge `Result`-based application failures to FusionCache's exception-based factory contract. If there is no usable cached value, the provider catches that exception and returns the original typed errors.

This adapter is intentionally local to Infrastructure. Exceptions do not become the application's expected-failure model.

Do not expose cache age or staleness through `Snapshot<T>` or public response metadata. Whether a value came from the last-known-good cache is an infrastructure detail for this exercise.

Do not add Redis, a serializer, a backplane, cache configuration classes, or cache-management endpoints.

## Dependency injection and lifetimes

- FusionCache owns shared in-memory cache state.
- `CachedSeatMapProvider` is scoped and depends on the typed feed client plus `IFusionCache`.
- `SeatMapService` is scoped so it does not capture a shorter-lived provider/client.
- Infrastructure owns registration of its options, typed client, resilience handler, cache, and provider.
- Host composes API, Domain service, Infrastructure, and operational services.

Do not call `BuildServiceProvider` during registration and do not inject `IHttpClientFactory` into the domain service.

## Error and HTTP semantics

Keep a small set of internal typed errors rather than a custom hierarchy with codes:

| Error | HTTP status |
|---|---:|
| API model-validation failure | 400 |
| `SeatNotFoundError` | 404 |
| `InvalidSeatMapError` | 502 |
| `UpstreamResponseError` | 502 |
| `SeatMapUnavailableError` | 503 |
| Empty or unknown error | 500 |

`ResultProblemMapper` is the one API-layer translation point. It uses a type switch and returns standard `ProblemDetails` without exposing internal messages, exception details, upstream content, custom codes, or cache state.

`UseExceptionHandler` handles unexpected defects as generic `500` responses. Expected failures should not reach it.

## Testing decisions

Keep only a focused unit-test project:

- `SeatMapServiceTests` cover case-insensitive lookup, descriptive rows, missing seats, a successful adjacent run, and row boundaries.
- `CinemaFeedMapperTests` cover valid expansion/order, descriptive row labels, and malformed seat strings.

These tests exercise the most important pure logic and contract transformation with little setup.

Do not include controller unit tests, `WebApplicationFactory`, cache timing/concurrency tests, resilience matrices, or exhaustive validation cases. A production service would justify more coverage, but that volume obscures the interview-sized core and makes the reference look generated rather than intentional.

Good follow-up tests to discuss, if asked, are stale fallback, cancellation, retry exhaustion, malformed timestamps, and public endpoint contracts.

## Operational decisions

- Register built-in `ProblemDetails`, exception handling, and status-code pages.
- Expose `/health/live` as process liveness only; it must not call the flaky upstream.
- Generate OpenAPI and expose Scalar only in Development.
- Keep request examples in `Cinema.http`.
- Use a multi-stage .NET 10 Docker image, run as the built-in non-root user, and expose port 8080.
- Keep Compose to one service; the cache is in-process.
- Use central package management, nullable analysis, warnings-as-errors, and Microsoft Testing Platform.

## What to build in a 90-minute interview

The reference app is the finished example, not the recommended implementation order.

Prioritize:

1. model and map the upstream JSON;
2. add the typed HTTP client with cancellation;
3. implement seat lookup;
4. expose the seat-map and single-seat endpoints;
5. return sensible `404`, `502`, and `503` problems;
6. add a few mapper/domain tests.

Add adjacent-seat search next if time remains. Standard resilience, FusionCache fail-safe, OpenAPI polish, Docker, and operational endpoints are strong follow-up improvements, but the core vertical slice should work before they are introduced.

## Deliberate omissions and follow-ups

The current reference intentionally omits:

- snapshot wrappers and freshness fields;
- custom public error codes and custom ProblemDetails writers;
- duplicate validation across API and Domain;
- custom resilience options and policy builders;
- distributed cache and persistence;
- authentication, authorization, rate limiting, and telemetry exporters;
- broad integration, concurrency, and failure-matrix tests;
- CI/CD configuration.

These are not claims that the concerns never matter. They are scope decisions that keep this particular reference proportional to the task.

## Verification

From `reference-solution`:

```bash
dotnet restore Cinema.slnx
dotnet build Cinema.slnx --configuration Release --no-restore
dotnet test Cinema.slnx --configuration Release --no-build
dotnet run --project src/Cinema.Host/Cinema.Host.csproj
```

Use `Cinema.http`, Scalar at `/scalar/v1`, or OpenAPI at `/openapi/v1.json` to exercise the running service.
