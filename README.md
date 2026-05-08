# SwarmAgents Example Workspace

A working demo of the [SwarmAgents framework](https://github.com/PoliedroSoftware/swarm-agents) — agent-orchestration for Claude Code, applied to a real .NET + Angular stack. Everything ships in containers so a client can clone, run, and explore in two commands.

## What's inside

- **Backend** — .NET 8 Web API with Clean Architecture (Domain / Application / Infrastructure / Api), MediatR, FluentValidation, EF Core 8 over MySQL, Redis cache.
- **Frontend** — Angular consuming the backend through generated OpenAPI clients.
- **Workspace agents** — planner (Opus), orchestrator, contract-guardian, cross-impact-analyzer, security-reviewer, final-reviewer.
- **Contracts** — `contracts/api.openapi.yaml` is the source of truth synced between back and front.
- **Fully containerized** — `docker compose up` brings up MySQL, Redis and the API.

## Quickstart

```bash
git clone https://github.com/PoliedroSoftware/swarm-agents-example.git
cd swarm-agents-example
cp .env.example .env       # fill in ANTHROPIC_API_KEY at minimum
docker compose up -d
```

- API + Swagger: http://localhost:5010/swagger
- MySQL: localhost:3306 (user `swarm`, password from `.env`)
- Redis: localhost:6379

## Repo layout

```
projects/
├── backend/                  .NET 8 Web API — CRUD of Products
│   ├── SwarmDemo.Domain/
│   ├── SwarmDemo.Application/
│   ├── SwarmDemo.Infrastructure/
│   ├── SwarmDemo.Api/
│   ├── tests/                xUnit + Testcontainers
│   ├── Dockerfile
│   └── docker-compose.yml    MySQL + Redis + API
└── frontend/                 Angular client (Phase 6+)

contracts/
└── api.openapi.yaml          source of truth between back and front

.claude/                      SwarmAgents workspace agents + MCP settings

swarmagents.workspace.json    workspace manifest read by the framework
```

## How the SwarmAgents framework drives this workspace

The `workspace-planner` agent (Opus, public, in `.claude/agents/`) is the entry point. When you invoke it inside Claude Code with a request like *"add a /products/search endpoint"*, it:

1. Reads `swarmagents.workspace.json` to know which projects exist and their stacks.
2. Produces a typed plan (DAG of stages) and asks for approval if breaking.
3. Dispatches `backend/orchestrator` and `frontend/orchestrator` in parallel where possible.
4. After implementation, runs `sonar-analyst` + tests in parallel, then `qa-tester`, then `security-reviewer` + `final-reviewer`.
5. On PR merge: `postman-curator` updates the official Postman workspace.

See the framework repo for the full architecture and agent catalog.

## License

MIT — see [LICENSE](./LICENSE).
