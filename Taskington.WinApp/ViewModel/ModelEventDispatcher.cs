using System;
using Taskington.Base.Events;
using Taskington.Base.Plans;
using Taskington.Base.Service;

namespace Taskington.WinApp.ViewModel
{
    class ModelEventDispatcher : IAutoInitializable
    {
        private readonly IApplicationEvents events;
        private readonly MainViewModel model;

        public ModelEventDispatcher(IApplicationEvents events, MainViewModel model)
        {
            this.events = events;
            this.model = model;
        }

        public void Initialize()
        {
            events.PlanCanExecuteUpdated += OnPlanCanExecuteUpdated;
            events.PlanHasErrorsUpdated += OnPlanHasErrorsUpdated;
            events.PlanIsRunningUpdated += OnPlanIsRunningUpdated;
            events.PlanProgressUpdated += OnPlanProgressUpdated;
            events.PlanStatusTextUpdated += OnPlanStatusTextUpdated;
        }

        private PlanViewModel FindPlanViewModel(Plan plan)
        {
            return 
        }

        private void OnPlanStatusTextUpdated(object sender, PlanStatusTextUpdatedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnPlanProgressUpdated(object sender, PlanProgressUpdatedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnPlanIsRunningUpdated(object sender, PlanIsRunningUpdatedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnPlanHasErrorsUpdated(object sender, PlanHasErrorsUpdatedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnPlanCanExecuteUpdated(object sender, PlanCanExecuteUpdatedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
