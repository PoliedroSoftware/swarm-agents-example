# Frontend — Angular (Demo)

This is the SwarmAgents demo frontend. It consumes the backend's OpenAPI spec to provide a UI for `Products`.

## Status

**Phase 5** — Angular 21 project with standalone components, signals, Tailwind CSS, proxy to backend API. All 11 frontend agents defined.

## Architecture

- Angular 21 standalone components (no `NgModule`).
- Signal-based state: `signal()`, `computed()`. No NgRx.
- API calls via `HttpClient` with `async/await` wrapped in `ProductsService`.
- Tailwind CSS v4 for styling (utility-first).
- Lazy-loaded pages via `loadComponent()`.
- Change detection: `OnPush` everywhere.

## Project structure

```
src/app/
├── app.ts / app.html / app.css     # Root component (nav + router-outlet)
├── app.config.ts                   # DI providers (http, router)
├── app.routes.ts                   # Lazy routes → /products, /products/new, /products/:id/edit
├── models/
│   └── product.model.ts            # TypeScript interfaces from OpenAPI
├── services/
│   └── products.service.ts         # Signal-based state + CRUD via HttpClient
└── pages/
    ├── product-list/               # Paginated product table
    │   ├── product-list.component.ts
    │   └── product-list.component.html
    └── product-form/               # Create/edit form with validation
        ├── product-form.component.ts
        └── product-form.component.html
```

## Conventions

- Standalone components only.
- Change detection: `OnPush` everywhere.
- State: `signal()` for internal state, `computed()` for derivations.
- HTTP: Services use `firstValueFrom(http.get/post...)` with `async/await`.
- Forms: `ReactiveFormsModule` with `FormBuilder` and inline validators.
- Tailwind: utility classes, no custom CSS files beyond `styles.css` import.
- Tests: vitest for unit, Playwright for E2E.
- Commits follow Conventional Commits.

## Generated code

`src/app/api/` is generated from `../../contracts/api.openapi.yaml` via `npm run gen:api`. Do not edit by hand — wrap in `src/app/services/` if extension is needed.

## Agents

Defined in `.claude/agents/` (11 agents):

| Agent | Visibility | Model | Role |
|-------|-----------|-------|------|
| `planner` | public | opus | Entry point for frontend-only tasks |
| `orchestrator` | internal | sonnet | Dispatches frontend plan stages |
| `code-writer` | internal | sonnet | Implements Angular components/services |
| `test-writer` | public | sonnet | Writes vitest unit tests |
| `component-reviewer` | public | opus | Reviews Angular best practices |
| `a11y-auditor` | public | sonnet | WCAG 2.2 AA compliance |
| `perf-auditor` | public | sonnet | Core Web Vitals + bundle size |
| `e2e-writer` | public | sonnet | Playwright E2E tests |
| `qa-tester` | public | sonnet | Exploratory QA |
| `security-reviewer` | public | opus | XSS, CSP, dependency audit |
| `final-reviewer` | internal | opus | Sign-off gate |

For cross-project work, invoke `workspace-planner` at workspace level.

## Local dev

```bash
npm install
npm run gen:api   # generate API client from contracts/api.openapi.yaml
npm start         # ng serve with proxy to backend (localhost:5010)
npm run build     # production build
npm test          # vitest unit tests
```
