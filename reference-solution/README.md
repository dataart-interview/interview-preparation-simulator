# Cinema Seat Service Reference Solution

This .NET 10 reference implementation isolates upstream feed DTOs, domain models, and public REST contracts. It uses controllers, a typed HTTP client with standard timeout/retry/circuit-breaker handling, a small in-memory cache, ProblemDetails, health checks, OpenAPI, and Scalar.

## Run locally

```bash
dotnet restore Cinema.slnx
dotnet build Cinema.slnx --configuration Release --no-restore
dotnet test Cinema.slnx --configuration Release --no-build
dotnet run --project src/Cinema.Host/Cinema.Host.csproj
```

Open [Scalar](http://localhost:8080/scalar/v1) or [OpenAPI](http://localhost:8080/openapi/v1.json). The service listens on port 8080. Try `GET /api/v1/seat-map`, `GET /api/v1/seats/B/3`, and `GET /api/v1/adjacent-seats?minSeats=2`.

The cache reuses a successful response for five seconds and can fall back to that last-known-good map for a further 30 seconds if the upstream feed fails. Cache state stays inside the infrastructure layer and does not change the public response contract. The feed parser accepts its known trailing comma while rejecting unusable content.

The upstream base address is configured in the `CinemaFeed` section. The known repository path remains private to the typed feed client.

For containers, run `docker compose up --build` from this directory.
