# Taskington - Project Specification

**Document Version: 2.1 - 2026-10-09 - Andreas Weizel (with Mistral Vibe)**

Living document: any development change that alters a decision stated here updates this
spec (plus `JOB_SYNTAX.md` and the `AGENTS.md` files as applicable) in the same change;
bump the version above. Stories and phases: [BACKLOG.md](BACKLOG.md).

## 1. Overview

Desktop GUI application for defining, running and observing repeatable tasks ("jobs") on
the local system. Cross-platform: Windows, Linux, macOS (developed on macOS).

## 2. Technical stack

- .NET 10, C#; Avalonia 12 (Desktop, AvalonEdit, ReactiveUI, Fluent theme, Inter font).
- Job scripts in Lua, interpreted by LuaCSharp (https://github.com/nuskey8/Lua-CSharp).
- DI: Microsoft.Extensions.DependencyInjection. Logging: Serilog + file sink.
- Projects: `Taskington.App` (GUI) -> `Taskington.Base` (logic, no UI); one xUnit test
  project per main project.
- NuGet central package management (`Directory.Packages.props`), lockfiles committed.

## 3. Architecture

- MVVM in App; DI container for all components; event-driven UI updates from job
  executions.
- App depends on Base; Base never references App or any UI framework.
- Operative guardrails live in the AGENTS.md files (root, Base, App).

## 4. Domain concepts

**Job:** `Id` (app-created GUID, doubles as storage file name), `Name`, `Description`,
`Script`, `CreatedAt`, `ModifiedAt`, `IsTemporary` (newly added, unsaved).

**Job states:** Idle, Running, Completed, Failed, Cancelled (Paused later).

**Execution context:** ExecutionId, JobId, StartTime, EndTime?, Status, `ExecutionMode`
(Normal/Dry), Progress (0-100), Output (in-memory stream), ExitCode.

## 5. User interface

Principles: no menu bar, no modal dialogs, no preferences. All interaction via tiles,
side panel and inline states. Theme follows the system light/dark setting; no toggle.

**Dashboard:** responsive tile grid plus "Add Job" button. Tile shows name, description,
color-coded state, progress bar (while running), status text, and buttons: Run, Edit,
Delete, View Output (output while running or after failure). Run disabled while running
(a job never executes twice concurrently).

**Add Job:** button creates a temporary tile (marked as new) and opens the editor in the
side panel; Save persists (tile becomes regular), Discard removes the tile.

**Delete:** inline confirmation state on the tile ("Confirm delete"); confirm removes the
job and its history, cancel reverts.

**Side panel:** one content slot - live output viewer or job editor; content switches
with job selection; user-closable; resizable (default 300-400px).

**Output viewer:** live scrolling text, timestamps, color-coded levels (stdout, stderr,
warning, error), auto-scroll with lock option, copy-all, clear (search later).

**Job editor:** Avalonia.AvalonEdit with Lua syntax highlighting, line numbers, basic
editing, Save/Discard buttons in the panel header (existing job: save/revert; new job:
create/remove), read-only while the job runs, syntax validation feedback.

**Status bar:** copyright, version, "About" link.

## 6. Job script language (DSL)

- Zero-trust sandbox: no standard Lua libraries (`io`, `os`, `debug`, ...), no direct
  filesystem or process access, no network, no reflection/metable escapes; only
  Taskington-registered functions are callable. Fresh Lua environment per execution.
  No timeouts in MVP.
- Concrete syntax is defined in [JOB_SYNTAX.md](JOB_SYNTAX.md). Goals: simple,
  non-technical, human-readable. MVP capability set: execute command-line tools,
  sync files/directories (rsync), logging, progress/status reporting. Detailed
  definitions deliberately postponed; function names elsewhere in this spec are
  provisional.
- Errors: syntax errors prevent execution (job Failed, details in output); runtime
  errors mark Failed; API argument errors raise descriptive Lua errors; file/permission
  issues surface via exit codes.

## 7. Components

**Base:** models (see 4); `JobRepository`, `JobHistoryService`, `FileSystemService`,
`ProcessService`, `JobExecutorService`; `LuaScriptEngine`, `ScriptSandbox`,
`TaskingtonLuaApi`, `ScriptExecutor`, `IActionExecutor`; `PathUtils`,
`ValidationUtils`, `LoggingExtensions`.

**App:** `MainWindow`, `DashboardView` (Add Job), `JobTileView`, `SidePanelView`,
`OutputViewerView`, `JobEditorView` plus matching ViewModels (DashboardViewModel with
Add Job command, JobEditorViewModel with Save/Discard); `NotificationService`.
Deliberately no dialog or preferences services.

## 8. Data storage

One JSON file per job in the fixed (not configurable) per-platform user config
directory: macOS `~/Library/Application Support/Taskington/`, Linux
`$XDG_CONFIG_HOME/taskington/` (default `~/.config/taskington/`), Windows
`%APPDATA%\Taskington\`.

Layout: `jobs/<GUID>.json` (id, name, description, script, createdAt, modifiedAt);
`history/<GUID>/<execution>.json` (executionId, jobId, startTime, endTime,
executionMode, status, exitCode - metadata only, never output); `logs/`;
`config.json` only if ever needed.

Job output is never persisted in MVP (live viewer only, in-memory). Internal paths use
the platform dirs; paths inside scripts are the user's responsibility (a variable system
may help later).

## 9. Execution flow

Phases: validate (parse + sandbox) -> prepare (context with execution mode, output
capture, progress, API registration) -> execute (background thread, output/progress
events, error catching) -> cleanup (history record, state update, UI notify).

**Parallelism:** jobs execute independently in parallel, no limits in MVP; the same job
never twice concurrently. Failed jobs are simply re-run via Run; partial completion is
ignored.

**Dry mode:** any job can run without real actions; each intended action is documented
as a "would do ..." entry in the job output. Implementation: `ExecutionMode` on the
context; all API actions go through `IActionExecutor`; Normal binds real executors, Dry
binds a recording executor; everything else is shared.

## 10. Errors and logging

Failures surface through the job state on its tile plus details in the output panel -
never dialogs.

| Category | Handling | Feedback |
|----------|----------|-----------|
| Script syntax error | Prevent execution | Failed state + output details |
| Script runtime error | Stop, Failed | Tile + output |
| Command failed | Continue with exit code | Output + tile status |
| File not found / permission denied | Stop, Failed | Tile + message in output |
| Internal error | Stop, log, Failed | Tile + output |

Serilog with verbose level for development; not user-configurable in MVP. Log files
under the platform `logs/` dir (`application-*.log`, `errors-*.log`).

## 11. Testing

**Base.Tests:** sandbox security (escape attempts), Lua API functions (both modes), dry
mode, repository/history, state management, execution flow (success, error, cancel),
cross-platform directory resolution.

**App.Tests:** ViewModels and commands - add/save/discard flows, inline delete,
per-state command availability, side panel switching.

Coverage goals: scripting engine 90%+, business logic 80%+, ViewModels 70%+,
services 85%+. Integration: end-to-end runs in both modes; cross-platform checks.

## 12. Build and deployment

Configurations Debug/Release/Production; framework-dependent by default,
self-contained optional. Packaging: macOS .app bundle, Windows MSI, Linux deb/rpm/
AppImage. GitHub Actions CI (build + tests, unicode check).

## 13. Future ideas (not MVP)

Plugin system (custom Lua functions); job templates; variable system (also for
cross-platform script paths); job grouping/folders/search; keyboard shortcuts, context
menus, command palette; enhanced output viewer (highlighting, filter, export, timeline);
script debugging (breakpoints, stepping); script testing framework; script versioning;
output streaming to disk; script timeouts; script signing; permission system; SQLite
storage; output persistence; backups; cloud sync.

## 14. MVP scope

**Included:** dashboard with Add Job flow; CRUD (Save/Discard in panel, inline delete);
Lua API per JOB_SYNTAX.md; command execution; rsync; parallel execution; dry mode; live
output; editor with highlighting; progress tracking; history (metadata only); sandbox;
system-following theme.

**Excluded:** scheduling; plugins; variable system; script debugging; folders; cloud
sync; persisted output; timeouts; preferences UI; backups.

## 15. Design decisions (resolved)

1. Storage: JSON, one file per job; file name = app-created GUID = job ID.
2. No job output persisted in MVP.
3. No script timeouts in MVP.
4. No retry mechanism; re-run via Run button; partial completion ignored.
5. Platform user config dirs for internal paths; script paths are the user's
   responsibility.
6. Logging not configurable in MVP; verbose level available for development.
7. No backups in MVP.

## 16. References

Avalonia docs (https://docs.avaloniaui.net/), Lua-CSharp (github.com/nuskey8/Lua-CSharp),
Lua 5.4 manual (lua.org), Microsoft.Extensions.DependencyInjection docs,
[JOB_SYNTAX.md](JOB_SYNTAX.md).
