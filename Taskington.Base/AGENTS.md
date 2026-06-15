# Taskington.Base guide

- **Purpose:** core task engine, plans, steps, configuration, logging, and non-UI system operations live here.
- **Basic approach:** persist behavior as YAML-backed plans and typed `PlanStep` objects; execute work by resolving `IStepExecution` implementations from the keyed registry.
- **Rule of thumb:** if a change is not specific to Avalonia or desktop presentation, prefer implementing it here.
- **Extensibility:** new step types should register themselves in the base environment and participate in plan pre-check/execution through the existing extension and registry pattern.
- **Keep it clean:** do not add GUI dependencies or view-model concerns to this project.
- **Tests:** when behavior changes here, add or update matching xUnit coverage in `../Taskington.Base.Tests`.
