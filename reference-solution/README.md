# Cinema Seat Service – Technical Exercise

## Introduction

Welcome to the DataArt interview tech challenge. You'll join us for a 90-minute remote session, during which you'll share your screen and lead the implementation of a small HTTP service, while we support and collaborate with you throughout the process. Before you start, please read the guidelines below so you know what we're looking for 🙂

- **RESTful design** – follow the REST API principles for the architecture
- **Quality over completeness** – clean code, clear naming, sensible tests and iterative refactoring matter more than "finishing".
- **Think out loud**: we're interested in your reasoning as much as the code you type.

### Traits we love to see:

- idiomatic C#/.NET 10
- thoughtful API design
- automated tests (TDD or test‑after – your choice)
- graceful failure‑handling

It's an "open book" exercise – Google/Stack Overflow are all fine (but not AI). Any IDE or editor is welcome.

Have fun! Treat the interviewer like a teammate – feel free to ask questions or bounce ideas.

Good luck! 🚀

## Scenario

The cinema has one auditorium and is showing a single film at 19:00 tonight. Assume the URL sometimes delays or returns 404. Live seat availability is exposed via our "flaky" upstream API:

https://raw.githubusercontent.com/dataart-interview/interview-technical-exercise-dotnet/main/seatmap-example.json

The response looks like this:

```json
[{
  "auditorium": "Main-Hall",
  "filmTitle":  "Space Odyssey",
  "startTime":  "1753804800",
  "seatRows": {
    "A": "111111",
    "B": "110000",
    "C": "11111001",
    "D": "1111111011",
  }
}]
```

Each row string is read left → right.
- `0` = available
- `1` = booked

## Your Task

Create a REST‑style API that lets clients query seat availability.

| Requirement | Details |
| - | - |
| **Endpoint for retrieving a map** | Return the entire seat layout converted to an object‑per‑seat structure (see contract below). |
| **Endpoint for check one seat** | Return a status of one seat by passing a row and a seat number. |
| **RESTful** | Follow RESTful priciples when designing your API.  |
| **Resilience** | Implement some resilience techniques so callers remain responsive during transient failures. |
| **Testing** | Cover the core functionality with unit tests|

## Sample Response Contract

### Get Seat Plan

```json
{
  "auditorium": "Main-Hall",
  "filmTitle": "Space Odyssey",
  "startTime": "19:00",
  "seats": [
    { "row": "A", "number": 1, "status": "booked" },
    { "row": "A", "number": 2, "status": "booked" },
    { "row": "A", "number": 3, "status": "booked" },
    { "row": "B", "number": 1, "status": "booked" },
    { "row": "B", "number": 2, "status": "booked" },
    { "row": "B", "number": 3, "status": "available" }
  ]
}
```

### Check a seat (e.g. B3)

```json
{ "available": true }
```

## Extension Opportunities

If you complete the core requirements early, feel free to tackle any of these advanced tasks:
- In-memory cache: If the upstream call fails or times out, serve the most recent copy held in an in‑memory cache with a 3–5 s TTL.
- Adjacent‑pair finder: Scan each row to find the first sequence of at least minSeats adjacent and unoccupied seats. If such a block exists, return found: true along with the seat range (e.g. seats "B3–B4"). If none are found in any row, return `found: false`.
- Add some integration tests
- Provide a **docker-compose.yml** so we can `docker compose up`.

## Notes & Constraints

- Target .NET 10 (or latest LTS). Minimal APIs, MVC controllers or both — your choice. If neccessary, you can also clone a basic API from here: [Link](./NET10/)
- Hard-code the feed URL, but keep the design flexible for future endpoint replacement.
- No authentication logic required.
- Prioritise readability, separation of concerns and meaningful tests — 100% coverage is not required.
- Use any NuGet packages you'd normally reach for.

Happy coding!