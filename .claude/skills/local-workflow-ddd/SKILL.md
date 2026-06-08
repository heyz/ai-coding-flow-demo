---
name: local-workflow-ddd
description: Complete spec-driven DDD development workflow that creates an issue, branches, writes product and tech specs, gets user spec approval, implements from specs, auto-creates Xunit tests, runs local code review, gets user code approval, then commits, pushes, and creates a PR. Use this skill whenever the user wants a full spec-first development cycle with automated tests, review-driven quality, and two human approval checkpoints — "从 issue 到 PR 含测试和审查", "完整 DDD 流程", "带单元测试的完整流程", "spec-driven with tests".
---

# local-workflow-ddd

Orchestrate a complete spec-first DDD development workflow from issue to PR,
with mandatory unit tests and automated code review.

## Overview

This skill provides a rigorous 12-step development cycle with **two mandatory
user-review checkpoints**:

```
create-issue → git-branch → write-product-spec → write-tech-spec
    → 🔴 user approves specs
    → implement-specs → create-and-run-unit-tests → review-pr-local
    → 🔴 user reviews code & review.json → fix → git-commit → git-push → create-pr
```

Compared to `local-workflow-full`, this skill:
- **Adds mandatory unit tests** with Xunit (auto-creates the test project if missing)
- **Adds automated code review** (`review-pr-local`) producing `review.json`
- **Adds a fix loop** after code review — the user sees `review.json` findings,
  requests fixes, then review can be re-run
- Still has the two human approval checkpoints: specs review and code review

## When to Use

- The user wants a full spec-first development cycle with automated tests and
  code review — "含测试和代码审查的完整开发流程"
- The user describes a feature and expects unit tests to be created automatically
- The user mentions "DDD 流程", "spec-driven with tests", "完整规格驱动开发"
- The user wants automated code review before committing
- The task is substantial enough to warrant specs, tests, and review

Do NOT use this skill when:
- The task is a trivial one-line fix or simple refactor where specs/tests/review
  would be wasteful. Use `local-workflow-full` or individual skills instead.
- The user only wants one step (just create issue, just commit, etc.)
- Tests already exist and the user doesn't need new ones

## Prerequisites

- A git repository with a GitHub remote (`origin`)
- `gh` CLI installed and authenticated
- `python3` available for the `review-pr-local` preparation scripts
- The following skills available: `create-issue`, `git-branch`,
  `write-product-spec`, `write-tech-spec`, `implement-specs`,
  `review-pr-local`, `git-commit`, `git-push`, `create-pr`
- For step 7: `dotnet test` capability and a unit test framework (Xunit) available

## Comparison with local-workflow-full

| Aspect | local-workflow-full | local-workflow-ddd |
|--------|-------------------|-------------------|
| Specs | ✅ Product + Tech | ✅ Product + Tech |
| Spec review | 🔴 One checkpoint | 🔴 One checkpoint |
| Unit tests | ❌ Not included | ✅ Mandatory (Xunit, auto-create) |
| Code review | ❌ Manual only | ✅ Automated (review.json) |
| Code review checkpoint | 🔴 One generic checkpoint | 🔴 Based on review.json findings |
| Fix loop | ❌ No | ✅ Review → Fix → Re-review |
| Commit/Push/PR | ✅ | ✅ |

## Workflow

### Phase 0: Preparation Check

Before starting, check whether the user's request is substantial enough for this
workflow. If it's a tiny bug fix or trivial refactor, suggest using
`local-workflow-full` instead.

### Step 1: Create Issue

Invoke the `create-issue` skill with the user's feature/task description.

**Input:** The user's description of the feature, bug fix, or change.

**Output to capture:**
- Issue number (e.g., `42`)
- Issue URL
- Issue title

If issue creation fails or the user declines, stop the workflow.

### Step 2: Create Branch

Invoke the `git-branch` skill, passing the issue number from Step 1. The branch
name follows `<type>/<short-desc>-<issueID>` convention.

