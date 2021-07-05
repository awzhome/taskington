using Avalonia.Threading;
using ReactiveUI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Taskington.Base;
using Taskington.Base.Config;
using Taskington.Base.Plans;

namespace Taskington.Gui.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly Application application;
        private readonly ConfigurationManager configurationManager;
        public ObservableCollection<PlanViewModel> Plans { get; }

        public ReactiveCommand<Unit, Unit> AddPlanCommand { get; }
        public ReactiveCommand<PlanViewModel, Unit> ExecutePlanCommand { get; }
        public Interaction<EditPlanViewModel, bool> ShowPlanEditDialog { get; }
        public ReactiveCommand<PlanViewModel, Unit> EditPlanCommand { get; }
        public ReactiveCommand<PlanViewModel, Unit> RemovePlanCommand { get; }

        public MainWindowViewModel(Application application, ConfigurationManager configurationManager, IApplicationEvents applicationEvents)
        {
            this.application = application;
            this.configurationManager = configurationManager;

            Plans = new ObservableCollection<PlanViewModel>();
            applicationEvents.ConfigurationReloaded += (sender, e) =>
            {
                UpdatePlanViewModels();
            };
            UpdatePlanViewModels();

            AddPlanCommand = ReactiveCommand.CreateFromTask(AddPlanAsync);
            ExecutePlanCommand = ReactiveCommand.CreateFromTask<PlanViewModel>(ExecutePlanAsync);
            ShowPlanEditDialog = new();
            EditPlanCommand = ReactiveCommand.CreateFromTask<PlanViewModel>(EditPlanAsync);
            RemovePlanCommand = ReactiveCommand.Create<PlanViewModel>(RemovePlan);
        }

        private void UpdatePlanViewModels()
        {
            Dispatcher.UIThread.Post(() =>
            {
                Plans.Clear();
                foreach (var plan in configurationManager.ExecutablePlans)
                {
                    Plans.Add(CreatePlanViewModel(plan));
                }
                application.NotifyInitialStates();
            });
        }

        private PlanViewModel CreatePlanViewModel(ExecutablePlan executablePlan) =>
            new(executablePlan, ExecutePlanCommand, EditPlanCommand, RemovePlanCommand);

        private async Task ExecutePlanAsync(PlanViewModel planViewModel)
        {
            await planViewModel.Execution.ExecuteAsync();
        }

        private async Task AddPlanAsync()
        {
            var newPlan = new Plan(Plan.OnSelectionRunType) { Name = "New plan" };
            var newExecutablePlan = configurationManager.InsertPlan(Plans.Count, newPlan);
            configurationManager.SaveConfiguration();
            var newPlanViewModel = CreatePlanViewModel(newExecutablePlan);
            Plans.Add(newPlanViewModel);
            newExecutablePlan.Execution.NotifyInitialStates();
            await EditPlanAsync(newPlanViewModel);
        }

        private async Task EditPlanAsync(PlanViewModel planViewModel)
        {
            var editPlanViewModel = new EditPlanViewModel(planViewModel);
            var shouldSave = await ShowPlanEditDialog.Handle(editPlanViewModel);
            if (shouldSave)
            {
                var newExecutablePlan = configurationManager.ReplacePlan(planViewModel.ExecutablePlan, editPlanViewModel.ConvertToPlan());
                int existingIndex = Plans.IndexOf(planViewModel);
                if (existingIndex >= 0)
                {
                    Plans[existingIndex] = new PlanViewModel(newExecutablePlan, ExecutePlanCommand, EditPlanCommand, RemovePlanCommand);
                    newExecutablePlan.Execution.NotifyInitialStates();
                }
                configurationManager.SaveConfiguration();
            }
        }

        private void RemovePlan(PlanViewModel planViewModel)
        {
            Plans.Remove(planViewModel);
            configurationManager.RemovePlan(planViewModel.ExecutablePlan);
            configurationManager.SaveConfiguration();
        }
    }
}
