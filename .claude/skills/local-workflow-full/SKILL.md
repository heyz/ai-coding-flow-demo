---
name: local-workflow-full
description: End-to-end local automation workflow that orchestrates issue creation, branch setup, spec drafting, user-reviewed implementation, code commit, push, and PR creation in one seamless flow. Use this skill whenever the user wants to start a new feature or fix from scratch and carry it all the way through to a pull request, or when the user says "full workflow", "end to end", "从 issue 到 PR", "完整流程", or describes a task that spans the entire development lifecycle.
---

# local-workflow-full

Orchestrate the complete local development workflow from issue to PR in a single
invocation.

## Overview

This skill chains together individual skills into one cohesive workflow with
**two mandatory user-review checkpoints** — one after spec drafting, one after
implementation:

```
create-issue → git-branch → write-product-spec → write-tech-spec
    → (user review: specs) → implement-specs → (user review: code)
    → git-commit → git-push → create-pr
```

Each step feeds its output forward — the issue number informs the branch name,
the branch carries the issue ID, the specs go under `specs/issue-<N>/`, and the
PR links back to the originating issue. The user only needs to provide the
initial feature description; the two review checkpoints ensure the user stays
in control of what gets built and committed.

**The two review checkpoints solve different problems:**

1. **Spec review** (after docs, before code): The user validates that the
   product behavior and technical approach are correct BEFORE any code is
   written. This prevents wasted implementation effort on misaligned specs.
2. **Code review** (after implementation, before commit): The user validates
   that the actual code matches the approved specs and is correct in detail.

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
  `write-product-spec`, `write-tech-spec`, `implement-specs`, `git-commit`,
  `git-push`, `create-pr`.

## Workflow

### Phase 1: Preparation

### Step 1: Create Issue

Invoke the `create-issue` skill with the user's feature/task description.

**Input:** The user's description of the feature, bug fix, or change.

**Output to capture:**
- Issue number (e.g., `42`)
- Issue URL (e.g., `https://github.com/org/repo/issues/42`)
- Issue title

If issue creation fails or the user declines, stop the workflow.

### Step 2: Create Branch

Invoke the `git-branch` skill, passing the issue number from Step 1.

The branch will be named `<type>/<short-desc>-<issueID>` following the
`git-branch` skill's naming convention. The type is inferred from the issue
classification (feat, fix, refactor, etc.).

**Input:** Issue number and title from Step 1.

**Output to capture:**
- Branch name (e.g., `feat/add-user-export-42`)
- Current branch confirmed

If branch creation fails, stop the workflow.

### Phase 2: Specs (docs only, no code yet)

### Step 3: Write Product Spec

Invoke the `write-product-spec` skill.

Produce the file at `specs/issue-<N>/product.md`. This describes **what** the
feature does from a user/external perspective — behavior, goals, non-goals,
acceptance criteria, edge cases. No implementation details.

For small, low-risk changes where specs would be unnecessary overhead, this step
may be skipped — but explicitly tell the user you're skipping it and why, and
proceed to Step 5 (implementation).

### Step 4: Write Tech Spec

Invoke the `write-tech-spec` skill when the feature warrants it (cross-cutting,
architectural decisions, multi-module changes).

Produce the file at `specs/issue-<N>/tech.md`. This describes **how** to build
it — relevant files, implementation plan, data flow, risks, test strategy. Must
be grounded in actual codebase patterns.

For small features or pure UI changes where a tech spec adds no value, this step
may be skipped — explicitly tell the user you're skipping it and why.

### Phase 3: Spec Review Checkpoint (🔴 MUST WAIT)

### Step 5: User Review of Specs

**This is a mandatory checkpoint. Do NOT proceed to Step 6 without user
confirmation.**

1. Present a summary to the user:
   - Issue: `#<N> — <title>` with URL
   - Product spec: `specs/issue-<N>/product.md` (or "skipped" with reason)
   - Tech spec: `specs/issue-<N>/tech.md` (or "skipped" with reason)
   - Key architectural decisions captured in the tech spec

