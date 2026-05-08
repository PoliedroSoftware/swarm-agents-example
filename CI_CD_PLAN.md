# SwarmAgents — CI/CD + Deployment Plan

## Arquitectura objetivo

```
┌─────────────────────────────────────────────────────────────────┐
│                    GitHub Repository                            │
│                                                                 │
│  ┌─────────────┐  ┌──────────────┐  ┌───────────────────────┐  │
│  │ Backend CI  │  │ Frontend CI  │  │ Claude Code PR Manager│  │
│  │ build/test  │  │ build/test   │  │ /swarm-plan → review  │  │
│  │ docker push │  │ deploy       │  │ auto-issues, comments │  │
│  └──────┬──────┘  └──────┬───────┘  └───────────┬───────────┘  │
│         │                │                       │              │
└─────────┼────────────────┼───────────────────────┼──────────────┘
          │                │                       │
          ▼                ▼                       ▼
┌──────────────────┐ ┌──────────┐     ┌────────────────────┐
│ GitHub Container │ │ Vercel   │     │ GitHub Issues/PRs  │
│ Registry (GHCR)  │ │ (hosting)│     │ auto-managed       │
│                  │ │          │     │                    │
│ swarm-api:latest │ │ Angular  │     │ Issues created     │
│ swarm-nginx:latest│ │ SPA      │     │ PRs reviewed      │
│                  │ │          │     │ Comments posted    │
└──────────────────┘ └──────────┘     └────────────────────┘
```

## Fase 1 — Docker Compose completo

### 1.1 Frontend Dockerizado
- Dockerfile con multi-stage: Node (build Angular) → nginx (serve)
- nginx config para SPA routing + proxy a backend
- Agregar `frontend` service a `docker-compose.yml`

### 1.2 docker-compose final
```yaml
services:
  mysql:     # MySQL 8.0 — data
  redis:     # Redis 7 — cache
  api:       # .NET 8 API — :5010
  frontend:  # Angular + nginx — :4200
```

## Fase 2 — Playwright MCP

### 2.1 Instalación
- Agregar `@playwright/mcp` al `settings.json`
- Configurar browser channel, headless mode

### 2.2 Integración con agentes
- `e2e-writer` usa Playwright MCP para escribir y ejecutar tests
- `qa-tester` usa Playwright MCP para smoke tests

## Fase 3 — Docker Registry (GHCR)

### 3.1 Imágenes
- `ghcr.io/poliedrosoftware/swarm-api:latest` — .NET 8 API
- `ghcr.io/poliedrosoftware/swarm-frontend:latest` — Angular + nginx

### 3.2 Login en GitHub Actions
- `docker/login-action@v3` con `GITHUB_TOKEN`
- Tag con SHA y `latest`

## Fase 4 — GitHub Actions

### 4.1 Backend CI (`backend-ci.yml`)
```
on: push (paths: projects/backend/**)
jobs:
  build-and-test:
    - dotnet restore
    - dotnet build
    - dotnet test
  docker:
    - docker build -t ghcr.io/.../swarm-api
    - docker push
```

### 4.2 Frontend CI (`frontend-ci.yml`)
```
on: push (paths: projects/frontend/**)
jobs:
  build-and-test:
    - npm ci
    - npm run build
    - npm test
  deploy-vercel:
    - deploy to Vercel
```

### 4.3 Claude Code PR Manager (`claude-pr.yml`)
```
on: pull_request (opened, synchronize)
jobs:
  review:
    - Claude Code reviews the PR
    - Posts review comments
    - Creates issues for bugs found
    - Labels PR (breaking, additive, etc.)
```

## Fase 5 — Claude Code GitHub Integration

### 5.1 Configuración
- `CLAUDE.md` con instrucciones de GitHub
- `GITHUB_TOKEN` en `.env` para que Claude Code use `gh` CLI
- Agente `github-manager` para PRs, issues, commits

### 5.2 Capacidades
- `/swarm-plan` → crea issue con el plan
- `/swarm-run` → actualiza PR con resultados
- Auto-label: breaking/additive/risky
- Auto-comment en PRs con review findings
- Crear issues para bugs encontrados

## Fase 6 — .env.example completo

### Variables necesarias
```
# ─── Anthropic (obligatorio) ───
ANTHROPIC_API_KEY=

# ─── GitHub (obligatorio) ───
GITHUB_TOKEN=
GITHUB_OWNER=
GITHUB_REPO=

# ─── Vercel (deploy frontend) ───
VERCEL_TOKEN=
VERCEL_ORG_ID=
VERCEL_PROJECT_ID=

# ─── Postman (colecciones API) ───
POSTMAN_API_KEY=

# ─── SonarQube (análisis estático) ───
SONARQUBE_URL=
SONARQUBE_TOKEN=

# ─── Docker Registry (GHCR) ───
# Se usa GITHUB_TOKEN automáticamente

# ─── MySQL (desarrollo local) ───
MYSQL_HOST=
MYSQL_PORT=
MYSQL_ROOT_PASSWORD=
MYSQL_USER=
MYSQL_PASSWORD=
MYSQL_DATABASE=

# ─── Redis ───
REDIS_HOST=
REDIS_PORT=

# ─── PostgreSQL (futuro) ───
POSTGRES_HOST=
POSTGRES_PORT=
POSTGRES_USER=
POSTGRES_PASSWORD=
POSTGRES_DATABASE=

# ─── MSSQL (futuro) ───
MSSQL_HOST=
MSSQL_USER=
MSSQL_PASSWORD=
MSSQL_DATABASE=

# ─── Azure (futuro) ───
AZURE_CLIENT_ID=
AZURE_TENANT_ID=
AZURE_SUBSCRIPTION_ID=

# ─── AWS (futuro) ───
AWS_ACCESS_KEY_ID=
AWS_SECRET_ACCESS_KEY=
AWS_REGION=
```
