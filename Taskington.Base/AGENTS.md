# AGENTS.md - Taskington.Base

Business logic, domain models, storage and the Lua scripting engine. Root `AGENTS.md`
applies; authoritative behavior in `spec/PROJECT_SPEC.md` chapters 4, 6-10.

## Boundaries

- Never reference Avalonia or any UI framework, nor `Taskington.App`. Must stay buildable
  and testable headless.
- Services register via a DI extension method; constructor injection everywhere.

## Architecture (built up per backlog)

- Models: `Job`, `JobState`, `ExecutionMode` (Normal/Dry), `JobExecution`,
  `ExecutionResult`, `ScriptValidationResult`.
- `JobRepository`: one JSON file per job under the platform config dir; GUID id = file
  name; atomic writes (temp file + rename). `JobHistoryService`: execution metadata
  only, never output.
- `LuaScriptEngine` wraps LuaCSharp; `ScriptSandbox` builds a fresh Lua environment per
  execution; `TaskingtonLuaApi` registers exactly the functions defined in
  `spec/JOB_SYNTAX.md` - nothing more.
- `ScriptExecutor`: one script per background task, cancellable. `JobExecutorService`:
  jobs run independently in parallel (no MVP limits); the same job never twice
  concurrently.

## Security (non-negotiable)

- Zero-trust sandbox: standard Lua libraries (`io`, `os`, `debug`, package loading,
  ...) never registered or reachable; scripts may only call Taskington-provided
  functions.
- Every script action (commands, file sync, ...) goes through the `IActionExecutor`
  abstraction - no direct `Process`/filesystem calls in Lua API functions. Dry mode
  depends on this; do not bypass it.
- Any sandbox or Lua API change requires an escape-attempt security test in
  `Taskington.Base.Tests`.

## Dry mode

- `ExecutionMode.Dry` runs a script with a recording executor that logs "would do ..."
  entries instead of acting. The whole pipeline is shared with Normal mode; only the
  bound `IActionExecutor` differs. New API functions are tested in both modes.

## Conventions

- Serilog logging; verbose level for development; no user-facing log configuration in
  MVP.
- Execution paths never block callers; output/progress published as events with a
  bounded in-memory buffer.
- MVP: no timeouts, no output persistence, no backups, no scheduling.
- Internal paths come from the platform directory service (macOS
  `~/Library/Application Support/Taskington/`, Linux `$XDG_CONFIG_HOME/taskington/`,
  Windows `%APPDATA%\Taskington\`). Paths inside job scripts are the user's
  responsibility - never rewrite them.
- xUnit tests in `Taskington.Base.Tests` are written together with the code.
