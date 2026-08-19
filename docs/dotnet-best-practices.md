# .NET Technical Solution Best Practices

## Purpose

This document provides technical ideas for reviewing a .NET solution and preparing interview questions. It covers C#, ASP.NET Core, architecture, testing, asynchronous programming, data modelling, external services, resilience, dependency injection, cloud concerns, and operability.

Base technical suggestions on repository evidence such as source code, project structure, configuration, build output, automated tests, or exercised endpoints. Communication, teamwork, time management, and interview-answer quality belong in `general-engineering-best-practices.md` and should not be inferred from code.

## Technical Review Order

Review in this order so cosmetic concerns do not hide fundamental failures:

1. restore, build, and test status;
2. required endpoint and business behaviour;
3. correctness of external integration and mapping;
4. HTTP contracts, validation, and failure semantics;
5. architecture, dependency direction, and maintainability;
6. automated test quality and coverage of important risks;
7. resilience, caching, logging, security, and operability;
8. lower-impact style and polish findings.

## 1. C# Proficiency and Code Quality

### 1.1 Keep the solution buildable

**Prefer:** A solution that restores and builds cleanly with the intended SDK, has consistent project references, and treats meaningful compiler and analyzer findings seriously.

**Avoid:** Missing projects, broken references, incompatible package versions, disabled checks used to hide defects, or code that exists only as an uncompiled sketch.

**Why it matters:** Buildability is the minimum evidence that the repository represents an integrated implementation rather than disconnected source fragments.

### 1.2 Prefer readable, cohesive code

**Prefer:** Small methods and classes with one clear purpose, intention-revealing names, limited nesting, and control flow that can be understood without extensive comments.

**Avoid:** God classes, long controller actions, generic names such as `Helper` or `Manager`, duplicated logic, deeply nested conditions, and comments that compensate for unclear code.

**Why it matters:** Cohesion and readability reduce change risk and make correctness, ownership, and test coverage easier to assess.

### 1.3 Use modern C# features deliberately

**Prefer:** Current language features such as records, pattern matching, nullable reference types, collection expressions, and primary constructors when their semantics fit the code.

**Avoid:** Outdated framework-era patterns in a current application or new syntax used where it obscures behaviour.

**Why it matters:** Modern C# can reduce incidental code, but technical quality comes from appropriate semantics rather than novelty.

### 1.4 Enable nullable reference analysis

**Prefer:** Nullable reference types enabled across the solution, accurate annotations, validation at external boundaries, and deliberate handling of optional values.

**Avoid:** Disabling nullable analysis, widespread null-forgiving operators, returning `null` with undocumented meaning, or suppressing warnings without establishing an invariant.

**Why it matters:** Nullability is part of the contract. Accurate analysis prevents a common class of runtime failures and makes optional state explicit.

### 1.5 Use meaningful domain types

**Prefer:** Native .NET and domain-specific types that match the value's meaning: `DateOnly` for a calendar date, `TimeOnly` for a time of day, `DayOfWeek` for a weekday, `DateTimeOffset` for an instant with an offset, enums for closed sets, and value objects for domain invariants.

**Avoid:** Stringly typed dates, times, weekdays, statuses, and identifiers; mixing local and UTC time implicitly; or using primitive values whose meaning changes between layers.

**Why it matters:** Meaningful types communicate invariants and make invalid or ambiguous states harder to construct.

### 1.6 Keep mutation controlled

**Prefer:** Immutable contracts and domain values where practical, localised state changes, read-only collection exposure, and explicit ownership of mutable state.

**Avoid:** Public setters everywhere, shared mutable static state, or returning collections that allow consumers to corrupt internal invariants.

**Why it matters:** Controlled mutation simplifies reasoning, concurrency safety, and testing.

### 1.7 Manage disposable resources correctly

**Prefer:** Clear ownership and disposal of streams, responses, cancellation registrations, and other disposable resources, while allowing DI-managed services to own their registered dependencies.

**Avoid:** Leaking responses or streams, disposing injected services manually, or creating long-lived resources inside short-lived operations without clear ownership.

**Why it matters:** Resource leaks and incorrect disposal lead to connection exhaustion, memory pressure, and behaviour that degrades under load.

### 1.8 Prefer straightforward algorithms

**Prefer:** Direct loops or readable LINQ whose ordering, filtering, complexity, and edge-case behaviour are evident.

**Avoid:** Dense nested LINQ, regular expressions for structured JSON, repeated enumeration with hidden cost, or clever transformations that obscure correctness.

**Why it matters:** Clear algorithms are easier to validate, test, debug, and optimise from evidence.

