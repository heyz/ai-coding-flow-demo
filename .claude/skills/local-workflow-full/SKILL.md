---
name: local-workflow-full
description: End-to-end local automation workflow that orchestrates issue creation, branch setup, spec-driven implementation, code commit, push, and PR creation in one seamless flow. Use this skill whenever the user wants to start a new feature or fix from scratch and carry it all the way through to a pull request, or when the user says "full workflow", "end to end", "从 issue 到 PR", "完整流程", or describes a task that spans the entire development lifecycle.
---

# local-workflow-full

Orchestrate the complete local development workflow from issue to PR in a single
invocation.

## Overview

This skill chains together six existing skills into one cohesive workflow:

```
create-issue → git-branch → spec-driven-implementation → (user review) → git-commit → git-push → create-pr
```

Each step feeds its output forward — the issue number informs the branch name,
the branch carries the issue ID, the specs go under `specs/issue-<N>/`, and the
PR links back to the originating issue. The user only needs to provide the
initial feature description; the workflow handles the rest.

## When to Use

- The user describes a feature, bug fix, or change they want implemented end to
  end, from issue to PR.
- The user explicitly asks for a "full workflow", "complete flow", "从 issue 到
  PR", "完整流程", or similar.
- The user wants to start work on a new task and carry it through to a
  reviewable PR.

Do NOT use this skill when:
- The user only wants one step (e.g., just create an issue, just commit).
  Invoke the individual skill directly instead.
- The user already has an issue and branch set up. Use `spec-driven-implementation`
  or the relevant individual skill for the remaining steps.

## Prerequisites

- A git repository with a GitHub remote (`origin`).
- `gh` CLI installed and authenticated.
- The following skills available: `create-issue`, `git-branch`,
  `spec-driven-implementation`, `git-commit`, `git-push`, `create-pr`.

## Workflow

### Step 1: Create Issue

Invoke the `create-issue` skill with the user's feature/task description.

**Input:** The user's description of the feature, bug fix, or change.

**Output to capture:**
- Issue number (e.g., `42`)
- Issue URL (e.g., `https://github.com/org/repo/issues/42`)
- Issue title

If issue creation fails or the user declines, stop the workflow. The user can
retry or proceed manually.

### Step 2: Create Branch

Invoke the `git-branch` skill, passing the issue number from Step 1.

The branch will be named `<type>/<short-desc>-<issueID>` following the
`git-branch` skill's naming convention. The type is inferred from the issue
classification (feat, fix, refactor, etc.).

**Input:** Issue number and title from Step 1.

**Output to capture:**
- Branch name (e.g., `feat/add-user-export-42`)
- Current branch confirmed

If branch creation fails, stop the workflow. Resolve the issue before
continuing.

### Step 3: Spec-Driven Implementation

Invoke the `spec-driven-implementation` skill with the issue context.

This step produces:
- Product spec at `specs/issue-<N>/product.md`
- Tech spec at `specs/issue-<N>/tech.md` (when warranted)
- Implementation code that follows the specs

**Input:** The issue number, issue title, and the user's original description.

**Important:** The `spec-driven-implementation` skill internally calls
`write-product-spec`, `write-tech-spec`, and `implement-specs`. This is the
most time-consuming step. For small bug fixes where specs are unnecessary, the
skill will skip specs and proceed directly to implementation — trust its
judgment.

### Step 4: User Review

**This is a mandatory checkpoint.** Before proceeding to commit:

1. Present a summary to the user:
   - Issue created: `#<N> — <title>` with URL
   - Branch: `<branch-name>`
   - Specs created (if any): list the spec file paths
   - Implementation summary: briefly describe what was implemented

2. Ask the user to review:
   - The specs (if created) — are they accurate?
   - The code changes — are they correct?

3. Wait for explicit user approval before continuing.

   If the user requests changes:
   - Make the requested changes
   - Re-present the summary for approval
   - Repeat until approved

Do NOT proceed to Step 5 without user confirmation. This checkpoint exists
because commits and pushes are hard to undo, and the user must validate the
implementation before it enters the git history.

### Step 5: Commit

Invoke the `git-commit` skill.

**Input:** The current staged/unstaged changes on the branch.

The commit message will:
- Follow `type(scope): summary` convention
- Include `Refs #<N>` or `Fixes #<N>` as appropriate
- Be atomic and focused

**Output to capture:**
- Commit hash
- Whether hooks/checks ran

If commit hooks fail, stop and report the failure. Do not use `--no-verify`.

### Step 6: Push

Invoke the `git-push` skill.

**Input:** The committed branch from Step 5.

The push will set upstream tracking on first push (`git push -u origin
<branch>`).

**Output to capture:**
- Remote branch
- Pushed commit hash
- Any dirty changes not pushed (should be none at this point)

If push is rejected, follow the `git-push` skill's rejection handling (fetch,
inspect divergence, ask before rebasing or force-with-lease).

### Step 7: Create PR

Invoke the `create-pr` skill.

**Input:** The pushed branch, issue number from Step 1.

The PR will:
- Link to the issue with `Closes #<N>` or `Fixes #<N>`
- Include a summary of the changes
- Reference validation performed
- Target the repo's default base branch

**Output to capture:**
- PR URL
- Base branch
- Title

Report the final result to the user with a complete summary.

## Error Handling

If any step fails:

1. **Stop the workflow.** Do not silently continue to the next step.
2. **Report the failure clearly** — which step failed, what went wrong, and
   what output was captured.
3. **Preserve state.** Any successfully completed steps remain in effect
   (e.g., if the issue was created but the branch failed, the issue still
   exists).
4. **Suggest recovery.** Tell the user how to fix the issue or resume
   manually using individual skills.

## Output Summary

After all steps complete successfully, present this summary:

```
✅ Full Workflow Complete

Issue:    #<N> — <title>
URL:      <issue-url>
Branch:   <branch-name>
Specs:    specs/issue-<N>/product.md (and tech.md if created)
Commit:   <hash>
PR:       <pr-url>

Next steps:
- Watch CI checks on the PR
- Respond to review comments
- Keep the PR current by rebasing on the base branch as needed
```

## Example

User says:
> 实现用户导出功能，支持导出为 Excel 和 CSV 格式

Workflow execution:
1. **create-issue**: Creates issue #42 "实现用户导出功能"
2. **git-branch**: Creates branch `feat/add-user-export-42`
3. **spec-driven-implementation**: Writes product spec and tech spec under
   `specs/issue-42/`, then implements the feature
4. **User review**: Presents specs and implementation for approval
5. **git-commit**: Commits with message `feat(export): add user export to Excel and CSV Refs #42`
6. **git-push**: Pushes to `origin/feat/add-user-export-42`
7. **create-pr**: Creates PR linking to `Closes #42`