2. Ask the user to review the specs:
   - Does the product spec describe the right behavior?
   - Does the tech spec propose a sound implementation approach?

   **Also inform the user what happens after approval:**
   ```
   ✅ 批准后执行：
      Step 6  implement-specs     — 根据规格编写代码
      Step 7  🔴 再次审查代码      — 确认代码正确
      Step 8  git-commit          — 提交代码
      Step 9  git-push            — 推送到远程
      Step 10 create-pr           — 创建 Pull Request
   ```

3. Wait for explicit user approval before continuing.

   If the user requests spec changes:
   - Update the spec files accordingly
   - Re-present for approval
   - Repeat until approved

   If the user says "approved" or "ok" or "go ahead" or equivalent: proceed to
   Step 6.

The purpose of this checkpoint is to avoid wasting effort on code that implements
the wrong behavior or the wrong architecture. Specs are cheap to revise; code is
not.

### Phase 4: Implementation

### Step 6: Implement from Specs

Invoke the `implement-specs` skill with the approved product spec and tech spec.

This step reads the approved specs and writes the actual code. The implementation
may update the specs if discoveries during coding change the approach — that's
normal and expected. What matters is that the user approved the direction in
Step 5.

**Output:** Working code on the branch, with specs possibly updated to reflect
implementation realities.

### Phase 5: Code Review Checkpoint (🔴 MUST WAIT)

### Step 7: User Review of Code

**This is a mandatory checkpoint. Do NOT proceed to Step 8 without user
confirmation.**

1. Present a summary to the user:
   - Issue: `#<N> — <title>` with URL
   - Branch: `<branch-name>`
   - Specs: list the spec file paths
   - Files changed: summary (git diff --stat)
   - Key implementation highlights: briefly describe what was implemented

2. Ask the user to review:
   - Does the code match the approved specs?
   - Are there any issues with the implementation?

   **Also inform the user what happens after approval:**
   ```
   ✅ 批准后执行：
      Step 8  git-commit   — 提交代码
      Step 9  git-push     — 推送到远程
      Step 10 create-pr    — 创建 Pull Request
   ```

3. Wait for explicit user approval before continuing to commit.

   If the user requests code changes:
   - Make the requested changes
   - Re-present the summary for approval
   - Repeat until approved

   If the user says "approved" or "ok" or "go ahead" or equivalent: proceed to
   Step 8.

This checkpoint exists because commits and pushes are hard to undo, and the user
must validate the implementation before it enters git history.

### Phase 6: Ship

### Step 8: Commit

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

### Step 9: Push

Invoke the `git-push` skill.

**Input:** The committed branch from Step 8.

The push will set upstream tracking on first push (`git push -u origin
<branch>`).

**Output to capture:**
- Remote branch
- Pushed commit hash

If push is rejected, follow the `git-push` skill's rejection handling (fetch,
inspect divergence, ask before rebasing or force-with-lease).

### Step 10: Create PR

Invoke the `create-pr` skill.

**Input:** The pushed branch, issue number from Step 1.

The PR will:
- Link to the issue with `Closes #<N>` or `Fixes #<N>`
- Include a summary of the changes and validation performed
- Target the repo's default base branch

**Output to capture:**
- PR URL
- Base branch, title

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
3. **write-product-spec**: Writes product spec to `specs/issue-42/product.md`
4. **write-tech-spec**: Writes tech spec to `specs/issue-42/tech.md`
5. **🔴 Spec Review**: User reviews specs, approves the plan
6. **implement-specs**: Implements the feature based on approved specs
7. **🔴 Code Review**: User reviews the code, approves the implementation
8. **git-commit**: Commits with message `feat(export): add user export to Excel and CSV Refs #42`
9. **git-push**: Pushes to `origin/feat/add-user-export-42`
10. **create-pr**: Creates PR linking to `Closes #42`