### 1.9 Keep code style consistent

**Prefer:** Consistent naming, formatting, namespace use, file organisation, and project-wide compiler settings enforced through shared configuration where appropriate.

**Avoid:** Inconsistent property naming, mixed conventions, arbitrary regions, or large amounts of per-file formatting configuration.

**Why it matters:** Consistency lowers navigation cost and prevents style noise from hiding substantive defects.

## 2. REST API and ASP.NET Core

### 2.1 Use resource-oriented routes

**Prefer:** Nouns in routes, HTTP methods for operations, route parameters for resource identity, and query parameters for filtering or search criteria.

**Avoid:** Verb-heavy paths such as `/getSeat`, inconsistent route conventions, or ambiguous endpoint identities.

**Why it matters:** Conventional routes make the public contract predictable and support standard client and infrastructure behaviour.

### 2.2 Use precise HTTP status codes

**Prefer:** Status codes that match the outcome, such as `200` for reads, `201 Created` for creation, `400` for invalid requests, `404` for absent resources, `502` for invalid dependency responses, and `503` for temporary dependency unavailability.

**Avoid:** Returning `200` for every outcome, using `400` for server failures, or leaking dependency status codes without translating their meaning for the public API.

**Why it matters:** HTTP status is part of the contract and drives client behaviour, monitoring, caching, and retry decisions.

### 2.3 Keep public response contracts explicit

**Prefer:** Small response types designed for API consumers, with stable naming and only the data the endpoint promises.

**Avoid:** Returning infrastructure DTOs, persistence entities, raw `HttpResponseMessage`, or unrelated internal metadata.

**Why it matters:** Explicit contracts allow the public API to evolve independently from external and internal models.

### 2.4 Validate transport input at the boundary

**Prefer:** Route, query, header, and body constraints validated before business processing through ASP.NET Core validation or an equally clear boundary mechanism.

**Avoid:** Allowing invalid transport values deep into application logic, duplicating identical checks across layers, or adding a validation framework without a coherent ownership model.

**Why it matters:** Boundary validation produces consistent client failures and keeps domain code focused on business invariants.

### 2.5 Keep business validation distinct

**Prefer:** Business invariants enforced by the domain or application component that owns them, independently of the API transport.

**Avoid:** Treating model binding as sufficient for domain correctness or placing every business rule in controllers.

**Why it matters:** Business rules must remain correct when invoked from tests, background work, or another transport.

### 2.6 Model expected failures explicitly

**Prefer:** A small consistent error mechanism for expected outcomes such as not found, invalid dependency content, conflict, and temporary unavailability.

**Avoid:** Exceptions for normal branches, ambiguous `null` results, string comparisons on error messages, or several unrelated error conventions in one solution.

**Why it matters:** Explicit failures make control flow reviewable and support consistent HTTP translation.

### 2.7 Use standard problem responses

**Prefer:** Central translation of expected failures into `ProblemDetails` or validation problem details, with safe and stable public messages.

**Avoid:** Raw exception text, stack traces, dependency bodies, or a different error shape for each endpoint.

**Why it matters:** Standard error contracts simplify client handling and prevent leakage of internal or sensitive details.

### 2.8 Handle unexpected exceptions centrally

**Prefer:** Register `AddProblemDetails()` and use `UseExceptionHandler()` for a safe global fallback, adding a custom `IExceptionHandler` through `AddExceptionHandler<T>()` only when consistent exception-to-response mapping is needed. Keep expected failures in the explicit error flow and return a generic `500` problem for unexpected defects.

**Avoid:** Repeating broad `try/catch` blocks in every endpoint, returning `Exception.Message` or stack traces to callers, converting all failures to `400`, or misrepresenting unexpected defects as `404`.

**Why it matters:** Central exception handling produces consistent safe responses without leaking implementation details, while preserving precise status codes for expected outcomes.

### 2.9 Keep endpoints thin

**Prefer:** Controllers or handlers that accept and validate transport input, invoke one focused application/domain operation, and translate its result.

**Avoid:** Feed parsing, retry policy, caching, data access, or non-trivial business algorithms inside endpoint methods.

**Why it matters:** Thin endpoints isolate HTTP behaviour and keep core logic reusable and independently testable.

### 2.10 Choose controllers or Minimal APIs proportionately

**Prefer:** Controllers when conventional validation, metadata, and grouping add value; Minimal APIs when the surface remains small and the handler boundaries stay focused.

**Avoid:** Scoring either style as inherently stronger or accepting large inline Minimal API handlers that mix every responsibility.

