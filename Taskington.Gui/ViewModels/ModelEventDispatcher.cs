using System;
using System.Linq;
using Taskington.Base.Plans;

namespace Taskington.Gui.ViewModels;

class ModelEventDispatcher
{
    private readonly MainWindowViewModel mainWindowViewModel;

    public ModelEventDispatcher(MainWindowViewModel mainWindowViewModel, IPlanExecution planExecution)
    {
        this.mainWindowViewModel = mainWindowViewModel;

        planExecution.PlanCanExecuteUpdated += (sender, e) => UpdateViewModel(
            e.Plan, model => model.CanExecute = e.CanExecute);
        planExecution.PlanErrorUpdated += (sender, e) => UpdateViewModel(
            e.Plan, model => model.HasErrors = e.HasErrors);
        planExecution.PlanRunningUpdated += (sender, e) => UpdateViewModel(
            e.Plan, model => model.IsRunning = e.Running);
        planExecution.PlanProgressUpdated += (sender, e) => UpdateViewModel(
            e.Plan, model => model.Progress = e.Progress);
        planExecution.PlanStatusTextUpdated += (sender, e) => UpdateViewModel(
            e.Plan, model => model.StatusText = e.StatusText);
    }

    private void UpdateViewModel(Plan plan, Action<PlanViewModel> updater)
    {
        var model = mainWindowViewModel.Plans.FirstOrDefault(model => model.Plan == plan);
        if (model != null)
        {
            updater(model);
        }
    }
}
