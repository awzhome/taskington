using Avalonia.Threading;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Taskington.Base;
using Taskington.Base.Config;
using Taskington.Base.Events;
using Taskington.Base.Plans;

namespace Taskington.Gui.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly Application application;
        private readonly ConfigurationManager configurationManager;

        public ObservableCollection<PlanViewModel> Plans { get; }
        public ObservableCollection<AppMessage> AppMessages { get; }

        public ReactiveCommand<Unit, Unit> AddPlanCommand { get; }
        public ReactiveCommand<PlanViewModel, Unit> ExecutePlanCommand { get; }
        public Interaction<EditPlanViewModel, bool> ShowPlanEditDialog { get; }
        public ReactiveCommand<PlanViewModel, Unit> EditPlanCommand { get; }
        public ReactiveCommand<PlanViewModel, Unit> RemovePlanCommand { get; }
        public ReactiveCommand<PlanViewModel, Unit> UndoPlanRemovalCommand { get; }

        public MainWindowViewModel(Application application, ConfigurationManager configurationManager, IApplicationEvents applicationEvents)
        {
            this.application = application;
            this.configurationManager = configurationManager;

            AppMessages = new ObservableCollection<AppMessage>();

            Plans = new ObservableCollection<PlanViewModel>();
            applicationEvents.ConfigurationReloaded += (sender, e) =>
            {
                UpdatePlanViewModels();
            };
            UpdatePlanViewModels();

            AddPlanCommand = ReactiveCommand.CreateFromTask(AddPlan);
            ExecutePlanCommand = ReactiveCommand.CreateFromTask<PlanViewModel>(ExecutePlan);
            ShowPlanEditDialog = new();
            EditPlanCommand = ReactiveCommand.CreateFromTask<PlanViewModel>(EditPlan);
            RemovePlanCommand = ReactiveCommand.Create<PlanViewModel>(RemovePlan);
            UndoPlanRemovalCommand = ReactiveCommand.Create<PlanViewModel>(UndoPlanRemoval);

            AppMessages.Add(new AppMessage()
            {
                MessageType = AppMessageType.AppInfo,
                LeftText = AppInfo.Copyright,
                RightText = $"v{AppInfo.Version}"
            });
        }

        private void UpdatePlanViewModels()
        {
            Dispatcher.UIThread.Post(() =>
            {
                var removedPlanModels = Plans.Where(plan => plan.IsRemoved);

                Plans.Clear();
                foreach (var plan in configurationManager.ExecutablePlans)
                {
                    Plans.Add(CreatePlanViewModel(plan));
                }
                foreach (var plan in removedPlanModels)
                {
                    Plans.Insert(plan.PreviousIndex, plan);
                }
                application.NotifyInitialStates();
            });
        }

        private PlanViewModel CreatePlanViewModel(ExecutablePlan executablePlan) =>
            new(executablePlan, ExecutePlanCommand, EditPlanCommand, RemovePlanCommand, UndoPlanRemovalCommand);

        private async Task ExecutePlan(PlanViewModel planViewModel)
        {
            await planViewModel.Execution.Execute();
        }

        private async Task AddPlan()
        {
            var newPlan = new Plan(Plan.OnSelectionRunType) { Name = "New plan" };
            var newExecutablePlan = configurationManager.InsertPlan(Plans.Count, newPlan);
            configurationManager.SaveConfiguration();
            var newPlanViewModel = CreatePlanViewModel(newExecutablePlan);
            Plans.Add(newPlanViewModel);
            newExecutablePlan.Execution.NotifyInitialStates();
            await EditPlan(newPlanViewModel);
        }

        private async Task EditPlan(PlanViewModel planViewModel)
        {
            var editPlanViewModel = new EditPlanViewModel(application.ServiceProvider, planViewModel);
            var shouldSave = await ShowPlanEditDialog.Handle(editPlanViewModel);
            if (shouldSave)
            {
                var newExecutablePlan = configurationManager.ReplacePlan(planViewModel.ExecutablePlan, editPlanViewModel.ConvertToPlan());
                int existingIndex = Plans.IndexOf(planViewModel);
                if (existingIndex >= 0)
                {
                    Plans[existingIndex] = new PlanViewModel(newExecutablePlan, ExecutePlanCommand, EditPlanCommand, RemovePlanCommand, UndoPlanRemovalCommand);
                    newExecutablePlan.Execution.NotifyInitialStates();
                }
                configurationManager.SaveConfiguration();
            }
        }

        private void RemovePlan(PlanViewModel planViewModel)
        {
            planViewModel.IsRemoved = true;
            planViewModel.PreviousIndex = Plans.IndexOf(planViewModel);
            configurationManager.RemovePlan(planViewModel.ExecutablePlan);
            configurationManager.SaveConfiguration();
        }

        private void UndoPlanRemoval(PlanViewModel planViewModel)
        {
            Plans.Remove(planViewModel);
            var newExecutablePlan = configurationManager.InsertPlan(planViewModel.PreviousIndex, planViewModel.ExecutablePlan.Plan);
            Plans.Insert(
                planViewModel.PreviousIndex, 
                new PlanViewModel(newExecutablePlan, ExecutePlanCommand, EditPlanCommand, RemovePlanCommand, UndoPlanRemovalCommand));
            newExecutablePlan.Execution.NotifyInitialStates();
            configurationManager.SaveConfiguration();
        }
    }
}