**Why it matters:** API style is a design choice. Contract correctness, separation, and testability are the relevant technical evidence.

### 2.11 Make the public API executable

**Prefer:** Working routes that can be exercised through integration tests, an `.http` file, OpenAPI tooling, or another repeatable request mechanism.

**Avoid:** Treating successful compilation or a unit test of an internal service as proof that routing, DI, serialization, and response behaviour work together.

**Why it matters:** Public execution verifies the assembled application boundary rather than isolated components.

### 2.12 Evolve public contracts deliberately

**Prefer:** Additive changes where possible, stable field meanings and failure semantics, contract tests for important consumers, and a clear versioning or deprecation path when a breaking change is necessary.

**Avoid:** Renaming or removing fields, changing status-code meaning, or silently altering response behaviour without considering existing clients.

**Why it matters:** An API is a shared contract. Safe evolution lets the service improve without forcing unexpected client failures, but versioning is unnecessary when no compatibility requirement exists.

## 3. Software Architecture and Design

### 3.1 Use proportionate clean boundaries

**Prefer:** Clear separation between API transport, business behaviour, and external infrastructure using the smallest structure that enforces useful dependency direction.

**Avoid:** One class containing routing, HTTP calls, mapping, caching, and business rules, or many layers that contain no meaningful behaviour.

**Why it matters:** Boundaries improve changeability and testability; excessive layering hides the core flow and adds maintenance cost.

### 3.2 Judge boundaries rather than project count

**Prefer:** Multiple projects when they enforce meaningful dependencies, or a single project with focused folders and classes when the solution is small.

**Avoid:** Treating one project as automatically unscalable or four projects as automatically clean.

**Why it matters:** Scalability and maintainability come from cohesion, coupling, and dependency direction, not the number of project files.

### 3.3 Separate external, domain, and API models

**Prefer:** Dedicated upstream DTOs mapped into meaningful internal types and then into independent public response contracts.

**Avoid:** One catch-all model used at every boundary or direct exposure of dependency payloads through the public API.

**Why it matters:** Each contract changes for different reasons; separation prevents dependency changes from silently breaking consumers.

### 3.4 Give business behaviour a clear owner

**Prefer:** Focused services or domain types for lookup, adjacency, validation, and orchestration, with APIs that reveal their responsibilities.

**Avoid:** Generic manager classes, controllers containing business rules, or excessive pass-through services.

**Why it matters:** Clear ownership improves cohesion and makes behaviour easier to locate, test, and modify.

### 3.5 Keep dependency direction inward

**Prefer:** Business code that depends on domain-level contracts while infrastructure implements external HTTP, caching, persistence, and framework-specific details.

**Avoid:** Domain code referencing ASP.NET Core, `HttpClient`, JSON serializers, cache libraries, or hosting configuration.

**Why it matters:** Inward dependencies protect core behaviour from infrastructure churn and allow isolated tests.

### 3.6 Use interfaces at real seams

**Prefer:** Interfaces for external clients, providers, persistence, clocks, and other boundaries that genuinely need substitution or inversion.

**Avoid:** Direct infrastructure dependencies from business code or an interface for every concrete class without alternate implementation, boundary, or testing value.

**Why it matters:** Interfaces reduce coupling when they express a meaningful contract; interface-per-class ceremony adds navigation cost without architectural benefit.

### 3.7 Prefer direct designs over speculative patterns

**Prefer:** Repositories, result types, mediators, factories, decorators, and pipelines only where they solve a visible responsibility or variation.

**Avoid:** Generic repositories over in-memory collections, unused base entities, decorative CQRS, or patterns that only forward calls.

**Why it matters:** Patterns should simplify a real design force rather than increase code volume and indirection.

### 3.8 Keep feature code discoverable

**Prefer:** Files grouped by coherent responsibility or feature, with registrations and contracts near the components they configure.

**Avoid:** Undifferentiated `Models`, `Services`, or `Helpers` folders containing unrelated code, and DTO files that combine several external and public contracts.

**Why it matters:** Discoverable structure reduces the context required to understand and safely change one feature.

### 3.9 Keep configuration outside business code

**Prefer:** Typed configuration bound and validated near the composition or infrastructure boundary.

**Avoid:** Hardcoded environment values, direct configuration lookups throughout the domain, or passing raw configuration keys between layers.

**Why it matters:** Configuration is an external input and should not distort core contracts or create hidden dependencies.

## 4. Unit, Integration, and Acceptance Testing

### 4.1 Test high-value pure behaviour first

