# PizzaProject

![CI](https://github.com/kodlon/PizzaProject/actions/workflows/ci.yml/badge.svg)

A small REST API for managing pizzas, built with ASP.NET Core 8.

This is a learning project. It started from the official Microsoft Learn
[ASP.NET Core Web API tutorial](https://learn.microsoft.com/en-us/training/modules/build-web-api-aspnet-core/),
where data lives in an in-memory list. From there I extended it step by step with dependency
injection, a real database, JSON Patch, DTOs, integration tests, global error handling,
JWT authentication and CI.

## Tech stack

- **ASP.NET Core 8** Web API (controllers)
- **Entity Framework Core 8** with **SQLite** and migrations
- **JWT Bearer** authentication with role-based authorization
- **JSON Patch** (`application/json-patch+json`) via Newtonsoft
- **xUnit**, **WebApplicationFactory**, **NSubstitute** for integration tests
- **Swagger / OpenAPI** (Swashbuckle)
- **GitHub Actions** for CI

## What's implemented

- **CRUD + PATCH** for pizzas.
- **DTOs** separate the API contract from the EF entity. Clients cannot set the `Id`,
  and internal fields cannot leak into responses.
- **Validation** with data annotations. Invalid input returns `400` as ProblemDetails.
- **Safe JSON Patch.** The patch is applied to a copy and validated before anything is saved,
  so an invalid patch never corrupts stored data.
- **Global exception handling.** Unhandled exceptions are logged on the server, and the client
  gets a generic `500` ProblemDetails response with a `traceId`. No stack traces or exception
  messages are exposed.
- **Authentication and authorization.** Reading is public, writing requires a signed-in user,
  and deleting requires the `Admin` role (`401` vs `403`).
- **Integration tests** that run the whole app in memory against an in-memory SQLite database,
  with a test authentication scheme instead of real tokens.

## API

| Method | Route | Access | Success |
|---|---|---|---|
| `GET` | `/api/pizza` | Public | `200` |
| `GET` | `/api/pizza/{id}` | Public | `200` / `404` |
| `POST` | `/api/pizza` | Signed-in user | `201` |
| `PUT` | `/api/pizza/{id}` | Signed-in user | `204` / `404` |
| `PATCH` | `/api/pizza/{id}` | Signed-in user | `200` / `404` |
| `DELETE` | `/api/pizza/{id}` | `Admin` role | `204` / `404` |

## Getting started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- EF Core CLI tools: `dotnet tool install --global dotnet-ef`

### Run the API

```bash
git clone https://github.com/kodlon/PizzaProject.git
cd PizzaProject/src/PizzaProject

dotnet ef database update   # creates pizza.db with the schema
dotnet run --launch-profile https
```

Swagger UI is available at <https://localhost:7254/swagger>.

### Get a token for protected endpoints

For local development, tokens are issued by the built-in `user-jwts` tool
(run from `src/PizzaProject`):

```bash
dotnet user-jwts create --name alice                # regular user
dotnet user-jwts create --name admin --role Admin   # admin
```

Send the token in the `Authorization: Bearer <token>` header. Ready-made requests are in
[`PizzaProject.http`](src/PizzaProject/PizzaProject.http); put the tokens into
`http-client.private.env.json` next to it (this file is git-ignored).

## Running tests

From the repository root:

```bash
dotnet test tests/PizzaProject.Tests/PizzaProject.Tests.csproj
```

The tests need no database, tokens or configuration: each run uses an in-memory SQLite
database and a test authentication scheme.

## Project structure

```
src/PizzaProject/
  Controllers/      HTTP endpoints
  Data/             EF Core DbContext
  Dtos/             request/response contracts and mapping
  Infrastructure/   global exception handler
  Migrations/       EF Core migrations
  Models/           EF entities
  Services/         business logic over the DbContext
  Utils/            JSON Patch formatter setup
tests/PizzaProject.Tests/
  PizzaApiFactory.cs            test host: in-memory SQLite + test auth
  TestAuthenticationHandler.cs  test authentication scheme
  PizzaApiTests.cs              integration tests
```

## Possible next steps

- A real login endpoint and user store instead of development tokens
- An **Authorize** button in Swagger UI
- PostgreSQL with Docker Compose

## License

[MIT](LICENSE)
