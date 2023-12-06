using Avalonia.Threading;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Taskington.Base.Config;
using Taskington.Base.Extension;
using Taskington.Base.Plans;
using Taskington.Base.SystemOperations;
using Taskington.Gui.Extension;

namespace Taskington.Gui.ViewModels;

class MainWindowViewModel : ViewModelBase
{
    private ModelEventDispatcher modelEventDispatcher;
    private readonly IConfigurationManager configurationManager;
    private readonly IPlanExecution planExecution;
    private readonly ISystemOperations systemOperations;
    private readonly IKeyedRegistry<IStepUI> stepUIs;
    private readonly IAppNotificationViewModel appNotificationViewModel;


    public ObservableCollection<PlanViewModel> Plans { get; }

    public ReactiveCommand<Unit, Unit> AddPlanCommand { get; }
    public ReactiveCommand<PlanViewModel, Unit> ExecutePlanCommand { get; }
    public Interaction<EditPlanViewModel, bool> ShowPlanEditDialog { get; }
    public ReactiveCommand<PlanViewModel, Unit> EditPlanCommand { get; }
    public ReactiveCommand<PlanViewModel, Unit> RemovePlanCommand { get; }
    public ReactiveCommand<PlanViewModel, Unit> UndoPlanRemovalCommand { get; }

    public ObservableCollection<AppNotification> AppNotifications => appNotificationViewModel.Notifications;

    public MainWindowViewModel(
        IConfigurationManager configurationManager,
        IPlanExecution planExecution,
        ISystemOperations systemOperations,
        IKeyedRegistry<IStepUI> stepUIs,
        IAppNotificationViewModel appNotificationViewModel)
    {
        this.appNotificationViewModel = appNotificationViewModel;

        modelEventDispatcher = new(this, planExecution);

        Plans = new ObservableCollection<PlanViewModel>();
        configurationManager.ConfigurationReloaded += (s, e) => UpdatePlanViewModels();

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
            RightText = $"v{GitVersionInformation.SemVer}"
        });

        configurationManager.Initialize();

        this.configurationManager = configurationManager;
        this.planExecution = planExecution;
        this.systemOperations = systemOperations;
        this.stepUIs = stepUIs;
    }

    private void UpdatePlanViewModels()
    {
        Dispatcher.UIThread.Post(() =>
        {
            var removedPlanModels = Plans.Where(plan => plan.IsRemoved);

            Plans.Clear();
            var plans = configurationManager.Plans;
            foreach (var plan in plans)
            {
                Plans.Add(CreatePlanViewModel(plan));
                planExecution.NotifyInitialStates(plan);
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
        planExecution.Execute(planViewModel.Plan);
    }

    private async Task AddPlan()
    {
        var newPlan = new Plan(Plan.OnSelectionRunType) { Name = "New plan" };
        configurationManager.InsertPlan(Plans.Count, newPlan);
        configurationManager.SaveConfiguration();
        var newPlanViewModel = CreatePlanViewModel(newPlan);
        Plans.Add(newPlanViewModel);
        planExecution.NotifyInitialStates(newPlan);
        await EditPlan(newPlanViewModel);
    }

    private async Task EditPlan(PlanViewModel planViewModel)
    {
        var editPlanViewModel = new EditPlanViewModel(systemOperations, stepUIs, planViewModel);
        var shouldSave = await ShowPlanEditDialog.Handle(editPlanViewModel);
        if (shouldSave)
        {
            var editedPlan = editPlanViewModel.ConvertToPlan();
            configurationManager.ReplacePlan(planViewModel.Plan, editedPlan);
            int existingIndex = Plans.IndexOf(planViewModel);
            if (existingIndex >= 0)
            {
                Plans[existingIndex] = new PlanViewModel(editedPlan, ExecutePlanCommand, EditPlanCommand, RemovePlanCommand, UndoPlanRemovalCommand);
                planExecution.NotifyInitialStates(editedPlan);
            }
            configurationManager.SaveConfiguration();
        }
    }

    private void RemovePlan(PlanViewModel planViewModel)
    {
        planViewModel.IsRemoved = true;
        planViewModel.PreviousIndex = Plans.IndexOf(planViewModel);
        configurationManager.RemovePlan(planViewModel.Plan);
        configurationManager.SaveConfiguration();
    }

    private void UndoPlanRemoval(PlanViewModel planViewModel)
    {
        Plans.Remove(planViewModel);
        configurationManager.InsertPlan(planViewModel.PreviousIndex, planViewModel.Plan);
        Plans.Insert(
            planViewModel.PreviousIndex,
            new PlanViewModel(planViewModel.Plan, ExecutePlanCommand, EditPlanCommand, RemovePlanCommand, UndoPlanRemovalCommand));
        planExecution.NotifyInitialStates(planViewModel.Plan);
        configurationManager.SaveConfiguration();
    }
}
