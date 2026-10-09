# AGENTS.md - Taskington

Cross-platform .NET 10 / C# / Avalonia 12 desktop app for defining, running and observing
repeatable tasks ("jobs") scripted in a sandboxed Lua-based DSL.

Authoritative docs: `spec/PROJECT_SPEC.md` (scope and behavior), `spec/BACKLOG.md`
(phases and stories; check off completed stories), `spec/JOB_SYNTAX.md` (Lua API surface).
Sub-project rules: `Taskington.Base/AGENTS.md`, `Taskington.App/AGENTS.md` (active in
those directories).

## Layout

`Taskington.App` (Avalonia GUI, references Base), `Taskington.App.Tests`,
`Taskington.Base` (business logic, no UI), `Taskington.Base.Tests`, `spec/`,
`buildtools/` (build/publish/test scripts), `Directory.Packages.props` (central package
versions). SDK pinned by `global.json` (10.0, no prerelease).

## Build / test

```sh
dotnet build Taskington.sln
dotnet test Taskington.sln
buildtools/publish-macosarm64
buildtools/unicode-check   # source text policy, runs in CI
```

Solution must build and all tests be green before committing.

## Packages (strict)

- Never install or update any NuGet dependency (including transitive) without explicit
  permission.
- Package versions only in `Directory.Packages.props`; `PackageReference` entries carry
  no versions.
- `packages.lock.json` lockfiles are committed; update only with an approved package
  change.

## Documentation is part of the change (strict)

Spec and AGENTS.md files describe the architecture as decided, not as it once was.
Any change to an architectural decision (component, responsibility, guardrail, storage
or execution behavior) updates, in the same change: `spec/PROJECT_SPEC.md`
(+ `spec/JOB_SYNTAX.md` if the script syntax is affected), the affected `AGENTS.md`
files, and `spec/BACKLOG.md` if stories are affected. No silent divergence; bump the
spec's Document Version when a decision stated there changes.

## Coding rules

- Follow `.editorconfig` (4-space indent, UTF-8); nullable reference types enabled.
- Clean, reviewable code: no speculative abstractions, no dead or commented-out code.
- ASCII where possible (`buildtools/unicode-check` enforces); no emoji anywhere.
- DI via constructor injection only; no service locator.
- Never edit `bin/`, `obj/`, `output/`, `build/`.
- Implement only what the spec and current backlog story require; MVP exclusions
  (spec chapter 14.1) are binding.
