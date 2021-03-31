using Avalonia.Threading;
using PPBackup.Base;
using PPBackup.Base.Plans;
using ReactiveUI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace PPBackup.Gui.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly Application application;
        private readonly IEnumerable<ExecutableBackupPlan> executableBackupPlans;
        public ObservableCollection<BackupPlanViewModel> BackupPlans { get; }

        public ReactiveCommand<BackupPlanViewModel, Unit> ExecutePlanCommand { get; }
        public Interaction<EditBackupPlanViewModel, bool> ShowPlanEditDialog { get; }
        public ReactiveCommand<BackupPlanViewModel, Unit> EditPlanCommand { get; }
        public ReactiveCommand<BackupPlanViewModel, Unit> RemovePlanCommand { get; }

        public MainWindowViewModel(Application application, IEnumerable<ExecutableBackupPlan> executableBackupPlans, IApplicationEvents applicationEvents)
        {
            this.application = application;
            this.executableBackupPlans = executableBackupPlans;

            BackupPlans = new ObservableCollection<BackupPlanViewModel>();
            applicationEvents.ConfigurationReloaded += (sender, e) =>
            {
                UpdatePlanViewModels();
            };
            UpdatePlanViewModels();

            ExecutePlanCommand = ReactiveCommand.CreateFromTask<BackupPlanViewModel>(ExecutePlanAsync);
            ShowPlanEditDialog = new();
            EditPlanCommand = ReactiveCommand.CreateFromTask<BackupPlanViewModel>(EditPlanAsync);
            RemovePlanCommand = ReactiveCommand.Create<BackupPlanViewModel>(RemovePlan);
        }

        private void UpdatePlanViewModels()
        {
            Dispatcher.UIThread.Post(() =>
            {
                BackupPlans.Clear();
                foreach (var plan in executableBackupPlans)
                {
                    BackupPlans.Add(new BackupPlanViewModel(plan, ExecutePlanCommand, EditPlanCommand, RemovePlanCommand));
                }
                application.NotifyInitialStates();
            });
        }

        private async Task ExecutePlanAsync(BackupPlanViewModel backupPlanViewModel)
        {
            await backupPlanViewModel.Execution.ExecuteAsync();
        }

        private async Task EditPlanAsync(BackupPlanViewModel backupPlanViewModel)
        {
            var editPlanViewModel = new EditBackupPlanViewModel(backupPlanViewModel);
            var shouldSave = await ShowPlanEditDialog.Handle(editPlanViewModel);
        }

        private void RemovePlan(BackupPlanViewModel plan)
        {
            // TODO
        }
    }
}
