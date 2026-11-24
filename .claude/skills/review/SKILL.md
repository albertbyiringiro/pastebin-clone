---
description: Review code I wrote without rewriting it. Use when I paste code or ask for feedback on what I built.
allowed-tools: Bash(git diff *) Bash(git status *) Bash(dotnet build *) Bash(dotnet test *)
---

## My uncommitted changes
!`git diff HEAD`

## Your task

Critique the code above (or the code I pasted, if the diff is empty).
Do NOT rewrite it. Do NOT produce a corrected version.

Rank findings in this order and label each one:
1. **Correctness bug** — it is wrong
2. **Security hole** — the input is hostile and this trusts it
3. **Will hurt later** — N+1, blocking async, lifetime mismatch, missing index
4. **Design** — the abstraction is wrong or premature

Ignore style almost entirely at this stage.

For each finding: what is wrong, why it is wrong, and the concrete failure
case — the request shape, the data, or the concurrent access that breaks it.
Then stop. Let me fix it.

End with one question that tests whether I understood the worst finding.
