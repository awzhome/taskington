# Taskington.Gui.Extension guide

- **Purpose:** shared GUI contracts, step UI abstractions, and cross-project UI-facing types live here.
- **Basic approach:** this project is the seam between core execution and Avalonia presentation; contracts here let GUI features plug in by step type without referencing concrete views from the base layer.
- **Boundary:** keep this project reusable by avoiding concrete app bootstrap, windowing, or view implementation details.
- **Dependencies:** prefer interfaces and lightweight models here; push app-specific behavior down into `../Taskington.Gui`.
