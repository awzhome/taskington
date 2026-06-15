# Repository guide

This file is the default guide for the whole repository. More specific `AGENTS.md` files in subdirectories override it for their scope.

- **Project split:** `Taskington.Base` is the core engine, `Taskington.Gui` is the Avalonia desktop app, `Taskington.Gui.Extension` holds UI-facing contracts, and `Taskington.Base.Tests` contains xUnit coverage for base behavior.
- **Boundaries:** keep reusable task/config/runtime logic in `Taskington.Base`; keep Avalonia and desktop-only concerns in `Taskington.Gui`; move shared GUI abstractions to `Taskington.Gui.Extension`.
- **Basic approach:** the app is configuration-driven. Plans contain typed steps, step behavior is looked up by string key, and features plug themselves into keyed registries instead of a DI container.
- **Composition:** environments are wired manually in constructors (`BaseEnvironment`, `GuiEnvironment`); prefer small explicit collaborators over service-locator style access.
- **Conventions:** follow `.editorconfig` (`4` spaces; `CRLF` for `*.cs`) and central package versions in `Directory.Packages.props`.
- **Validation:** prefer targeted `dotnet test` runs first; use `dotnet restore Taskington.sln && dotnet test Taskington.sln --no-restore` for repo-wide validation.