**Prefer:** Focused tests for mapping, lookup, validation, adjacency, ordering, malformed data, and other important deterministic rules.

**Avoid:** Large suites of trivial property tests, framework-internal behaviour, or controllers whose only logic is delegation.

**Why it matters:** Pure behaviour tests provide high confidence with low setup cost and precise failure diagnosis.

### 4.2 Distinguish test levels by boundary

**Prefer:** Unit tests for isolated logic, integration tests for assembled component boundaries, and end-to-end or acceptance tests for critical deployed journeys.

**Avoid:** Calling a test a unit test while it constructs a real host or database, or expecting one test level to cover all risks.

**Why it matters:** Each level provides a different balance of realism, speed, and diagnostic precision.

### 4.3 Test the public API contract

**Prefer:** Integration coverage for routing, validation, JSON shape, status codes, problem responses, dependency registration, and important failure paths.

**Avoid:** Assuming service unit tests prove that the HTTP application behaves correctly.

**Why it matters:** Many defects occur in composition, serialization, model binding, and HTTP translation rather than in pure business logic.

### 4.4 Substitute stable boundaries

**Prefer:** Fakes, stubs, or mocks around focused interfaces such as an upstream client or provider, and a controlled HTTP handler when testing the client itself.

**Avoid:** Mocking implementation details, fragile call-order assertions, or trying to mock non-virtual `HttpClient` methods directly.

**Why it matters:** Boundary-focused tests survive refactoring and make failure arrangements easier to understand.

### 4.5 Test behaviour rather than implementation shape

**Prefer:** Assertions on returned values, errors, public contracts, and significant side effects, with names that state condition and expected result.

**Avoid:** Assertions on private call sequences, tests that duplicate the production algorithm, or names such as `Test1`.

**Why it matters:** Behavioural tests document requirements and allow the internal design to improve safely.

### 4.6 Cover meaningful failure paths

**Prefer:** Tests for invalid input, absent resources, malformed dependency payloads, non-success dependency responses, cancellation, timeout, and fallback behaviour where implemented.

**Avoid:** Success-only coverage for code whose primary complexity is dependency failure handling.

**Why it matters:** Failure paths are where integration and resilience code most often contains unobserved defects.

### 4.7 Keep tests deterministic

**Prefer:** Controlled clocks, known test data, isolated state, bounded asynchronous work, and explicit setup for cache or concurrency behaviour.

**Avoid:** Real network dependencies, ambient local time, shared mutable fixtures, arbitrary delays, or order-dependent tests.

**Why it matters:** Deterministic tests produce trustworthy signals and stable automation.

### 4.8 Keep test setup proportional

**Prefer:** Small builders, fixtures, or object mothers only when they remove repeated irrelevant setup while keeping the scenario visible.

**Avoid:** Complex test frameworks, excessive AutoFixture customization, or large mock graphs for simple domain behaviour.

**Why it matters:** Test infrastructure should reduce noise rather than become another system that obscures intent.

### 4.9 Treat manual execution as supplementary evidence

**Prefer:** Automated regression coverage plus a repeatable public smoke request for the assembled application.

**Avoid:** Only manual Swagger checks or an unrecorded claim that the endpoint worked once.

**Why it matters:** Manual execution proves the current assembly; automated tests preserve that evidence as the repository changes.

## 5. Asynchronous Programming and Concurrency

### 5.1 Use `Task`-based asynchronous APIs

**Prefer:** `Task` or `Task<T>` for naturally asynchronous I/O and application operations, using `await` where it preserves clear control flow and error handling.

**Avoid:** `async void`, blocking with `.Result` or `.Wait()`, or `ValueTask<T>` without a measured allocation-sensitive use case.

**Why it matters:** Conventional task-based APIs compose reliably and propagate errors correctly.

### 5.2 Propagate cancellation end to end

**Prefer:** Accept the request `CancellationToken` and pass it through application services, HTTP calls, stream reads, delays, and other cancellable operations.

**Avoid:** Ignoring the supplied token, creating unrelated tokens, or converting caller cancellation into a dependency failure.

**Why it matters:** Cancellation prevents wasted work after a request ends and preserves correct failure semantics.

### 5.3 Avoid synchronous-over-asynchronous execution

**Prefer:** Asynchronous flow from API boundary to I/O boundary with no blocking bridge.

**Avoid:** `.Result`, `.Wait()`, `GetAwaiter().GetResult()`, or thread-pool wrapping around naturally asynchronous I/O.

**Why it matters:** Blocking async work can deadlock, reduce throughput, and consume request threads unnecessarily.

