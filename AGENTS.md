# AGENTS.md

## Purpose

This file tells coding agents how to work with this repository.

Use the project documents as sources of truth. Do not duplicate their full content here and do not invent product or architecture decisions that are not documented.

## Project documents

- `README.md` — project entry point, usage and user-facing requirements.
- `PRODUCT.md` — current implemented product behavior and UX rules.
- `ARCHITECTURE.md` — current technical structure, component responsibilities, runtime flow and invariants.

Before a nontrivial change, read the relevant sections of these documents and inspect the current code they describe.

If documents conflict, state the conflict explicitly instead of silently choosing one interpretation.

## Working rules

- Do not present guesses as established project decisions.
- If a required decision is missing, do not invent one silently.
- Prefer the smallest implementation that satisfies the documented requirement.
- Do not add speculative or "useful later" behavior that was not requested or documented.
- Reuse existing project patterns before introducing new abstractions.
- Keep changes local to the component that owns the responsibility.
- Do not change public behavior merely to simplify implementation unless the product documentation is updated accordingly.
- When a change alters implemented product behavior or UX, update `PRODUCT.md`.
- `PRODUCT.md` must describe only behavior that currently exists in the implementation; do not use it for backlog, future plans, non-goals or missing features.
- When a change alters system boundaries, data flow, technical invariants or an accepted technical decision, update `ARCHITECTURE.md`.
- When usage or user-facing requirements change, update `README.md`.
- Keep repository documentation about the current repository state; do not add roadmap or backlog material unless a dedicated document is explicitly requested.

## Repository-specific conventions

- Keep the plugin as one C# project unless a concrete requirement justifies another project.
- Target `Flow.Launcher.Plugin` 5.3.2 and preserve compatibility with Flow Launcher 2.1.4+.
- `plugin.json` `Version` is the release version source of truth. The release workflow creates the corresponding `v<Version>` tag and GitHub Release from `main`.
- Existing user-facing strings are English; keep new strings consistent unless the product requirements change.
- Prefer small concrete services over framework-style abstractions.
- For Flow Launcher integration behavior, check the official Flow documentation and upstream Flow Launcher source before introducing a custom implementation.
