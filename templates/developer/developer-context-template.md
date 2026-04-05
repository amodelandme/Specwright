# Developer Context

> **What this file is:**
> Personal configuration for the AI skills in this framework. It tells the
> Architect and Engineer skills *who is doing the building* — not what is being
> built. That lives in `/docs`.
>
> **What this file is not:**
> Project documentation. Nothing here belongs in a PR, a README, or a team wiki.
>
> **How to use it:**
> Copy this file to your repo root, rename it `developer-context.md`, and fill
> it in. Add `developer-context.md` to your `.gitignore` before your first commit.
> It is personal and should never be committed.
>
> **How the skills use it:**
> Both `skills/architect.md` and `skills/engineer.md` load this file at the
> start of every session. The more honestly you fill it in, the better they
> adapt to you. Placeholder text left unchanged helps no one.

**Last updated:** <YYYY-MM-DD>

---

## Table of Contents

- [Profile](#profile)
- [Career Goals](#career-goals)
- [Learning Style](#learning-style)
- [Learning Tendencies and Blind Spots](#learning-tendencies-and-blind-spots)
- [Engineering Values](#engineering-values)
- [Interaction Preferences](#interaction-preferences)
- [Current Focus](#current-focus)
- [Off-Limits Topics](#off-limits-topics)

---

## Profile

> Who you are as a developer right now. Be honest — this is not a resume.
> The skills use this to calibrate explanation depth, feedback tone, and
> the level of detail in code examples.

**Current level:** <junior | mid | mid → senior (transitioning) | senior>

**Primary stack:** <e.g., .NET 8, EF Core, ASP.NET Core, React>

**Years of production experience:** <e.g., 3 years total, with a 2-year gap>

**Domain background:** <e.g., healthcare APIs, e-commerce, internal tooling>

---

## Career Goals

> What you are trying to achieve and on what timeline. The skills use this
> to decide when to push you toward more senior patterns, when to flag
> interview-relevant decisions, and when to surface portfolio opportunities.

**Immediate goal:** <e.g., interview-ready for mid-to-senior backend roles within 6 months>

**Secondary goal:** <e.g., a specific certification, framework, or domain>

**Portfolio intent:** <e.g., what projects you are building and why>

**Target roles:** <e.g., backend engineer, full-stack, platform engineer>

**Community goal:** <e.g., open source contributions, technical writing, speaking>
<!-- Delete this line if not applicable. -->

---

## Learning Style

> How you absorb and retain technical concepts best. The skills use this
> to decide when to reach for a diagram, when to go deep on internals,
> and how to structure explanations.

**Visual learner:** <yes | no | partial>
<!-- If yes, the Architect skill defaults to diagrams and visual
     representations before prose explanations. -->

**Explanation preference:** <e.g., concrete example first, then the abstract rule>
<!-- The alternative is rule-first, then example. Most developers learn
     faster from examples. -->

**Depth preference:** <surface | conceptual | deep>
<!-- Surface: just tell me what to do.
     Conceptual: tell me why.
     Deep: tell me what the runtime is doing internally. -->

**Example preference:** <e.g., production-realistic examples over toy examples>

---

## Learning Tendencies and Blind Spots

> Honest observations about where your thinking goes wrong or gets fuzzy.
> This is the most valuable section for the skills — and the hardest to fill in.
> The skills use it to catch you before you go down a bad path, not to judge you.
>
> Examples of useful entries:
> - "I tend to over-abstract early — I reach for interfaces before I know what I'm abstracting"
> - "I think in terms of implementation before I think about the caller's perspective"
> - "I conflate validation and sanitization"
> - "I underestimate how much context a new team member needs"
> - "I gold-plate — I keep refining instead of shipping"

- <Tendency or blind spot>
- <Tendency or blind spot>
- <Tendency or blind spot>

---

## Engineering Values

> The principles you hold strongly and want the skills to reinforce.
> The skills use this to know when to push back (when you violate these)
> and when to affirm (when you get it right).
>
> These are personal — they reflect how *you* want to work, not necessarily
> what the project requires. Project-level conventions live in `docs/ai-context.md`.
>
> Examples:
> - "Correctness over velocity — I'd rather ship late and right than fast and broken"
> - "Every piece of work should be interview-reviewable"
> - "Decisions without rationale are technical debt"
> - "Treat AI as a collaborator, not an autocomplete — push back on bad suggestions"

- <Engineering value>
- <Engineering value>
- <Engineering value>

---

## Interaction Preferences

> How you want the skills to communicate with you. Stylistic preferences,
> not architectural ones. The skills use this to calibrate tone, feedback
> style, and how they handle uncertainty.

**Tone:** <e.g., direct and conversational | formal | light humor welcome>

**Feedback style:** <e.g., push back hard when I'm wrong | soften it | just show me the fix>

**When I'm stuck:** <e.g., ask me a probing question before giving me the answer | just tell me>

**Interview lens:** <yes | no>
<!-- If yes, the Engineer skill flags decisions worth articulating in a
     technical interview and suggests how to explain them. -->

**Mentorship mode:** <yes | no>
<!-- If yes, both skills treat sessions as learning opportunities — they
     explain decisions, connect patterns to fundamentals, and occasionally
     ask probing questions to test your understanding. -->

---

## Current Focus

> What you are actively working on right now. Update this when your focus
> shifts. The skills use it to avoid suggesting work that is out of phase
> or premature.
>
> This is different from `docs/current-state.md`, which tracks the project's
> state. This section tracks *your* state — what you are trying to learn or
> accomplish in the current sprint or session.

**Active project:** <project name and one-line description>

**Current phase goal:** <what done looks like for the current phase>

**What I'm trying to learn right now:** <specific concept, pattern, or technology>

**What I want to avoid right now:** <e.g., getting pulled into future phases prematurely>

---

## Off-Limits Topics

> Topics the skills should not raise unless you bring them up first.
> Use this sparingly — it exists for genuine constraints, not avoidance.
>
> Examples:
> - "Don't suggest microservices until I ask — I'm not ready for that complexity yet"
> - "Don't recommend [framework] — I'm committed to [alternative] for this project"
> - "Don't bring up [topic] during [other topic] sessions — I context-switch poorly"

- <Off-limits topic and why>
<!-- Add more as needed, or delete this section entirely if not applicable. -->