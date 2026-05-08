# Frontend — Angular (Demo)

This is the SwarmAgents demo frontend. It will consume the backend's OpenAPI spec to generate API clients and provide a UI for `Products`.

## Status

**Phase 0** — directory scaffolded but no Angular code yet. The actual `ng new` runs in Phase 1 (more precisely Phase 2 / Phase 6, after the backend has a stable contract).

## Planned architecture (Phase 6+)

- Angular 20+ standalone components (no `NgModule`).
- Signal-based state: `signal()`, `computed()`, `linkedSignal()`. No NgRx unless complexity actually demands it.
- API client generated from `../../contracts/api.openapi.yaml` via `ng-openapi-gen`.
- Tailwind CSS for styling.
- Playwright for E2E.

## Conventions

- Standalone components only.
- Inputs/outputs use the signal API: `input()`, `output()`.
- Change detection: `OnPush` everywhere.
- One component = one feature responsibility (split if a component grows past ~150 lines).
- Tests: Jest for unit, Playwright for E2E.
- Commits follow Conventional Commits.

## Generated code

`src/app/api/` is **regenerated** from the OpenAPI spec by the `code-writer` agent on every contract change. Do not edit by hand — your changes will be lost on the next sync. If you need to extend behavior, add wrappers in `src/app/services/`.

## Agents

Defined in `.claude/agents/` (Phase 6+):

- `planner` (public, Opus) — entry point for frontend-only tasks.
- `orchestrator` (internal, Sonnet).
- `component-reviewer`, `code-writer`, `test-writer` (executors).
- `a11y-auditor` (WCAG 2.2 AA), `perf-auditor` (Core Web Vitals), `e2e-writer` (Playwright).
- `qa-tester` (public).
- `security-reviewer` (internal).
- `final-reviewer` (internal).

For cross-project work (e.g. requesting a backend change implied by a UI need), invoke the workspace-level `workspace-planner` instead.

## Local dev

```bash
# Phase 6+:
npm install
npm run gen:api    # regenerates src/app/api/ from ../../contracts/api.openapi.yaml
npm start          # ng serve
```
