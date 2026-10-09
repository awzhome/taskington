# Taskington - Development Backlog

From [PROJECT_SPEC.md](PROJECT_SPEC.md). Incremental phases; check stories off when
done. Every story includes its unit tests (`Taskington.Base.Tests` /
`Taskington.App.Tests`) unless stated otherwise.

Standing rules:
- Never install or update any dependency without permission.
- Clean, reviewable code; no modals/menus/preferences in the GUI (spec 5, 14.1).
- If a story changes an architectural decision, update spec, AGENTS.md files and this
  backlog in the same change (root AGENTS.md, "Documentation is part of the change").

Dependencies: 0 -> 1 -> 2 -> 3 -> 4; Phase 5 needs Phase 1, Phases 6-8 need 3/4 and 5;
Phase 9 last.

## Phase 0: Foundation

- [ ] **S0.1 Platform directory service:** resolve/create per-OS user config dir + `jobs/`, `logs/` subdirs (macOS `~/Library/Application Support/Taskington/`, Linux `$XDG_CONFIG_HOME/taskington/`, Windows `%APPDATA%\Taskington\`). Tests cover resolution/creation per OS.
- [ ] **S0.2 Logging bootstrap:** Serilog file sinks under platform `logs/`, verbose level for development. Log files written on start; verbose via build config or startup flag.
- [ ] **S0.3 DI setup:** container at app entry, registration extension methods per module. Root services resolved from container; no manual construction in views/VMs.
- [ ] **S0.4 CI sanity:** CI builds the solution and runs both test projects; green on push.

## Phase 1: Core domain (Taskington.Base)

- [ ] **S1.1 Domain models:** Job, JobState, ExecutionMode, JobExecution, ExecutionResult, ScriptValidationResult. Tests cover state transitions.
- [ ] **S1.2 JobRepository:** one JSON file per job, GUID ids, atomic writes (temp + rename), load/save/delete. Tests include malformed files and concurrent access.
- [ ] **S1.3 JobHistoryService:** metadata JSON per execution under `history/<GUID>/`. Tests verify layout and record-on-completion.

## Phase 2: Scripting engine and sandbox

- [ ] **S2.1 LuaScriptEngine:** load/parse scripts, structured syntax errors with line info.
- [ ] **S2.2 ScriptSandbox:** fresh environment per execution, no standard libraries; escape-attempt tests (os, io, require-style, metatables).
- [ ] **S2.3 Define MVP syntax in [JOB_SYNTAX.md](JOB_SYNTAX.md):** simple, non-technical, human-readable; commands, rsync sync, logging, progress/status. Code names must match it.
- [ ] **S2.4 TaskingtonLuaApi:** exactly the JOB_SYNTAX.md functions, delegating via IActionExecutor, argument validation raising Lua errors. Tests per function with mock and real executors.

## Phase 3: Job execution

- [ ] **S3.1 IActionExecutor + real implementations:** ProcessService command execution (stdout/stderr capture, working directory), rsync invocation with presets. Integration tests against temp dirs.
- [ ] **S3.2 ScriptExecutor:** execution context, output capture, progress tracking, API registration, error catching, history finalization; background task, cancellable. Tests: success, runtime error, cancellation.
- [ ] **S3.3 JobExecutorService:** jobs run independently in parallel (no limits); refuse second concurrent execution of the same job; isolated Lua environments. Concurrency tests.
- [ ] **S3.4 Output/progress streaming:** in-memory events (timestamped, leveled), bounded buffer. Tests verify event order and buffer cap.

## Phase 4: Dry mode

- [ ] **S4.1 Recording IActionExecutor:** logs "would do ..." entries instead of acting. Tests prove no process/file operation happens and all intended actions are documented.
- [ ] **S4.2 Mode plumbing:** ExecutionMode selectable per run through executor/service; progress, logging, errors identical to normal mode; history records the mode. Same script tested in both modes.

## Phase 5: Application shell (Taskington.App)

- [ ] **S5.1 MVVM infrastructure:** MainWindow + MainWindowViewModel, view location, DI ViewModel construction, reactive bindings. App shows empty dashboard and side panel region.
- [ ] **S5.2 Layout:** dashboard area, resizable side panel (default 300-400px, single slot), status bar. Panel opens/closes/resizes.
- [ ] **S5.3 Status bar and theme:** copyright, version (from build info), "About" link/overlay; app automatically follows system light/dark theme.

## Phase 6: Dashboard and job tiles

- [ ] **S6.1 Dashboard + tile views/VMs:** jobs loaded from repository; tile shows name, description, state, progress bar, status text; live updates from running executions.
- [ ] **S6.2 Tile commands:** Run (disabled while running), Edit, Delete, View Output (while running or after failure). ViewModel tests per state.
- [ ] **S6.3 Add Job flow:** temporary tile (marked as new) + editor opens in side panel; Save persists (regular tile), Discard removes. ViewModel tests; no dialogs.
- [ ] **S6.4 Inline delete confirmation:** confirm state on the tile; confirmed delete removes job + history; cancel reverts. ViewModel tests.

## Phase 7: Side panel - job editor

- [ ] **S7.1 SidePanelViewModel:** single content slot, switches on job selection, user-closable. ViewModel tests.
- [ ] **S7.2 JobEditor:** AvalonEdit, Lua highlighting, line numbers, Save/Discard in panel header (existing: save/revert; new: create/remove). Script round-trips via repository; unsaved changes survive panel switches.
- [ ] **S7.3 Editor safety:** read-only while job runs; syntax validation feedback with line hints.

## Phase 8: Side panel - output viewer

- [ ] **S8.1 Output viewer:** live scrolling display, timestamps, color-coded levels, auto-scroll with lock, buffered updates (UI responsive under heavy output).
- [ ] **S8.2 Viewer commands:** copy-all, clear; opens from running/failed tiles; switches with selection.

## Phase 9: MVP completion

- [ ] **S9.1 End-to-end pass (macOS):** add job -> write script -> dry run -> normal run -> observe output -> restart app -> job persists with history. Fix or ticket findings.
- [ ] **S9.2 Cross-platform verification:** config dirs on Windows/Linux; document rsync availability (macOS/Linux; Windows user-provided).
- [ ] **S9.3 Coverage check** against goals (engine 90+, logic 80+, VMs 70+, services 85+); fill gaps or consciously defer.
- [ ] **S9.4 Packaging:** macOS .app bundle via existing buildtools; version stamped for status bar and About.
- [ ] **S9.5 Spec/backlog review:** reconcile implementation against spec, syntax doc and backlog; update docs or fix code.

## Post-MVP candidates (not part of these phases)

Execution limits/queue/cancellation polish; scheduling, variables, timeouts; plugin
system, templates; enhanced output viewer, output persistence; script debugging and
versioning; folders; signing, permissions; backups; cloud sync.
