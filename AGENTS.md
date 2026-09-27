# AGENTS.md

## Purpose

This file tells coding agents how to work with this repository.

Use the project documents as sources of truth. Do not duplicate their full content here and do not invent product or architecture decisions that are not documented.

## Project documents

- `README.md` — project entry point, prerequisites, configuration.
- `PRODUCT.md` — product vision, users, behavior, scope, requirements, UX rules and non-goals.
- `ARCHITECTURE.md` — technical structure, component boundaries, runtime flow, data model, integrations, constraints and technical decisions.

Before a nontrivial change, read the relevant sections of these documents and inspect the current code they describe.

If documents conflict, state the conflict explicitly instead of silently choosing one interpretation.

## Working rules

- Do not present guesses as established project decisions.
- If a required decision is missing, mark it as an assumption or open question.
- Prefer the smallest implementation that satisfies the documented requirement.
- Do not expand the current product scope with speculative or "useful later" functionality.
- Reuse existing project patterns before introducing new abstractions.
- Keep changes local to the component that owns the responsibility.
- Do not change public behavior merely to simplify implementation unless the product documentation is updated accordingly.
- When a change alters product behavior, scope or UX, update `PRODUCT.md`.
- When a change alters system boundaries, data flow, technical invariants or an accepted technical decision, update `ARCHITECTURE.md`.
- When setup, prerequisites or run instructions change, update `README.md`.

## Repository-specific conventions

<!-- Add only conventions that really apply to this repository. Examples: naming, commit format, migration policy, testing rules, generated files. -->

- [Add repository-specific conventions here.]

## Tooling / MCP routing

<!-- Add repository-specific tool routing only when needed. Keep it out of this file if the project has no special requirements. -->

- [Add repository-specific tool instructions here, or remove this section.]
