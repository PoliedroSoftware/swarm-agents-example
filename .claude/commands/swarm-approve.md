---
description: Approve a pending approval gate in a running or paused SwarmAgents plan
arguments:
  - name: stage_id
    description: The stage ID to approve (e.g. S3-migration). If omitted, lists pending stages.
    required: false
---

Manage approval gates for a running SwarmAgents plan.

## If no `$stage_id` is provided

1. Find the most recent execution log in `.swarm-reports/`.
2. Read `execution.json`.
3. List all stages with status `awaiting-approval`.
4. Show plan ID, stage ID, agents pending, and why approval is needed.
5. Tell the user: `/swarm-approve {stage-id}` to approve a specific stage.

## If `$stage_id` is provided

1. Find the most recent execution log in `.swarm-reports/`.
2. Read `execution.json`.
3. Verify the stage exists and is in `awaiting-approval` status.
4. If not found or not awaiting: show current plan status and exit.
5. If valid:
   a. Update `execution.json`: change the stage status from `awaiting-approval` to `approved`.
   b. Write the updated execution log back to `.swarm-reports/{plan-id}/execution.json`.
   c. Show: `Stage {stage_id} approved. Run /swarm-run to continue execution.`

Note: This command persists the approval decision to `execution.json`. The orchestrator reads this file before dispatching each stage and skips the approval prompt if the stage status is `approved`.
