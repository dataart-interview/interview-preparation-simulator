# Simple Intentionally Flawed Cinema Candidate Design

## Goal

Create a small, buildable .NET 10 Cinema Seat Service submission for repository evaluation. The submission must omit automated tests and intentionally demonstrate several common engineering weaknesses while still implementing the two core read endpoints.

## Scope

Create a new, isolated `candidate-solution/` directory. Do not modify `reference-solution/` or unrelated existing files.

The candidate service will expose:

- `GET /api/seats` — fetch the upstream seat map and return an object-per-seat response;
- `GET /api/seats/{row}/{number}` — return whether the requested seat is available.

The candidate will use the existing exercise feed URL and tolerate its trailing comma during JSON parsing. It will not implement adjacent-seat search, health checks, Docker support, or automated tests.

## Intentional weaknesses

The implementation is deliberately one-project and controller-centric. It will intentionally:

- construct `HttpClient` inside request handling;
- block on asynchronous HTTP work with `.Result`;
- parse and map external JSON inside the controller;
- represent feed, domain, and response values with simple strings/classes;
- omit request validation and explicit missing-seat handling;
- omit cancellation propagation, retries, timeouts, caching, resilience, structured logging, and `ProblemDetails` mapping;
- allow upstream and parsing exceptions to become generic server failures;
- use mutable collections and nullable analysis disabled;
- contain no test project or test files.

These weaknesses are intentional evaluation signals, not accidental acceptance criteria.

## Structure

```text
candidate-solution/
├── Cinema.Candidate.sln
├── Cinema.Candidate/
│   ├── Cinema.Candidate.csproj
│   ├── Program.cs
│   └── Controllers/SeatsController.cs
└── README.md
```

The solution will use the ASP.NET Core Web SDK, target `net10.0`, and rely only on the shared framework. The controller will call the hard-coded feed URL, deserialize the first feed item, expand row strings into seat records, and reuse that map for single-seat lookup.

## Verification

Run `dotnet restore candidate-solution/Cinema.Candidate.sln` and `dotnet build candidate-solution/Cinema.Candidate.sln`. Verify that the project builds without a test project. If the local SDK supports it, start the API and make a lightweight request to the map endpoint; do not add tests or repair the intentional weaknesses.
