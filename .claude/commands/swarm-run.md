---
description: Execute a SwarmAgents plan — dispatches agents stage by stage according to the plan DAG
arguments:
  - name: plan_id
    description: Optional plan ID or timestamp. If omitted, runs the latest plan found in .swarm-reports/
    required: false
---

Execute the SwarmAgents orchestration runtime for a plan.

## Step 1 — Find the plan

1. If `$plan_id` is provided, read `.swarm-reports/$plan_id/plan.json`.
2. If omitted, list directories in `.swarm-reports/`, find the most recent one containing `plan.json`, and read it.
3. If no plan is found: `No plan found. Run /swarm-plan first.`

## Step 2 — Load the orchestrator

Load the `swarm-orchestrate` skill — it contains the full execution algorithm. Follow its instructions exactly.

Use the skill tool: load `swarm-orchestrate`.

## Step 3 — Execute the plan

Follow the swarm-orchestrate algorithm:
1. Pre-flight checks (approval gates at plan level).
2. Topological sort of stages.
3. For each stage batch (same dependency level):
   - Dispatch all agents in the stage concurrently via `Task` tool.
   - For each agent dispatch, use the `swarm-dispatch` pattern:
     a. Read the agent's `.md` file.
     b. Parse frontmatter, check visibility.
     c. Build the Task prompt: agent body + stage task.
     d. Call `Task(subagent_type="general", description="...", prompt="...")`.
   - Collect results. Handle failures per plan classification.
4. After each stage, persist execution log to `.swarm-reports/{plan-id}/execution.json`.
5. After all stages, emit final summary.

## Step 4 — Handle completion

- If plan has `onMerge` hooks, note them (they fire on github.pr.merged, not now).
- Show stage-by-stage status.
- If any stage failed on a breaking/risky plan, tell the user what to fix.

## Agent dispatch reference

When dispatching agents, resolve names as follows:

| Agent name | File path |
|---|---|
| `workspace-planner`, `contract-guardian`, `cross-impact-analyzer`, `security-reviewer`, `workspace-final-reviewer` | `.claude/agents/{name}.md` |
| `backend/planner`, `backend/code-writer`, `backend/test-writer`, etc. | `projects/backend/.claude/agents/{rest}.md` |
| `frontend/planner`, `frontend/code-writer`, etc. | `projects/frontend/.claude/agents/{rest}.md` |

Each dispatch must:
1. Read the agent `.md` file completely.
2. Check `visibility` in frontmatter — if `internal`, it's OK since you (the orchestrator) are an agent caller.
3. Combine the agent's system prompt (body after `---`) with the stage-specific task.
4. Use `Task(subagent_type="general", ...)` with the combined prompt.
5. Include in the prompt which tools the agent may use (from its `tools` field).
6. Direct agent outputs to `.swarm-reports/{plan-id}/`.
