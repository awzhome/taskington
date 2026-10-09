# AGENTS.md - Taskington.App

Avalonia 12 GUI frontend. Root `AGENTS.md` applies; authoritative UI spec in
`spec/PROJECT_SPEC.md` chapter 5.

## Boundaries

- GUI only; business logic lives in `Taskington.Base` - this project binds against it,
  never re-implements it.
- MVVM with Avalonia.ReactiveUI: views thin, all behavior in ViewModels; ViewModels hold
  no Avalonia control references.
- ViewModels/services resolved via DI; no manual construction chains, no service locator.
- Tests in `Taskington.App.Tests` cover ViewModel/command behavior, not pixels.

## UI guardrails (non-negotiable)

- No menu bar; the dashboard's "Add Job" button is the entry point for creating jobs.
- No modal dialogs: failures show through the job's state on its tile, details in the
  output side panel; delete uses an inline tile confirmation.
- No preferences/settings UI, no theme toggle; the app follows the system light/dark
  theme.
- Side panel has exactly one content slot (output viewer or job editor); Save/Discard
  are buttons in the panel header; switching jobs switches content.
- New jobs are temporary tiles edited in the side panel (Save persists, Discard removes)
  - never a "new job" dialog.
- The job editor is read-only while the job is running.
- Status bar shows copyright, version and an "About" link only.

## Avalonia conventions

- Compiled bindings by default: keep `x:DataType` correct on binding scopes.
- `ViewLocator.cs` maps ViewModels to views by convention; follow it for new views.
- Code-behind stays minimal (wiring that cannot be a binding/command).
- Never block the UI thread: background services, UI updates via observables marshalled
  to the UI thread.
- Render live output with buffered updates; never append unbounded output to an
  ItemsControl.
