# Simple Intentionally Flawed Cinema Candidate Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Build a small, buildable .NET 10 Cinema Seat Service candidate with no automated tests and several deliberate engineering weaknesses for evaluation.

**Architecture:** Use one ASP.NET Core Web API project with a single controller responsible for HTTP handling, upstream access, JSON parsing, mapping, and seat lookup. Keep the implementation intentionally direct and under-abstracted while preserving the two core read endpoints.

**Tech Stack:** .NET 10, ASP.NET Core Web SDK, `System.Text.Json`, built-in ASP.NET Core controllers, no third-party packages.

**Spec:** `docs/superpowers/specs/2026-08-18-simple-intentionally-flawed-cinema-candidate-design.md`

## Global Constraints

- Target `net10.0`.
- Create all candidate files under `candidate-solution/`.
- Do not modify `reference-solution/` or unrelated existing files.
- Do not create a test project or test files.
- Use only the ASP.NET Core shared framework; do not add NuGet packages.
- Keep the upstream URL hard-coded in the controller.
- Preserve the intentional weaknesses described in the approved spec.
- Verify restore and build, but do not add automated tests or repair the deliberate design flaws.

---

### Task 1: Scaffold the candidate solution

**Files:**
- Create: `candidate-solution/Cinema.Candidate.sln`
- Create: `candidate-solution/Cinema.Candidate/Cinema.Candidate.csproj`
- Create: `candidate-solution/Cinema.Candidate/Program.cs`

**Interfaces:**
- Produces a solution containing one executable ASP.NET Core project named `Cinema.Candidate`.
- `Program.cs` starts the controller-based API and contains no business logic.

- [ ] **Step 1: Create the solution and project directories**

Create `candidate-solution/` and `candidate-solution/Cinema.Candidate/`. Keep the directory free of `tests/`, `*.Tests`, and unrelated projects.

- [ ] **Step 2: Define the web project**

Create `candidate-solution/Cinema.Candidate/Cinema.Candidate.csproj` with the ASP.NET Core Web SDK and this target framework:

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>disable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
</Project>
```

- [ ] **Step 3: Add the project to the solution**

Create `Cinema.Candidate.sln` and add the project so `dotnet sln candidate-solution/Cinema.Candidate.sln list` reports exactly one project.

- [ ] **Step 4: Add the minimal host**

Create `candidate-solution/Cinema.Candidate/Program.cs`:

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run();
```

This intentionally omits health checks, OpenAPI, exception middleware, logging configuration, and resilience registration.

- [ ] **Step 5: Restore and compile the scaffold**

Run:

```bash
dotnet restore candidate-solution/Cinema.Candidate.sln
dotnet build candidate-solution/Cinema.Candidate.sln --no-restore
```

Expected: restore and build succeed with one web project and no test project.

### Task 2: Implement the intentionally weak seat API

**Files:**
- Create: `candidate-solution/Cinema.Candidate/Controllers/SeatsController.cs`

**Interfaces:**
- `GET /api/seats` returns an object with `auditorium`, `filmTitle`, `startTime`, and a `seats` array.
- `GET /api/seats/{row}/{number}` returns `{ "available": true|false }` for a found seat.
- A missing seat is intentionally not handled explicitly and may produce a generic server error.

- [ ] **Step 1: Define the controller and hard-coded feed access**

Create a controller with these characteristics:

```csharp
[ApiController]
[Route("api/seats")]
public class SeatsController : ControllerBase
{
    private const string FeedUrl = "https://raw.githubusercontent.com/dataart-interview/interview-technical-exercise-dotnet/main/seatmap-example.json";

    [HttpGet]
    public SeatMapResponse GetAll() => LoadMap();

    [HttpGet("{row}/{number}")]
    public object GetOne(string row, int number)
    {
        var map = LoadMap();
        var seat = map.Seats.FirstOrDefault(x =>
            x.Row.Equals(row, StringComparison.OrdinalIgnoreCase) && x.Number == number);

        return new { available = seat.Status == "available" };
    }
}
```

