using System;
using System.Linq;
using Taskington.Base.Events;
using Taskington.Base.Plans;
using Taskington.Base.Service;

namespace Taskington.Gui.ViewModels
{
    class ModelEventDispatcher : IAutoInitializable
    {
        private readonly IApplicationEvents events;
        private readonly MainWindowViewModel mainWindowViewModel;

        public ModelEventDispatcher(IApplicationEvents events, MainWindowViewModel mainWindowViewModel)
        {
            this.events = events;
            this.mainWindowViewModel = mainWindowViewModel;
        }

        public void Initialize()
        {
            events.PlanCanExecuteUpdated += (o, e) => UpdateViewModel(e.Plan, m => m.CanExecute = e.CanExecute);
            events.PlanHasErrorsUpdated += (o, e) => UpdateViewModel(e.Plan, m => m.HasErrors = e.HasErrors);
            events.PlanIsRunningUpdated += (o, e) => UpdateViewModel(e.Plan, m => m.IsRunning = e.IsRunning);
            events.PlanProgressUpdated += (o, e) => UpdateViewModel(e.Plan, m => m.Progress = e.Progress);
            events.PlanStatusTextUpdated += (o, e) => UpdateViewModel(e.Plan, m => m.StatusText = e.StatusText);
        }

        private void UpdateViewModel(Plan plan, Action<PlanViewModel> updater)
        {
            var model = mainWindowViewModel.Plans.FirstOrDefault(model => model.ExecutablePlan.Plan == plan);
            if (model != null)
            {
                updater(model);
            }
        }
    }
}
