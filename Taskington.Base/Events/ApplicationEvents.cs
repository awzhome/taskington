using System;
using Taskington.Base.Plans;

namespace Taskington.Base.Events
{
    public interface IApplicationEvents
    {
        event EventHandler? ConfigurationReloaded;
        event EventHandler<ConfigurationReloadDelayEventArgs>? ConfigurationReloadDelayed;

        event EventHandler<PlanProgressUpdatedEventArgs> PlanProgressUpdated;
        event EventHandler<PlanStatusTextUpdatedEventArgs> PlanStatusTextUpdated;
        event EventHandler<PlanCanExecuteUpdatedEventArgs> PlanCanExecuteUpdated;
        event EventHandler<PlanHasErrorsUpdatedEventArgs> PlanHasErrorsUpdated;
        event EventHandler<PlanIsRunningUpdatedEventArgs> PlanIsRunningUpdated;
    }

    public class ApplicationEvents : IApplicationEvents
    {
        public event EventHandler? ConfigurationChanged;
        public event EventHandler? ConfigurationReloaded;
        public event EventHandler<ConfigurationReloadDelayEventArgs>? ConfigurationReloadDelayed;

        public event EventHandler<PlanProgressUpdatedEventArgs>? PlanProgressUpdated;
        public event EventHandler<PlanStatusTextUpdatedEventArgs>? PlanStatusTextUpdated;
        public event EventHandler<PlanCanExecuteUpdatedEventArgs>? PlanCanExecuteUpdated;
        public event EventHandler<PlanHasErrorsUpdatedEventArgs>? PlanHasErrorsUpdated;
        public event EventHandler<PlanIsRunningUpdatedEventArgs>? PlanIsRunningUpdated;

        public ApplicationEvents OnConfigurationChanged()
        {
            ConfigurationChanged?.Invoke(this, new EventArgs());
            return this;
        }

        public ApplicationEvents OnConfigurationReloaded()
        {
            ConfigurationReloaded?.Invoke(this, new EventArgs());
            return this;
        }

        public ApplicationEvents OnConfigurationReloadDelayed(bool isDelayed)
        {
            ConfigurationReloadDelayed?.Invoke(this, new ConfigurationReloadDelayEventArgs() { IsDelayed = isDelayed });
            return this;
        }

        public ApplicationEvents OnPlanProgress(Plan plan, int progress)
        {
            PlanProgressUpdated?.Invoke(this, new PlanProgressUpdatedEventArgs(plan, progress));
            return this;
        }

        public ApplicationEvents OnPlanStatusText(Plan plan, string statusText)
        {
            PlanStatusTextUpdated?.Invoke(this, new PlanStatusTextUpdatedEventArgs(plan, statusText));
            return this;
        }

        public ApplicationEvents OnPlanCanExecute(Plan plan, bool canExecute)
        {
            PlanCanExecuteUpdated?.Invoke(this, new PlanCanExecuteUpdatedEventArgs(plan, canExecute));
            return this;
        }

        public ApplicationEvents OnPlanHasErrors(Plan plan, bool hasErrors, string statusText = "")
        {
            PlanHasErrorsUpdated?.Invoke(this, new PlanHasErrorsUpdatedEventArgs(plan, hasErrors, statusText));
            return this;
        }

        public ApplicationEvents OnPlanIsRunning(Plan plan, bool isRunning)
        {
            PlanIsRunningUpdated?.Invoke(this, new PlanIsRunningUpdatedEventArgs(plan, isRunning));
            return this;
        }
    }
}
