# SwarmAgents Example Workspace

This is a **demo workspace** that exercises the [SwarmAgents framework](https://github.com/PoliedroSoftware/swarm-agents) on a real .NET + Angular stack.

## Architecture in one paragraph

A workspace-level **planner** (Opus) receives any request and produces a typed plan. A workspace-level **orchestrator** (Sonnet) dispatches to project-level orchestrators (`backend/orchestrator`, `frontend/orchestrator`) and to cross-cutting agents (`contract-guardian`, `cross-impact-analyzer`, `security-reviewer`). Each project's executors do the actual work; **reviewers** (Opus) sign off. Cross-project sync (e.g. backend API change → frontend service regeneration) flows through `contracts/api.openapi.yaml`.

## Repo layout

- `projects/backend/` — .NET 8 Web API (CRUD of Products against MySQL, Redis cache, Clean Architecture).
- `projects/frontend/` — Angular app consuming the backend.
- `contracts/` — OpenAPI spec, source of truth between backend and frontend.
- `.claude/` — workspace-level SwarmAgents agents, skills, MCP settings.
- `docker-compose.yml` — MySQL + Redis + API for local development and client demos.

## Conventions

- **Plan-first**: never write code without an approved plan from a planner agent. Executors only run after a plan exists.
- **Visibility**: agents declare `visibility: public | internal` in frontmatter. The runtime refuses direct user invocation of internal agents.
- **Memory**: only planners and reviewers write to `.claude/memory/`. Executors read.
- **Contracts**: backend regenerates `contracts/api.openapi.yaml` after any API change; `contract-guardian` diffs against the snapshot; `cross-impact-analyzer` translates changes into frontend tasks.
- **Breaking changes**: pause-and-ask the user before propagating across projects.
- **Postman**: collection updates are preview-only during dev. Official workspace updates run only on `github.pr.merged`.
- **Sonar**: runs in parallel with tests, before any reviewer agent.
- **Security**: dedicated `security-reviewer` agent — separate from `final-reviewer`. Can block on its own.
- **Containers everywhere**: anything a client might run locally must be reachable via `docker compose up`. Tests use Testcontainers; nothing assumes a host-installed MySQL or Redis.

## Where to start

- For backend work: `cd projects/backend`, see its `CLAUDE.md`.
- For frontend work: `cd projects/frontend`, see its `CLAUDE.md`.
- For end-to-end / cross-cutting work: invoke the `workspace-planner` agent.

## Environment

Copy `.env.example` to `.env` and fill in API keys. `.claude/settings.json` references env vars via `${VAR}` substitution to wire MCP servers. Env vars must be present in the shell that launches Claude Code, or sourced via `docker compose --env-file .env`.
