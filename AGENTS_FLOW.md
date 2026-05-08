# SwarmAgents — Flujo de Agentes

## 1. Jerarquía de carpetas

```
swarm-agents-example/                         ← WORKSPACE ROOT
│
├── .claude/                                  ← RUNTIME
│   ├── agents/                               ← 6 workspace-level agents
│   │   ├── 🔵 workspace-planner.md
│   │   ├── ⬛ workspace-orchestrator.md
│   │   ├── 🟠 contract-guardian.md
│   │   ├── 🟠 cross-impact-analyzer.md
│   │   ├── 🔴 security-reviewer.md
│   │   └── 🟣 workspace-final-reviewer.md
│   │
│   ├── skills/                               ← RUNTIME SKILLS
│   │   ├── swarm-dispatch/SKILL.md           ← Agent loader + Task dispatch
│   │   └── swarm-orchestrate/SKILL.md        ← DAG executor + parallel dispatch
│   │
│   └── commands/                             ← USER ENTRY POINTS
│       ├── swarm-plan.md                     ← /swarm-plan
│       ├── swarm-run.md                      ← /swarm-run
│       ├── swarm-approve.md                  ← /swarm-approve
│       └── swarm-status.md                   ← /swarm-status
│
├── contracts/                                ← CROSS-PROJECT TRUTH
│   ├── api.openapi.yaml                      ← Generated from backend
│   └── api.openapi.snapshot.yaml             ← Last accepted state
│
├── skills/                                   ← TECH STACK SKILLS (separate repo)
│   ├── README.md
│   ├── dotnet/SKILL.md                       ← .NET patterns (300+ lines)
│   ├── angular/SKILL.md                      ← Angular patterns (300+ lines)
│   ├── java-springboot/PLACEHOLDER.md
│   └── react/PLACEHOLDER.md
│
├── projects/
│   ├── backend/                              ← .NET 8 PROJECT
│   │   ├── .claude/agents/                   ← 12 backend agents
│   │   │   ├── 🔵 planner.md
│   │   │   ├── ⬛ orchestrator.md
│   │   │   ├── 🟢 code-writer.md
│   │   │   ├── 🟡 test-writer.md
│   │   │   ├── 🟣 architect-reviewer.md
│   │   │   ├── 🟣 final-reviewer.md
│   │   │   ├── 🔷 db-migrator.md
│   │   │   ├── 🔷 postman-curator.md
│   │   │   ├── 🟠 sonar-analyst.md
│   │   │   ├── ⚪ jmeter-runner.md
│   │   │   ├── ⚪ qa-tester.md
│   │   │   └── 🔴 security-reviewer.md
│   │   │
│   │   ├── SwarmDemo.Domain/
│   │   ├── SwarmDemo.Application/
│   │   ├── SwarmDemo.Infrastructure/
│   │   ├── SwarmDemo.Api/
│   │   └── tests/
│   │
│   └── frontend/                             ← ANGULAR 21 PROJECT
│       ├── .claude/agents/                   ← 11 frontend agents
│       │   ├── 🔵 planner.md
│       │   ├── ⬛ orchestrator.md
│       │   ├── 🟢 code-writer.md
│       │   ├── 🟡 test-writer.md
│       │   ├── 🟣 component-reviewer.md
│       │   ├── 🟣 final-reviewer.md
│       │   ├── 🟠 a11y-auditor.md
│       │   ├── 🟠 perf-auditor.md
│       │   ├── 🟡 e2e-writer.md
│       │   ├── ⚪ qa-tester.md
│       │   └── 🔴 security-reviewer.md
│       │
│       └── src/app/
│           ├── models/
│           ├── services/
│           └── pages/
│
├── docker-compose.yml                       ← MySQL + Redis + API
├── swarmagents.workspace.json               ← Manifest
└── .swarm-reports/                          ← Plan + execution artifacts
```

## 2. Flujo completo: desde el usuario hasta el PR