**Input:** Issue number and title from Step 1.

**Output to capture:**
- Branch name (e.g., `feat/add-user-export-42`)

If branch creation fails, stop the workflow.

### Step 3: Write Product Spec

Invoke the `write-product-spec` skill. Produce the file at
`specs/issue-<N>/product.md`. This describes **what** the feature does from a
user/external perspective — behavior, goals, non-goals, acceptance criteria,
edge cases. No implementation details.

For small, low-risk changes where specs would be unnecessary overhead, suggest
switching to `local-workflow-full` instead — this skill is designed for
substantial features that benefit from full spec coverage.

### Step 4: Write Tech Spec

Invoke the `write-tech-spec` skill. Produce the file at
`specs/issue-<N>/tech.md`. This describes **how** to build it — relevant files,
implementation plan, data flow, risks, test strategy. Must be grounded in actual
codebase patterns.

The tech spec MUST include a "Testing and Validation" section that outlines what
unit tests will be needed. This feeds into Step 7.

### Phase 1: Spec Review Checkpoint (🔴 MUST WAIT)

### Step 5: User Review of Specs

**This is a mandatory checkpoint. Do NOT proceed to Step 6 without user
confirmation.**

1. Present a summary to the user:
   - Issue: `#<N> — <title>` with URL
   - Product spec: `specs/issue-<N>/product.md`
   - Tech spec: `specs/issue-<N>/tech.md`
   - Key architectural decisions

2. Ask the user to review and approve the specs before implementation begins.
   Specs are cheap to revise; code is not.

   If the user requests spec changes:
   - Update the spec files accordingly
   - Re-present for approval
   - Repeat until approved

   If the user says "approved" or equivalent: proceed to Step 6.

### Phase 2: Implementation

### Step 6: Implement from Specs

Invoke the `implement-specs` skill with the approved product spec and tech spec.

This step reads the approved specs and writes the actual code. Keep the specs
updated if implementation reveals changes.

**Output:** Working code on the branch.

### Step 7: Create and Run Unit Tests

**7.1 Check if a test project exists.**

Look for an existing test project in the solution (search for `*Test*` or
`*test*` projects, or check the `.sln` file). If one exists:

- Add test classes/files for the newly implemented code
- Reference the source projects from the test project as needed

**7.2 If no test project exists, create one:**

Create a new Xunit test project in the solution:

```bash
dotnet new xunit -n SJ.BackEnd.Template.Tests -o src/backend/SJ.BackEnd.Template.Tests
dotnet sln SH.BackEnd.Tempalte.sln add src/backend/SJ.BackEnd.Template.Tests
```

Add project references to the source projects that need testing:

```bash
dotnet add src/backend/SJ.BackEnd.Template.Tests reference src/backend/SJ.BackEnd.Template.Services/SJ.BackEnd.Template.Services.csproj
dotnet add src/backend/SJ.BackEnd.Template.Tests reference src/backend/SJ.BackEnd.Template.WebAPI/SJ.BackEnd.Template.WebAPI.csproj
```

Add test framework packages:

```bash
dotnet add src/backend/SJ.BackEnd.Template.Tests package Moq
dotnet add src/backend/SJ.BackEnd.Template.Tests package Microsoft.NET.Test.Sdk
```

**7.3 Write unit tests:**

Write focused unit tests covering:
- Core business logic in Service classes
- Validation logic in DTOs or Validators
- Key edge cases from the product spec's acceptance criteria
- Critical failure modes

Organize tests to mirror the source project structure:

```
SJ.BackEnd.Template.Tests/
├── Services/
│   └── SysUserServiceTests.cs
├── Validators/
│   └── CreateUserRequestValidatorTests.cs
└── Usings.cs
```

**7.4 Run tests and verify they pass:**

```bash
dotnet test src/backend/SJ.BackEnd.Template.Tests --no-restore
```

If tests fail, fix the code or tests until all pass. Report the test results
(summary: passed/failed/skipped count).

### Phase 3: Code Review