Keep the synchronous action methods and the possible null dereference for a missing seat. Do not add `[Range]`, cancellation tokens, `IHttpClientFactory`, or typed error results.

- [ ] **Step 2: Implement controller-local feed loading and mapping**

Implement `LoadMap()` inside the same controller. It must:

1. Create `new HttpClient()` on every call.
2. Call `GetStringAsync(FeedUrl).Result`.
3. Deserialize `List<FeedItem>` with `PropertyNameCaseInsensitive = true` and `AllowTrailingCommas = true`.
4. Use the first feed item without checking for null or empty input.
5. Convert `0` to `available` and every other character to `booked`.
6. Convert Unix seconds with `DateTimeOffset.FromUnixTimeSeconds(long.Parse(...)).ToLocalTime().ToString("HH:mm")`.
7. Return a mutable `SeatMapResponse` with a mutable `List<SeatResponse>`.

The controller-local types should remain simple mutable classes:

```csharp
public class FeedItem
{
    public string Auditorium { get; set; }
    public string FilmTitle { get; set; }
    public string StartTime { get; set; }
    public Dictionary<string, string> SeatRows { get; set; }
}

public class SeatMapResponse
{
    public string Auditorium { get; set; }
    public string FilmTitle { get; set; }
    public string StartTime { get; set; }
    public List<SeatResponse> Seats { get; set; }
}

public class SeatResponse
{
    public string Row { get; set; }
    public int Number { get; set; }
    public string Status { get; set; }
}
```

- [ ] **Step 3: Compile the API implementation**

Run:

```bash
dotnet build candidate-solution/Cinema.Candidate.sln
```

Expected: build succeeds without adding test artifacts.

### Task 3: Add minimal candidate instructions

**Files:**
- Create: `candidate-solution/README.md`

**Interfaces:**
- Documents how to run the API and the two implemented routes.
- Does not claim that resilience, tests, adjacent-seat search, or production error handling are implemented.

- [ ] **Step 1: Write concise run instructions**

Document:

```bash
dotnet run --project candidate-solution/Cinema.Candidate/Cinema.Candidate.csproj
```

Document the routes `/api/seats` and `/api/seats/{row}/{number}` and include one example request for each. State that the service reads the exercise feed URL.

- [ ] **Step 2: Check documentation paths**

Run:

```bash
rg --files candidate-solution
```

Expected: only the solution, one project, `Program.cs`, one controller, project build output after compilation, and `README.md`; no test project or test source.

### Task 4: Smoke-check the assembled candidate

**Files:**
- No additional files.

**Interfaces:**
- The solution builds as a single ASP.NET Core project.
- The running process exposes the two declared routes when the upstream feed is reachable.

- [ ] **Step 1: Verify the solution inventory**

Run:

```bash
dotnet sln candidate-solution/Cinema.Candidate.sln list
find candidate-solution -type f \( -name '*Test*.cs' -o -name '*Tests*' \) -print
```

Expected: exactly one project is listed and the test-file command prints nothing.

- [ ] **Step 2: Start the API on a local HTTP URL**

Run the project with a disposable local URL:

```bash
dotnet run --project candidate-solution/Cinema.Candidate/Cinema.Candidate.csproj --no-build --urls http://127.0.0.1:5099
```

- [ ] **Step 3: Request the map endpoint**

In another shell, run:

```bash
curl --fail-with-body --max-time 10 http://127.0.0.1:5099/api/seats
```

Expected when the upstream feed is reachable: HTTP 200 with `auditorium`, `filmTitle`, `startTime`, and expanded `seats`. If the upstream is unavailable, record that runtime limitation without changing the intentionally weak error handling.

- [ ] **Step 4: Confirm final build and status**

Run:

```bash
dotnet build candidate-solution/Cinema.Candidate.sln
git status --short
```

Expected: the candidate solution builds, and unrelated pre-existing working-tree changes remain untouched.
