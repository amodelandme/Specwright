# ADR-002: PostgreSQL as Primary Store

**Status:** Accepted  
**Date:** 2026-04-08

---

## Context

Specwright requires a primary system store for runtime concerns such as:

- workflow execution records
- execution journaling
- metadata and retrieval support
- policy and governance outcomes
- references to context packets and evidence artifacts
- future workflow state and auditability needs

The architecture already defines PostgreSQL as the primary system store. A durable persistence decision is needed so runtime contracts, infrastructure boundaries, and execution recording can be designed consistently.

This decision must support the platform’s broader goals:

- deterministic-first processing
- traceability and auditability
- structured workflow records
- a path to richer retrieval and policy validation over time

The store does not need to solve every future retrieval or analytics concern on day one, but it must provide a stable and credible foundation.

---

## Decision

Specwright will use **PostgreSQL as the primary system store**.

PostgreSQL will initially back:

- execution journal records
- workflow metadata
- capability execution outcomes
- policy evaluation results
- metadata supporting retrieval and traceability

The initial persistence model may store some complex artifacts, such as context packets or evidence bundles, in serialized form while the platform is still early. Normalization can increase later where real usage justifies it.

Entity Framework Core with the Npgsql provider will be the default persistence path for runtime data access.

This decision does **not** imply that every future data concern must live entirely inside PostgreSQL, but it does establish PostgreSQL as the authoritative primary system store for the initial platform.

---

## Alternatives Considered

### 1. SQLite for initial bootstrap

This would reduce local setup burden and may accelerate early prototyping.

Rejected because:

- the system is being designed as a real platform, not a throwaway prototype
- it would likely create a second migration later once real persistence needs arrive
- it would underrepresent the operational shape of the intended runtime

### 2. JSON files or markdown-only persistence

This would align with the project’s strong document orientation and keep persistence very lightweight.

Rejected because:

- document artifacts are source-of-truth for architecture and specs, not a complete runtime persistence model
- workflow execution, metadata indexing, auditability, and queryability need more structure
- file-only persistence would make enforcement and retrieval features harder to evolve cleanly

### 3. Document database as the primary store

A document-oriented store could feel natural for context packets and workflow artifacts.

Rejected because:

- the initial platform also needs strong structured querying and relational traceability
- governance, workflow, and execution data are likely to benefit from relational modeling
- PostgreSQL can still support serialized JSON payloads where useful without making the entire system document-first

### 4. In-memory only during early phases

This would reduce early complexity.

Rejected because:

- execution journaling is already a central architectural concept
- lack of durable persistence would weaken traceability during the exact phase where the platform is trying to prove its architecture
- it would delay important infrastructure decisions instead of clarifying them

---

## Consequences

### Positive Consequences

- gives Specwright a durable, production-credible system store early
- supports structured querying for workflow, policy, and execution data
- aligns well with EF Core and the broader .NET ecosystem
- allows a hybrid approach where structured data and serialized artifacts can coexist
- supports future traceability, auditability, and retrieval needs

### Negative Consequences

- increases local runtime complexity compared with file-based or in-memory approaches
- introduces schema design concerns early
- may tempt premature database modeling for artifacts that should remain simple initially

### Ongoing Implications

Future specs and implementation work should assume:

- runtime persistence lives behind infrastructure boundaries
- PostgreSQL is the primary authoritative store
- Core contracts should remain persistence-agnostic
- serialized storage is acceptable initially when it preserves velocity and does not obscure traceability
- normalization decisions should be driven by real query and validation needs

---

## Review Triggers

This decision should be revisited if:

- the early platform gains little value from relational persistence
- execution records and context artifacts prove overwhelmingly document-shaped and relational access patterns remain weak
- PostgreSQL introduces disproportionate operational burden for the project’s real scope
- a split storage model becomes clearly necessary based on actual usage

---

## Related Artifacts

- `docs/architecture.md`
- `docs/current-state.md`
- `docs/roadmap.md`
