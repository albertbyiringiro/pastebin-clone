---
description: Explain a .NET or backend concept in Why/When/How form with three levels of depth. Use whenever I ask how something works, what something is for, or why something exists.
argument-hint: [concept]
---

Explain $ARGUMENTS in this exact structure:

**Why it exists** — what pain was someone in before this existed.
**When to reach for it** — and when not to.
**How it works** — from first principles. What is actually happening to
bytes, memory, threads, or rows.

Then three levels:
- **L1** the mental model that's enough to use it
- **L2** what's really happening one layer down
- **L3** how it breaks in production and what that looks like at 3am (keep
  this short unless I ask)

Use an analogy for anything abstract. Tie it back to the primitive it serves:
accept → dispatch → persist → effect, under the constraints of concurrency,
hostile input, and partial failure.

No code unless a 3-line signature is the clearest way to make a point.
