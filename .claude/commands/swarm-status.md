---
description: Show SwarmAgents workspace status — current phase, configured projects, active MCPs, env health
---

Run a quick health check of the SwarmAgents workspace and present a concise dashboard (under 40 lines):

1. Read `swarmagents.workspace.json` and report:
   - Workspace name, version, GitHub repo (`owner/repo`)
   - Each project: name, path, stack, agent count, MCP count
2. Inspect `.env` if it exists (NEVER print values — only report which categories are filled):
   - Anthropic, GitHub, Postman, SonarQube, JMeter, MySQL, PostgreSQL, MSSQL
3. Read `.claude/settings.json` and list configured MCP servers by name.
4. Report current phase by parsing `README.md` (look for "Currently in **Phase ...**").
5. List agents present in `.claude/agents/` and `projects/*/.claude/agents/` along with their `visibility` (parsed from frontmatter).

Output format: a Markdown table per section, status emoji-free, terse.
