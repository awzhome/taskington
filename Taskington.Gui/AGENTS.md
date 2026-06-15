# Taskington.Gui guide

- **Purpose:** Avalonia desktop frontend for Taskington.
- **Basic approach:** the GUI bootstraps `Taskington.Base.Application`, loads the GUI assembly as an extension, and builds view models directly from the exposed environment services.
- **Structure:** keep presentation split across `Views`, `ViewModels`, `Controls`, and `UIProviders`; keep app bootstrap in `Program.cs` and `App.xaml*`.
- **Step UI pattern:** each step type gets an `IStepUi` implementation registered by key; keep creation of edit models and new-step templates there.
- **Boundary:** if code needs to be reused by GUI integrations or step UIs, prefer `../Taskington.Gui.Extension` instead of concrete app code here.
- **Changes:** keep UI edits consistent across XAML, code-behind, and the related view model when applicable.