### 5.4 Add concurrency only for real shared mutation

**Prefer:** Identify shared state, atomicity requirements, and access patterns before choosing locks or concurrent collections.

**Avoid:** `ConcurrentDictionary`, `BlockingCollection`, locks, or parallel work simply because multiple requests may exist.

**Why it matters:** Concurrency primitives solve specific hazards and introduce semantics that must match the state being protected.

### 5.5 Match service lifetime to thread-safety guarantees

**Prefer:** Immutable singleton dependencies or explicitly thread-safe shared state, with scoped and transient services used according to their dependencies and state.

**Avoid:** Mutable non-thread-safe singletons or shared static collections hidden behind shorter-lived services.

**Why it matters:** DI lifetime and concurrency correctness are connected; a valid registration can still expose unsafe shared state.

### 5.6 Keep asynchronous tests bounded

**Prefer:** Awaited tasks, explicit cancellation, deterministic completion signals, and timeouts only as safety bounds.

**Avoid:** Fire-and-forget work, unobserved exceptions, or `Task.Delay` as the primary synchronization mechanism in tests.

**Why it matters:** Bounded async tests reveal real failures instead of producing hangs or intermittent results.

## 6. Problem Solving, Data Modelling, and Mapping

### 6.1 Model the source contract accurately

**Prefer:** DTO shapes, property types, serializer settings, and nullability that match the actual dependency payload.

**Avoid:** Guessing the JSON structure, forcing mismatched values into strings, or repeatedly changing unrelated code when deserialization fails.

**Why it matters:** Incorrect source modelling prevents every downstream feature from working and was a frequent implementation failure in the collected feedback.

### 6.2 Validate dependency data before domain construction

**Prefer:** Check the fields needed to create a usable internal model, including collection presence, allowed values, timestamps, and structural invariants.

**Avoid:** Assuming successful deserialization means the payload is semantically usable.

**Why it matters:** Syntactically valid JSON may still violate the application's required invariants.

### 6.3 Keep mapping deterministic

**Prefer:** Explicit ordering, stable row and seat numbering, well-defined duplicate handling, and repeatable transformation rules.

**Avoid:** Depending on incidental dictionary order, accepting duplicates silently, or producing output whose order changes across runs.

**Why it matters:** Deterministic mapping supports stable API contracts, reliable tests, and reproducible failures.

### 6.4 Choose collections from access patterns

**Prefer:** Dictionaries for repeated keyed lookup, lists for ordered traversal, sets for uniqueness, and immutable or read-only views where mutation is unnecessary.

**Avoid:** Repeated linear scans for key access or concurrent collections without concurrent mutation.

**Why it matters:** Collection choice affects algorithmic complexity, clarity, and correctness.

### 6.5 Keep algorithms aligned with domain boundaries

**Prefer:** Algorithms that preserve row boundaries, case rules, ordering, and no-result semantics explicitly.

**Avoid:** Carrying state across unrelated groups, conflating not-found with invalid input, or relying on accidental source order.

**Why it matters:** Small boundary mistakes can produce plausible but incorrect business results.

### 6.6 Handle duplicates explicitly

**Prefer:** Define whether duplicate keys are rejected, merged, ignored, or treated as conflicts at the boundary that owns the rule.

**Avoid:** Allowing collection construction to throw an unexplained exception or silently overwrite data without a documented rule.

**Why it matters:** Duplicate handling is a business and contract decision, not merely a collection implementation detail.

### 6.7 Make no-result semantics unambiguous

**Prefer:** Distinguish an empty successful search from a missing resource, invalid request, and dependency failure through explicit result types or contracts.

**Avoid:** Using `null`, empty collections, and exceptions interchangeably for different outcomes.

**Why it matters:** Clear absence semantics prevent incorrect HTTP mapping and consumer ambiguity.

### 6.8 Optimise only observable hot paths

**Prefer:** Complexity appropriate to known data size and access frequency, with simple improvements such as avoiding repeated fetch, parse, mapping, and lookup work.

**Avoid:** Premature parallelism, distributed components, or micro-optimisation while correctness and required behaviour remain incomplete.

**Why it matters:** Evidence-driven optimisation preserves clarity and targets real cost.

## 7. Third-Party HTTP Services and Resilience

### 7.1 Prefer a typed or named `HttpClient`

**Prefer:** A typed or named client registered through `IHttpClientFactory`, with dependency-specific behaviour inside a dedicated adapter.

**Avoid:** `new HttpClient()` per operation, manual client construction inside business services, or an unconfigured factory used only as ceremony.