```
                         ┌─────────────┐
                         │   USUARIO   │
                         └──────┬──────┘
                                │
                    /swarm-plan "add discount to Product"
                                │
                                ▼
┌──────────────────────────────────────────────────────────────────────────────┐
│  🔵 workspace-planner (Opus, public)                                       │
│                                                                            │
│  1. Lee swarmagents.workspace.json                                         │
│  2. Lee CLAUDE.md (workspace + projects)                                   │
│  3. Lee git diff                                                           │
│  4. Classifica: additive | breaking | risky | trivial                      │
│  5. Construye plan.json (DAG de stages)                                    │
│  6. Persiste → .swarm-reports/{ts}/plan.json                               │
│  7. Muestra resumen al usuario: "Reply 'go' to proceed"                    │
└──────────────────────────────────────────────────────────────────────────────┘
                                │
                         /swarm-run
                                │
                                ▼
┌──────────────────────────────────────────────────────────────────────────────┐
│  ⬛ workspace-orchestrator (Sonnet, internal)                               │
│                                                                            │
│  1. Carga swarm-dispatch skill                                             │
│  2. Carga swarm-orchestrate skill                                          │
│  3. Lee plan.json                                                          │
│  4. Topological sort de stages                                             │
│  5. EJECUTA EL DAG — stages en paralelo donde el plan lo permite           │
└──────────────────────────────────────────────────────────────────────────────┘
                                │
          ┌─────────────────────┼─────────────────────────┐
          ▼                     ▼                         ▼
     ┌─────────┐          ┌──────────┐            ┌──────────────┐
     │  S1     │          │  S2      │            │  S3          │
     │  impl   │          │  contract│            │  impact      │
     └────┬────┘          └────┬─────┘            └──────┬───────┘
          │                    │                         │
          ▼                    │                         │
┌─────────────────────┐       │                         │
│ ⬛ backend/orchestrator│     │                         │
└──────────┬──────────┘       │                         │
           │                  │                         │
    ┌──────┼──────┐           │                         │
    ▼      ▼      ▼           │                         │
  ┌────┐┌────┐┌────┐         │                         │
  │🟢  ││🟡  ││🔷  │         │                         │
  │code││test││db  │         │                         │
  │writ││writ││migr│         │                         │
  └──┬─┘└──┬─┘└──┬─┘         │                         │
     │     │     │            ▼                         │
     │     │     │    ┌──────────────┐                  │
     │     │     │    │🟠 contract-   │                  │
     ▼     ▼     ▼    │  guardian    │                  │
  ┌─────────────────┐ │  Diffs spec  │                  │
  │ 🟣 architect-    │ │  vs snapshot │                  │
  │    reviewer     │ │  → additive  │                  │
  └────────┬────────┘ └──────┬───────┘                  │
           │                 │                          │
           ▼                 ▼                          │
  ┌─────────────────┐ ┌─────────────────┐              │
  │ 🟣 backend       │ │ 🟠 cross-impact  │              │
  │   final-reviewer│ │   analyzer      │              │
  │ ✓ / ✗ verdict   │ │ → tasks for FE  │              │
  └────────┬────────┘ └────────┬────────┘              │
           │                   │                        │
           └───────┬───────────┘                        │
                   │                                    │
                   ▼                                    ▼
          ┌──────────────┐                     ┌──────────────┐
          │  S4 FE impl  │                     │  S5          │
          │  ⬛ frontend/ │                     │  security    │
          │  orchestrator│                     │  🔴 reviewer │
          └──────┬───────┘                     └──────┬───────┘
                 │                                    │
          ┌──────┼──────┐                             │
          ▼      ▼      ▼                             │
        ┌────┐┌────┐┌────┐                           │
        │🟢  ││🟡  ││🟣  │                           │
        │code││test││comp│                           │
        │writ││writ││rev │                           │
        └──┬─┘└──┬─┘└──┬─┘                           │
           │     │     │                              │
           ▼     ▼     ▼                              │
        ┌────────────────┐                           │
        │ 🟣 frontend     │                           │
        │   final-review │                           │
        └───────┬────────┘                           │
                │                                    │
                └──────────────┬─────────────────────┘
                               │
                               ▼
                    ┌─────────────────────┐
                    │ S6 final            │
                    │ 🟣 workspace-final- │
                    │    reviewer         │
                    │                     │
                    │ Sintetiza:          │
                    │  - security report  │
                    │  - contract change  │
                    │  - backend review   │
                    │  - frontend review  │
                    │                     │
                    │ → ✓ READY TO PR     │
                    │ → ✗ BACK TO PLANNER │
                    └──────────┬──────────┘
                               │
                               ▼
                         ┌─────────┐
                         │  PR     │
                         │  OPEN   │
                         └─────────┘
```

## 3. Flujo de dispatch paralelo dentro de un stage