### Step 8: Run Local Code Review

Invoke the `review-pr-local` skill. This skill:

1. Runs `python3 .github/scripts/prepare_local_review_inputs.py` to create
   `pr_diff.txt` and `pr_description.txt` at the repository root
2. Delegates to the `review-pr` skill for actual review logic
3. Outputs `review.json` at the repository root with findings

**Important:** This skill does NOT fix code — it only reports findings. Do NOT
run `git add`, `git commit`, `git push`, `gh`, or GitHub API commands during
this step. Do NOT modify source files during this step.

**Output to capture:**
- Path to `review.json`
- Summary of findings (bugs, issues, suggestions)

### Phase 4: Code Review Checkpoint (🔴 MUST WAIT)

### Step 9: User Review of Code Based on review.json

**This is a mandatory checkpoint. Do NOT proceed to Step 10 without user
confirmation.**

1. Present a summary to the user:
   - All files changed (git diff --stat)
   - Test results (from Step 7.4)
   - Review findings from `review.json` (bugs, suggestions, improvement areas)

2. Ask the user to review:
   - Do the code and tests match the approved specs?
   - Are the review findings valid and need fixing?

3. If the user requests code fixes:
   - Apply the fixes based on user direction
   - Consider whether fixes change the `review.json` — if material changes were
     made, re-run Step 8 (review-pr-local) to get an updated review
   - Re-run Step 7.4 (dotnet test) to verify tests still pass
   - Re-present the summary for approval
   - Repeat until approved

   If the user says "approved" or equivalent: proceed to Step 10.

This checkpoint exists because commits are hard to undo, and the user must
validate both the implementation and the review findings before entering git
history.

### Phase 5: Ship

### Step 10: Commit

Invoke the `git-commit` skill.

**Input:** The current staged/unstaged changes on the branch.

The commit message will:
- Follow `type(scope): summary` convention
- Include `Refs #<N>` or `Fixes #<N>` as appropriate
- Be atomic and focused

If commit hooks fail, stop and report. Do not use `--no-verify`.

### Step 11: Push

Invoke the `git-push` skill. Sets upstream tracking on first push.

If push is rejected, follow the `git-push` skill's rejection handling.

### Step 12: Create PR

Invoke the `create-pr` skill.

The PR will:
- Link to the issue with `Closes #<N>` or `Fixes #<N>`
- Include a summary of changes, validation performed, and test results
- Target the repo's default base branch

## Error Handling

If any step fails:

1. **Stop the workflow.** Do not silently continue.
2. **Report the failure clearly** — which step, what went wrong, what state
   was preserved.
3. **Suggest recovery.** Tell the user how to fix or resume manually.

## Output Summary

```
✅ DDD Workflow Complete

Issue:    #<N> — <title>
Branch:   <branch-name>
Specs:    specs/issue-<N>/product.md, specs/issue-<N>/tech.md
Tests:    <test-project>/<count> passed, <count> failed
Review:   review.json — <finding-count> findings
Commit:   <hash>
PR:       <pr-url>
```

## Example

User says:
> 实现用户导出功能，支持导出为 Excel 和 CSV 格式

1. **create-issue**: Issue #42 "实现用户导出功能"
2. **git-branch**: `feat/add-user-export-42`
3. **write-product-spec**: `specs/issue-42/product.md`
4. **write-tech-spec**: `specs/issue-42/tech.md`
5. **🔴 Spec Review**: User approves
6. **implement-specs**: Implements export feature
7. **Unit tests**: Creates `SJ.BackEnd.Template.Tests`, writes `SysUserExportServiceTests.cs`, runs ✅
8. **review-pr-local**: Produces `review.json` with 2 minor findings
9. **🔴 Code Review**: User reviews findings, approves after quick fix
10. **git-commit**: `feat(export): add user export to Excel and CSV Refs #42`
11. **git-push**: Pushes to `origin/feat/add-user-export-42`
12. **create-pr**: PR linking to `Closes #42`
