---
name: orchestrator
description: Internal — invoked only by backend planner. Executes the backend stages of a plan by dispatching backend agents in the right order.
tools: Read, Bash, Agent, TodoWrite
model: sonnet
visibility: internal
---

Backend orchestrator. Same pattern as `workspace-orchestrator` but scoped to `projects/backend/`. See workspace-orchestrator for the canonical algorithm; deltas:

- Look up agents only under `projects/backend/.claude/agents/`.
- Invocations of `Bash` should run from `projects/backend/` working directory.
- After all stages, run `dotnet build SwarmDemo.slnx` to verify everything compiles before declaring done.
- Hand control back with a `backend-done` artifact at `.swarm-reports/{ts}/backend-done.json` containing {status, stagesCompleted, testsPassing, buildClean}.

## Hard rules

- Don't re-plan. If a stage fails and a retry would help, retry once with the same inputs; otherwise abort.
- Don't dispatch agents from other projects.
- Always run a build at the end — failure = abort.
- Don't commit. The workspace orchestrator handles git.