```
┌─────────────────────────────────────────────────────────────────┐
│  ⬛ orchestrator — Stage S2-contract (3 agents in parallel)     │
│                                                                 │
│  ┌─ Task(🟢 backend/code-writer: S2-impl, prompt=...) ────────┐│
│  │  Lee agent .md → parse frontmatter → dispatch via Task      ││
│  │  Escribe Domain/Application/Infrastructure/Api layers       ││
│  └─────────────────────────────────────────────────────────────┘│
│                                                                 │
│  ┌─ Task(🟡 backend/test-writer: S2-impl, prompt=...) ────────┐│
│  │  Lee agent .md → parse frontmatter → dispatch via Task      ││
│  │  Escribe xUnit tests (Domain + App + Integration)           ││
│  └─────────────────────────────────────────────────────────────┘│
│                                                                 │
│  ┌─ Task(🟣 architect-reviewer: S2-impl, prompt=...) ─────────┐│
│  │  Lee agent .md → parse frontmatter → dispatch via Task      ││
│  │  Revisa capas, SOLID, Clean Architecture                   ││
│  └─────────────────────────────────────────────────────────────┘│
│                                                                 │
│  ▸ Los 3 agentes corren CONCURRENTEMENTE                       │
│  ▸ El orquestador espera a que TODOS terminen                  │
│  ▸ Luego avanza al siguiente stage                             │
└─────────────────────────────────────────────────────────────────┘
```

## 4. Flujo de contratos (cross-project sync)

```
┌──────────────────┐
│  Backend cambia  │
│  API surface     │
└────────┬─────────┘
         │
         ▼
┌────────────────────┐
│ 🟢 code-writer     │
│ Regenera OpenAPI   │
│ dotnet swagger →   │
│ contracts/         │
│ api.openapi.yaml   │
└────────┬───────────┘
         │
         ▼
┌────────────────────────┐
│ 🟠 contract-guardian   │
│ Diff spec vs snapshot  │
│ → additive | breaking  │
│ → contract-change.json │
└────────┬───────────────┘
         │
         ▼
┌──────────────────────────┐
│ 🟠 cross-impact-analyzer │
│ Traduce cambios a        │
│ tareas para consumidores │
│ → frontend:              │
│   - regen API client     │
│   - update ProductForm   │
│ → cross-impact-tasks.json│
└────────┬─────────────────┘
         │
         ▼
┌────────────────────┐
│ ⬛ frontend/        │
│   orchestrator     │
│ Ejecuta tareas FE  │
└────────────────────┘
```

## 5. Flujo de skills (carga de conocimiento de stack)

```
┌─────────────────────┐
│ Agente necesita     │
│ escribir código     │
│ .NET / Angular      │
└────────┬────────────┘
         │
         ▼
┌─────────────────────────┐
│ Carga skill relevante   │
│ skill("dotnet-backend") │
│ o                       │
│ skill("angular-frontend")│
└────────┬────────────────┘
         │
         ▼
┌──────────────────────────────┐
│ Skill inyecta conocimiento:  │
│  - Patrones de arquitectura  │
│  - Convenciones de código    │
│  - Estructura de archivos    │
│  - Comandos CLI              │
│  - Testing patterns          │
│  - Docker / deploy           │
└────────┬─────────────────────┘
         │
         ▼
┌────────────────────┐
│ Agente aplica      │
│ patrones del skill │
│ al escribir código │
└────────────────────┘
```

## 6. Agentes por color/emoji

| Color | Emoji | Agent Type | Workspace | Backend | Frontend | Total |
|-------|-------|-----------|-----------|---------|----------|-------|
| Azul | 🔵 | Planners | 1 | 1 | 1 | 3 |
| Gris | ⬛ | Orchestrators | 1 | 1 | 1 | 3 |
| Verde | 🟢 | Code Writers | - | 1 | 1 | 2 |
| Amarillo | 🟡 | Test Writers | - | 1 | 2 | 3 |
| Púrpura | 🟣 | Reviewers | 1 | 2 | 2 | 5 |
| Rojo | 🔴 | Security | 1 | 1 | 1 | 3 |
| Naranja | 🟠 | Analysts | 2 | 1 | 2 | 5 |
| Blanco | ⚪ | QA / Perf | - | 2 | 1 | 3 |
| Cyan | 🔷 | Infrastructure | - | 2 | - | 2 |
| | | **TOTAL** | **6** | **12** | **11** | **29** |