**Why it matters:** Factory-managed clients centralise configuration and handler pipelines while avoiding connection-management problems.

### 7.2 Keep environment settings out of code

**Prefer:** Base addresses, credentials, and environment-specific policy values supplied through validated configuration.

**Avoid:** Hardcoded hosts, secrets, timeouts, or environment branches in client implementation and service registration.

**Why it matters:** External settings vary by environment and must be changeable without recompiling the application.

### 7.3 Encapsulate the dependency contract

**Prefer:** A focused client contract that returns application-meaningful results and keeps paths, headers, serializer details, and status classification inside infrastructure.

**Avoid:** Passing raw URLs through layers or exposing `HttpResponseMessage` and dependency DTOs to domain code.

**Why it matters:** Encapsulation limits the blast radius of dependency changes and keeps transport policy out of business logic.

### 7.4 Handle response resources correctly

**Prefer:** Dispose responses and content streams, propagate cancellation, and consider `ResponseHeadersRead` when streaming is appropriate.

**Avoid:** Leaked responses, unnecessary full buffering, or loss of cancellation between sending and reading content.

**Why it matters:** Correct resource handling prevents connection pressure and improves behaviour under load.

### 7.5 Parse into explicit dependency DTOs

**Prefer:** DTOs that match the dependency schema and serializer options scoped to that integration.

**Avoid:** Regular expressions, loosely navigated JSON for a stable contract, or global serializer relaxation for one dependency.

**Why it matters:** Explicit DTOs expose schema mismatches and make deserialization and validation testable.

### 7.6 Keep mapping separate from transport

**Prefer:** A small mapper that converts validated dependency DTOs into internal types.

**Avoid:** Mapping in controllers, cache adapters, or HTTP send code, and mapping libraries that hide the primary transformation being reviewed.

**Why it matters:** Separate mapping localises transformation defects and supports focused unit tests.

### 7.7 Classify dependency failures

**Prefer:** Distinct handling for transport failure, timeout, non-success status, malformed JSON, semantically invalid content, and caller cancellation.

**Avoid:** A broad catch that converts every failure into one vague result or leaks dependency exceptions through the public API.

**Why it matters:** Different failures require different retry, fallback, logging, and HTTP behaviour.

### 7.8 Set an explicit timeout budget

**Prefer:** A bounded overall timeout compatible with the public request budget, coordinated with per-attempt limits and caller cancellation.

**Avoid:** Infinite waits, several uncoordinated timeout layers, or retry schedules that exceed the caller's useful latency.

**Why it matters:** Timeouts limit resource consumption and prevent a slow dependency from exhausting request capacity.

### 7.9 Retry only transient safe operations

**Prefer:** Small bounded retries with backoff for transient failures on idempotent operations and an explicit retry predicate.

**Avoid:** Retrying every `4xx`, retrying non-idempotent writes blindly, or multiplying latency through excessive attempts.

**Why it matters:** Incorrect retries amplify incidents and can duplicate side effects.

### 7.10 Use circuit breaking coherently

**Prefer:** A circuit breaker as part of a standard resilience pipeline when repeated calls to a failing dependency would cause further harm.

**Avoid:** Treating circuit breaking as a replacement for timeouts or retries, or configuring it without understanding its state transitions.

**Why it matters:** Circuit breaking protects both services during sustained failure when its thresholds match real traffic behaviour.

### 7.11 Prefer standard resilience integration

**Prefer:** The current .NET resilience handler or another established pipeline with a small, inspectable configuration.

**Avoid:** Custom policy frameworks and extensive tuning without service-level objectives, traffic, or failure data.

**Why it matters:** Standard integrations reduce accidental interactions and keep policy behaviour discoverable.

### 7.12 Cache only successful validated values

**Prefer:** A short TTL tied to freshness needs, stable cache keys, bounded storage, and a cache hidden behind an infrastructure boundary.

**Avoid:** Caching failures, permanent entries, unnecessary serialization for in-memory values, or distributed caching without a scale requirement.

**Why it matters:** Clear cache semantics improve latency without poisoning results or coupling the public API to infrastructure.

### 7.13 Use last-known-good data only when valid for the domain

**Prefer:** A bounded stale fallback after refresh failure when old data is safer than unavailability, with only previously successful values eligible.

**Avoid:** Serving stale data indefinitely or using fallback where outdated information can cause greater harm.

**Why it matters:** Fail-safe caching is a business availability trade-off, not a universally correct technical feature.

## 8. Design Patterns and Dependency Injection

### 8.1 Keep a single composition root

