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
            ConfigurationReloadedMessage.Subscribe(_ => UpdatePlanViewModels());
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

            new InitializeConfigurationMessage().Publish();
        }

        private void UpdatePlanViewModels()
        {
            Dispatcher.UIThread.Post(() =>
            {
                var removedPlanModels = Plans.Where(plan => plan.IsRemoved);

                Plans.Clear();
                var plans = new GetPlansMessage().Request().SelectMany(v => v);
                foreach (var plan in plans)
                {
                    Plans.Add(CreatePlanViewModel(plan));
                    new NotifyInitialPlanStatesMessage(plan).Publish();
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
            new ExecutePlanMessage(planViewModel.Plan).Publish();
        }

        private async Task AddPlan()
        {
            var newPlan = new Plan(Plan.OnSelectionRunType) { Name = "New plan" };
            new InsertPlanMessage(Plans.Count, newPlan).Publish();
            new SaveConfigurationMessage().Publish();
            var newPlanViewModel = CreatePlanViewModel(newPlan);
            Plans.Add(newPlanViewModel);
            new NotifyInitialPlanStatesMessage(newPlan).Publish();
            await EditPlan(newPlanViewModel);
        }

        private async Task EditPlan(PlanViewModel planViewModel)
        {
            var editPlanViewModel = new EditPlanViewModel(planViewModel);
            var shouldSave = await ShowPlanEditDialog.Handle(editPlanViewModel);
            if (shouldSave)
            {
                var editedPlan = editPlanViewModel.ConvertToPlan();
                new ReplacePlanMessage(planViewModel.Plan, editedPlan).Publish();
                int existingIndex = Plans.IndexOf(planViewModel);
                if (existingIndex >= 0)
                {
                    Plans[existingIndex] = new PlanViewModel(editedPlan, ExecutePlanCommand, EditPlanCommand, RemovePlanCommand, UndoPlanRemovalCommand);
                    new NotifyInitialPlanStatesMessage(editedPlan).Publish();
                }
                new SaveConfigurationMessage().Publish();
            }
        }

        private void RemovePlan(PlanViewModel planViewModel)
        {
            planViewModel.IsRemoved = true;
            planViewModel.PreviousIndex = Plans.IndexOf(planViewModel);
            new RemovePlanMessage(planViewModel.Plan).Publish();
            new SaveConfigurationMessage().Publish();
        }

        private void UndoPlanRemoval(PlanViewModel planViewModel)
        {
            Plans.Remove(planViewModel);
            new InsertPlanMessage(planViewModel.PreviousIndex, planViewModel.Plan).Publish();
            Plans.Insert(
                planViewModel.PreviousIndex,
                new PlanViewModel(planViewModel.Plan, ExecutePlanCommand, EditPlanCommand, RemovePlanCommand, UndoPlanRemovalCommand));
            new NotifyInitialPlanStatesMessage(planViewModel.Plan).Publish();
            new SaveConfigurationMessage().Publish();
        }
    }
}
