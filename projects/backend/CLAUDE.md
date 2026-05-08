# Backend — .NET 8 Web API (Demo)

This is the SwarmAgents demo backend. It will host a CRUD API for `Products` against MySQL, exercising every backend agent in the framework.

## Status

**Phase 0** — directory scaffolded but no .NET code yet. The actual `dotnet new webapi` runs in Phase 1.

## Planned architecture (Phase 1+)

Clean Architecture, four projects in one solution:

```
SwarmDemo.sln
├── SwarmDemo.Domain/            entities, value objects, domain events. Zero infra dependencies.
├── SwarmDemo.Application/       use cases (CQRS-style commands/queries), interfaces (ports)
├── SwarmDemo.Infrastructure/    EF Core, MySQL provider, external services (adapters)
└── SwarmDemo.Api/               controllers, DI composition, OpenAPI exposure
```

## Conventions

- All business logic lives in `Application`. Controllers are thin pass-throughs.
- `Domain` references nothing external. No NuGets except primitives.
- `Infrastructure` implements interfaces declared in `Application` (Dependency Inversion).
- API exposes OpenAPI via Swashbuckle. The spec is exported to `../../contracts/api.openapi.yaml` after every API change (the `code-writer` agent does this; `contract-guardian` validates).
- Tests: xUnit for unit tests, `WebApplicationFactory` for integration tests against a real MySQL (TestContainers).
- Commits follow Conventional Commits.

## Database

MySQL. Connection string is built from env vars `MYSQL_HOST`, `MYSQL_PORT`, `MYSQL_USER`, `MYSQL_PASSWORD`, `MYSQL_DATABASE` (see root `.env.example`). EF Core migrations live in `SwarmDemo.Infrastructure/Migrations/`. The `db-migrator` agent owns migration creation and execution.

## Agents

Defined in `.claude/agents/` (Phase 1+):

- `planner` (public, Opus) — entry point for backend-only tasks.
- `orchestrator` (internal, Sonnet) — dispatches the DAG.
- `architect-reviewer`, `code-writer`, `test-writer`, `db-migrator` (executors).
- `postman-curator` (internal, runs only on PR merge).
- `sonar-analyst`, `jmeter-runner` (internal, runs in parallel with tests).
- `qa-tester` (public).
- `security-reviewer` (internal, can block independently).
- `final-reviewer` (internal, runs last).

For cross-project work (anything that may touch the frontend), invoke the workspace-level `workspace-planner` instead.

## Local dev

```bash
# Phase 1+:
dotnet restore
dotnet ef database update --project SwarmDemo.Infrastructure --startup-project SwarmDemo.Api
dotnet run --project SwarmDemo.Api
```