**Prefer:** Service registration concentrated in the host or composition boundary, with small feature extension methods where they improve organisation.

**Avoid:** Registration scattered through business classes or application code that constructs its own dependency graph.

**Why it matters:** A visible composition root makes lifetimes, configuration, decorators, and dependency direction reviewable.

### 8.2 Use constructor injection

**Prefer:** Required dependencies supplied through constructors and represented by focused contracts.

**Avoid:** Service location, mutable dependency properties, static accessors, or method parameters that repeatedly carry infrastructure dependencies.

**Why it matters:** Constructor injection exposes requirements and prevents partially initialised services.

### 8.3 Select lifetimes from state and dependencies

**Prefer:** Transient, scoped, and singleton lifetimes chosen from state ownership, thread safety, and the lifetimes of dependencies.

**Avoid:** Guessing, using static fields to compensate for a wrong lifetime, or capturing scoped services in singletons.

**Why it matters:** Lifetime defects cause lost state, captive dependencies, races, and failures that vary between requests.

### 8.4 Avoid building nested service providers

**Prefer:** Options, factories, and registration callbacks supported by the container.

**Avoid:** Calling `BuildServiceProvider` during registration to resolve another service.

**Why it matters:** Nested providers create duplicate singleton graphs, disposal problems, and configuration that differs from the real application container.

### 8.5 Apply decorators at cross-cutting boundaries

**Prefer:** Decorators or pipelines for cache, resilience, metrics, and logging when they wrap a coherent contract without changing business semantics.

**Avoid:** Cross-cutting code copied into every service or layers of decorators that make execution order unclear.

**Why it matters:** A well-placed decorator isolates policy; excessive wrapping obscures control flow.

### 8.6 Do not confuse a library with a pattern

**Prefer:** Pattern terminology tied to actual responsibilities, dependency direction, and message or control flow in the code.

**Avoid:** Claiming CQRS because MediatR is installed, repository architecture because an interface exists, or clean architecture because projects have familiar names.

**Why it matters:** Patterns are structural decisions, not package references or naming conventions.

### 8.7 Keep abstractions no broader than their consumers need

**Prefer:** Small interfaces shaped by the use case and owned near the consuming boundary.

**Avoid:** Large generic service interfaces that expose every operation or mirror the complete concrete class.

**Why it matters:** Narrow contracts reduce coupling and make substitutions and tests more focused.

## 9. Microservices, Cloud, Deployment, and Observability

### 9.1 Use structured logging

**Prefer:** Stable event messages with named properties such as dependency, status code, duration, and correlation context at an appropriate level.

**Avoid:** String-concatenated logs, logging every successful internal step, or messages without enough context to diagnose failure.

**Why it matters:** Structured logs support search, aggregation, alerting, and incident diagnosis.

### 9.2 Protect sensitive data in diagnostics

**Prefer:** Log only metadata required for diagnosis and redact tokens, credentials, personal data, and sensitive payload fields.

**Avoid:** Full request or response bodies, authorization headers, secrets, or raw exception details in public responses.

**Why it matters:** Observability must not introduce privacy, security, or cost risks.

### 9.3 Separate liveness and readiness semantics

**Prefer:** Liveness that confirms the process can run and readiness that reflects whether the instance can serve traffic, with dependency checks added deliberately.

**Avoid:** A liveness endpoint that calls a flaky dependency and causes healthy instances to restart during an upstream outage.

**Why it matters:** Incorrect health checks can amplify dependency failure into a platform-wide availability incident.

### 9.4 Add metrics and tracing at useful boundaries

**Prefer:** Request and dependency latency, error rate, cache outcomes, retry activity, circuit state, and trace propagation around external calls.

**Avoid:** Logging as the only observability mechanism or high-cardinality labels that make telemetry expensive and difficult to aggregate.

**Why it matters:** Metrics and traces reveal trends and distributed request behaviour that individual log events cannot.

### 9.5 Validate configuration at startup

**Prefer:** Required options bound to typed configuration and validated before serving requests.

**Avoid:** Silent invalid defaults or failures deferred until the first dependency call.

**Why it matters:** Startup validation turns hidden runtime faults into immediate deployment feedback.

### 9.6 Keep secrets outside source control

**Prefer:** Environment variables, secret stores, workload identity, or platform-supported secret injection with no credentials committed to the repository.

**Avoid:** API keys, connection strings, tokens, or private certificates in source, examples, images, or test data.

**Why it matters:** Repository history is durable and broadly distributed; committed secrets remain exposed even after deletion.

### 9.7 Keep container images small and non-root

