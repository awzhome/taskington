using System;
using System.Linq;
using Taskington.Base.Plans;

namespace Taskington.Gui.ViewModels
{
    class ModelEventDispatcher
    {
        private readonly MainWindowViewModel mainWindowViewModel;

        public ModelEventDispatcher(MainWindowViewModel mainWindowViewModel)
        {
            this.mainWindowViewModel = mainWindowViewModel;

            PlanCanExecuteUpdateMessage.Subscribe(message => UpdateViewModel(
                message.Plan, model => model.CanExecute = message.CanExecute));
            PlanErrorUpdateMessage.Subscribe(message => UpdateViewModel(
                message.Plan, model => model.HasErrors = message.HasErrors));
            PlanRunningUpdateMessage.Subscribe(message => UpdateViewModel(
                message.Plan, model => model.IsRunning = message.IsRunning));
            PlanProgressUpdateMessage.Subscribe(message => UpdateViewModel(
                message.Plan, model => model.Progress = message.Progress));
            PlanStatusTextUpdateMessage.Subscribe(message => UpdateViewModel(
                message.Plan, model => model.StatusText = message.StatusText));
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
}
