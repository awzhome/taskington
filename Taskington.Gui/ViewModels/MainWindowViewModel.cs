using Avalonia.Threading;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Taskington.Base.Config;
using Taskington.Base.Plans;
using Taskington.Gui.Extension;

namespace Taskington.Gui.ViewModels
{
    class MainWindowViewModel : ViewModelBase
    {
        public ModelEventDispatcher modelEventDispatcher;

        public ObservableCollection<PlanViewModel> Plans { get; }
        public ObservableCollection<AppNotification> AppNotifications { get; }

        public ReactiveCommand<Unit, Unit> AddPlanCommand { get; }
        public ReactiveCommand<PlanViewModel, Unit> ExecutePlanCommand { get; }
        public Interaction<EditPlanViewModel, bool> ShowPlanEditDialog { get; }
        public ReactiveCommand<PlanViewModel, Unit> EditPlanCommand { get; }
        public ReactiveCommand<PlanViewModel, Unit> RemovePlanCommand { get; }
        public ReactiveCommand<PlanViewModel, Unit> UndoPlanRemovalCommand { get; }

        public MainWindowViewModel()
        {
            modelEventDispatcher = new(this);

            AppNotifications = new ObservableCollection<AppNotification>();

            Plans = new ObservableCollection<PlanViewModel>();
            ConfigurationEvents.ConfigurationReloaded.Subscribe(UpdatePlanViewModels);
            UpdatePlanViewModels();

            AddPlanCommand = ReactiveCommand.CreateFromTask(AddPlan);
            ExecutePlanCommand = ReactiveCommand.Create<PlanViewModel>(ExecutePlan);
            ShowPlanEditDialog = new();
            EditPlanCommand = ReactiveCommand.CreateFromTask<PlanViewModel>(EditPlan);
            RemovePlanCommand = ReactiveCommand.Create<PlanViewModel>(RemovePlan);
            UndoPlanRemovalCommand = ReactiveCommand.Create<PlanViewModel>(UndoPlanRemoval);

            AppNotifications.Add(new AppNotification()
            {
                NotificationType = AppNotificationType.AppInfo,
                LeftText = AppInfo.Copyright,
                RightText = $"v{AppInfo.Version}"
            });

            ConfigurationEvents.InitializeConfiguration.Push();
        }

        private void UpdatePlanViewModels()
        {
            Dispatcher.UIThread.Post(() =>
            {
                var removedPlanModels = Plans.Where(plan => plan.IsRemoved);

                Plans.Clear();
                var plans = ConfigurationEvents.GetPlans.Request().SelectMany(v => v);
                foreach (var plan in plans)
                {
                    Plans.Add(CreatePlanViewModel(plan));
                    PlanEvents.NotifyInitialPlanStates.Push(plan);
                }
                foreach (var plan in removedPlanModels)
                {
                    Plans.Insert(plan.PreviousIndex, plan);
                }
            });
        }

        private PlanViewModel CreatePlanViewModel(Plan plan) =>
            new(plan, ExecutePlanCommand, EditPlanCommand, RemovePlanCommand, UndoPlanRemovalCommand);

        private void ExecutePlan(PlanViewModel planViewModel)
        {
            PlanEvents.ExecutePlan.Push(planViewModel.Plan);
        }

        private async Task AddPlan()
        {
            var newPlan = new Plan(Plan.OnSelectionRunType) { Name = "New plan" };
            ConfigurationEvents.InsertPlan.Push(Plans.Count, newPlan);
            ConfigurationEvents.SaveConfiguration.Push();
            var newPlanViewModel = CreatePlanViewModel(newPlan);
            Plans.Add(newPlanViewModel);
            PlanEvents.NotifyInitialPlanStates.Push(newPlan);
            await EditPlan(newPlanViewModel);
        }

        private async Task EditPlan(PlanViewModel planViewModel)
        {
            var editPlanViewModel = new EditPlanViewModel(planViewModel);
            var shouldSave = await ShowPlanEditDialog.Handle(editPlanViewModel);
            if (shouldSave)
            {
                var editedPlan = editPlanViewModel.ConvertToPlan();
                ConfigurationEvents.ReplacePlan.Push(planViewModel.Plan, editedPlan);
                int existingIndex = Plans.IndexOf(planViewModel);
                if (existingIndex >= 0)
                {
                    Plans[existingIndex] = new PlanViewModel(editedPlan, ExecutePlanCommand, EditPlanCommand, RemovePlanCommand, UndoPlanRemovalCommand);
                    PlanEvents.NotifyInitialPlanStates.Push(editedPlan);
                }
                ConfigurationEvents.SaveConfiguration.Push();
            }
        }

        private void RemovePlan(PlanViewModel planViewModel)
        {
            planViewModel.IsRemoved = true;
            planViewModel.PreviousIndex = Plans.IndexOf(planViewModel);
            ConfigurationEvents.RemovePlan.Push(planViewModel.Plan);
            ConfigurationEvents.SaveConfiguration.Push();
        }

        private void UndoPlanRemoval(PlanViewModel planViewModel)
        {
            Plans.Remove(planViewModel);
            ConfigurationEvents.InsertPlan.Push(planViewModel.PreviousIndex, planViewModel.Plan);
            Plans.Insert(
                planViewModel.PreviousIndex,
                new PlanViewModel(planViewModel.Plan, ExecutePlanCommand, EditPlanCommand, RemovePlanCommand, UndoPlanRemovalCommand));
            PlanEvents.NotifyInitialPlanStates.Push(planViewModel.Plan);
            ConfigurationEvents.SaveConfiguration.Push();
        }
    }
}