**Prefer:** Multi-stage builds, pinned appropriate base images, only published runtime artifacts, a non-root runtime user, and explicit ports and health behaviour.

**Avoid:** SDK images in production, root execution without need, copied source and build caches, or secrets embedded during the build.

**Why it matters:** Smaller least-privilege images reduce attack surface, transfer cost, and operational ambiguity.

### 9.8 Provide repeatable build and test commands

**Prefer:** Documented restore, build, test, and run commands that work from a clean checkout and align with automation.

**Avoid:** Undocumented local prerequisites, reliance on IDE-only state, or commands that modify source unexpectedly.

**Why it matters:** Repeatability is required for CI, review, deployment, and incident recovery.

### 9.9 Address API security at the correct boundary

**Prefer:** Authentication, authorization, rate limiting, input size limits, transport security, and safe error responses where required by the service contract.

**Avoid:** Security checks scattered through domain logic or a public endpoint left unrestricted when the requirements demand protection.

**Why it matters:** Security controls must be consistent and located where identity and transport context are available.

### 9.10 Design for scale without implementing imaginary infrastructure

**Prefer:** Stateless request handling where practical, bounded work, efficient access patterns, and clearly identified extension points for distributed cache or messaging when justified.

**Avoid:** Redis, message queues, sharding, distributed locks, or Kubernetes-specific code without a requirement or demonstrated scale constraint.

**Why it matters:** A scalable design anticipates real constraints while keeping current complexity proportional.

### 9.11 Keep telemetry and health out of domain code

**Prefer:** Instrumentation at hosting, API, integration, and decorator boundaries with domain logic remaining framework-independent.

**Avoid:** Domain models that depend on logging providers, metrics SDKs, or cloud-specific health libraries.

**Why it matters:** Operational concerns should observe the domain without coupling its rules to one platform.

## 10. Turning Review into Useful Suggestions

### 10.1 Use evidence, not resemblance to one preferred implementation

**Prefer:** Judge build results, working behaviour, correctness, boundaries, tests, and failure handling while accepting justified alternative designs.

**Avoid:** Requiring controllers, a fixed project count, particular result or cache libraries, or one exact algorithm when alternatives satisfy the requirements.

**Why it matters:** Different coherent designs can satisfy the same technical requirements and quality constraints.

### 10.2 Do not invent gaps

**Prefer:** Say that an area could not be verified when source, tests, configuration, and execution do not support a useful conclusion.

**Avoid:** Presenting missing cloud artifacts, integration tests, or resilience features as weaknesses when they were outside the exercise or could not be observed.

**Why it matters:** Useful preparation notes distinguish missing evidence from incorrect implementation.

### 10.3 Rank findings by technical impact

**Prefer:** Prioritise broken builds, missing core behaviour, incorrect contracts, unsafe dependency integration, data corruption, and absent high-value tests before naming and formatting concerns.

**Avoid:** A flat list where a spelling issue appears equivalent to an endpoint that never runs.

**Why it matters:** Preparation effort should target defects most likely to affect correctness and interview evaluation.

### 10.4 Distinguish required work from production follow-up

**Prefer:** Discuss core exercise requirements directly and label authentication, distributed caching, broad telemetry, exhaustive failure matrices, and CI/CD as follow-ups when not required.

**Avoid:** Demanding every production capability from an interview-sized repository.

**Why it matters:** Unrealistic suggestions encourage overengineering and obscure whether the requested service works correctly.

### 10.5 Treat a failed build or test as evidence, not a review blocker

**Prefer:** Record the exact command and failure, continue static inspection where possible, and scope conclusions to the evidence available.

**Avoid:** Stopping the entire review or silently fixing the candidate repository before understanding it.

**Why it matters:** Failure is itself meaningful technical evidence, while unrequested modification changes the artifact being assessed.

### 10.6 Cite concrete repository evidence

**Prefer:** File and line references, project relationships, test names, command output, routes exercised, and configuration values relevant to each finding.

**Avoid:** Generic claims such as "architecture is weak" or "testing is good" without supporting observations.

**Why it matters:** Evidence-backed findings are reviewable, actionable, and suitable for later self-Q&A generation.

### 10.7 Do not infer behavioural competencies from code

**Prefer:** Restrict code-derived suggestions to technical repository evidence. Discuss communication, teamwork, prioritisation, and ownership only from direct feedback or as clearly labelled general habits.

**Avoid:** Inferring interview communication, collaboration, time management, or leadership from code style, commit count, or project structure.

**Why it matters:** Behavioural inference from source code is unreliable and can make otherwise useful preparation advice feel unfair.
