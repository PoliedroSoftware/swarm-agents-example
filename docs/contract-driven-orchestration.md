# Contract-driven orchestration

This workspace is moving from a hard-coded `backend -> frontend` flow to a contract-driven flow based on named producers and consumers.

## Goal

The workspace planner should not assume there is only one backend and one frontend. It should read `swarmagents.workspace.json` and answer these questions:

1. Which project produces the changed contract?
2. Which named contract may change?
3. Which projects consume that contract?
4. Which project-specific planners should receive tasks?

## Current contract model

```jsonc
{
  "contracts": [
    {
      "name": "main-api",
      "type": "openapi",
      "producer": "backend",
      "path": "contracts/api.openapi.yaml",
      "snapshot": "contracts/api.openapi.snapshot.yaml",
      "consumers": ["frontend"]
    }
  ]
}
```

Projects refer to contracts by name:

```jsonc
{
  "name": "backend",
  "produces": ["main-api"]
}
```

```jsonc
{
  "name": "frontend",
  "consumes": ["main-api"]
}
```

## Adding a .NET MAUI consumer

A MAUI app can live under `projects/frontend/mobile-maui` or any other folder. The physical folder is less important than declaring it as a project and as a consumer of the named contract.

Example:

```jsonc
{
  "name": "mobile-app",
  "kind": "frontend",
  "path": "projects/frontend/mobile-maui",
  "stack": "dotnet",
  "framework": "maui",
  "profile": "maui-mvvm",
  "produces": [],
  "consumes": ["main-api"],
  "agents": [
    "planner",
    "orchestrator",
    "code-writer",
    "test-writer",
    "ui-reviewer",
    "security-reviewer",
    "final-reviewer"
  ]
}
```

Then add the project to the contract consumers:

```jsonc
{
  "name": "main-api",
  "consumers": ["frontend", "mobile-app"]
}
```

## Expected flow when a producer changes a contract

```text
workspace-planner
  -> backend/planner
  -> contract-guardian
  -> cross-impact-analyzer
  -> frontend/planner
  -> mobile-app/planner
  -> security-reviewer
  -> workspace-final-reviewer
```

The workspace orchestrator coordinates the plan. It should not implement Angular, MAUI, or backend details directly. Project-specific planners and code writers own stack-specific implementation.

## Plan-first guardrail

Workspace plans must not dispatch `code-writer` directly. A code writer is only allowed behind a project planner/orchestrator and must receive an assigned task id.

Valid:

```jsonc
{ "agents": ["backend/planner"] }
```

Invalid:

```jsonc
{ "agents": ["backend/code-writer"] }
```

This keeps the system aligned with the rule: plan first, then implement, then review.
