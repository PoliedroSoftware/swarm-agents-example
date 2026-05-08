# SwarmAgents Example Workspace

A working demo of the [SwarmAgents framework](https://github.com/PoliedroSoftware/swarm-agents) — agent-orchestration for Claude Code, applied to a real .NET + Angular stack. Everything ships in containers so a client can clone, run, and explore in two commands.

## What's inside

- **Backend** — .NET 8 Web API with Clean Architecture (Domain / Application / Infrastructure / Api), MediatR, FluentValidation, EF Core 8 over MySQL, Redis cache.
- **Frontend** — Angular 21 consuming the backend through generated OpenAPI clients. Standalone components, signals, Tailwind CSS.
- **Workspace agents** — planner (Opus), orchestrator, contract-guardian, cross-impact-analyzer, security-reviewer, final-reviewer.
- **Contracts** — `contracts/api.openapi.yaml` is the source of truth synced between back and front.
- **Fully containerized** — `docker compose up` brings up MySQL, Redis, API, and frontend (nginx).

## Quickstart

```bash
git clone https://github.com/PoliedroSoftware/swarm-agents-example.git
cd swarm-agents-example
cp .env.example .env       # fill in ANTHROPIC_API_KEY and GITHUB_TOKEN at minimum
docker compose up -d
```

- Frontend: http://localhost:4200
- API + Swagger: http://localhost:5010/swagger
- MySQL: localhost:3306 (user `swarm`, password from `.env`)
- Redis: localhost:6379

## Claude Code GitHub Integration

Claude Code can manage PRs, issues, and commits when configured with a GitHub token.

### Setup

1. Add `GITHUB_TOKEN` to your `.env` file (classic token with `repo`, `issues`, `pull_requests` scopes).
2. Claude Code reads `.claude/settings.json` which wires `GITHUB_TOKEN` into the GitHub MCP server.
3. The `gh` CLI is permitted via `Bash(gh *)` in settings.

### Capabilities

| Action | Command |
|--------|---------|
| Create issue | `gh issue create --title "..." --body "..."` |
| List PRs | `gh pr list` |
| Review PR | `gh pr review {N} --approve|--comment|--request-changes` |
| Add labels | `gh issue edit {N} --add-label "breaking"` |
| Post comment | `gh issue comment {N} --body "..."` |
| Create branch | `gh pr checkout {N}` |
| View diff | `gh pr diff {N}` |
| Merge PR | `gh pr merge {N} --squash` |

### SwarmAgents workflow with GitHub

```
Developer opens PR
  └─ GitHub Action "Claude Code PR Manager"
       ├─ Analyzes changed files
       ├─ Classifies: additive | breaking | risky
       ├─ Posts review comment with findings
       └─ Adds auto-reviewed label

Developer comments: /swarm-plan "Add feature X"
  └─ workspace-planner creates plan, posts as issue comment

Developer comments: /swarm-run
  └─ Orchestrator dispatches agents, posts results

CI completes: tests pass + docker images pushed to GHCR
  └─ PR ready to merge
```

## GitHub Actions CI/CD

| Workflow | Trigger | Actions |
|----------|---------|---------|
| `backend-ci.yml` | Push/PR to `projects/backend/**` | Build, test, docker push to GHCR, regenerate OpenAPI |
| `frontend-ci.yml` | Push/PR to `projects/frontend/**` | Build, test, docker push to GHCR, deploy to Vercel |
| `claude-pr-manager.yml` | PR opened/synced, Issue opened | Analyze PR, classify changes, triage issues, post comments |

## Docker Registry (GHCR)

Images are pushed to GitHub Container Registry:
- `ghcr.io/poliedrosoftware/swarm-api:latest`
- `ghcr.io/poliedrosoftware/swarm-frontend:latest`

Pull and run:
```bash
docker pull ghcr.io/poliedrosoftware/swarm-api:latest
docker pull ghcr.io/poliedrosoftware/swarm-frontend:latest
```

## Repo layout

```
projects/
├── backend/                  .NET 8 Web API — CRUD of Products
│   ├── SwarmDemo.Domain/
│   ├── SwarmDemo.Application/
│   ├── SwarmDemo.Infrastructure/
│   ├── SwarmDemo.Api/
│   ├── tests/                xUnit + Testcontainers
│   └── Dockerfile
└── frontend/                 Angular 21 client
    ├── src/app/
    ├── Dockerfile
    └── nginx.conf

contracts/
├── api.openapi.yaml          source of truth between back and front
└── api.openapi.snapshot.yaml last accepted state

.claude/                      SwarmAgents runtime + agents + settings
.github/workflows/            CI/CD pipelines
skills/                       Technology skill packs (separate repo)

swarmagents.workspace.json    workspace manifest read by the framework
```

## License

MIT — see [LICENSE](./LICENSE).
