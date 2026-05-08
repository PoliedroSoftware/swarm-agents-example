---
description: Show SwarmAgents workspace status — current phase, configured projects, active MCPs, env health, and execution state
---

Run a quick health check of the SwarmAgents workspace and present a concise dashboard (under 50 lines):

## 1. Workspace info

Read `swarmagents.workspace.json` and report:
- Workspace name, version, GitHub repo (`owner/repo`)
- Each project: name, path, stack, agent count (defined vs listed), MCP count

## 2. Environment health

Inspect `.env` if it exists (NEVER print values — only report which categories are filled):
- Anthropic, GitHub, Postman, SonarQube, JMeter, MySQL, PostgreSQL, MSSQL, Redis

## 3. MCP servers

Read `.claude/settings.json` and list configured MCP servers with their status (enabled/disabled).

## 4. Agents inventory

Scan `.claude/agents/` and `projects/*/.claude/agents/`:
- List each agent with `name`, `visibility`, `model`, and file path.
- Flag agents listed in `swarmagents.workspace.json` but missing their `.md` file as `MISSING`.
- Flag agents with `.md` files but not listed in manifest as `ORPHANED`.
- Workspace-level agents (`.claude/agents/`) are always valid — they are not listed per-project in the manifest.

## 5. Contracts

Check `contracts/api.openapi.yaml`:
- Exists → report version, endpoint count, last modified.
- Missing → flag as `NOT GENERATED`.
Check `contracts/api.openapi.snapshot.yaml`:
- Exists → report last modified.
- Missing → flag as `NO SNAPSHOT`.

## 6. Execution state

Check `.swarm-reports/`:
- List recent plans (last 5): plan ID, intent, classification, status (from execution.json if exists).
- Show currently running plan and its stage progress if any.
- Show any stages in `awaiting-approval` status.

## 7. Runtime integrity

- Check `.claude/skills/swarm-dispatch/SKILL.md` and `.claude/skills/swarm-orchestrate/SKILL.md` exist.
- Check `.claude/commands/swarm-plan.md`, `swarm-run.md`, `swarm-approve.md` exist.
- Verify `swarmagents.workspace.json` is valid JSON.

## Output format

Use a Markdown table per section. No emojis. Terse.
