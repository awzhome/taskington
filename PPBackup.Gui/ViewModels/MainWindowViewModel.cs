using Avalonia.Threading;
using PPBackup.Base;
using PPBackup.Base.Config;
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
        private readonly ConfigurationManager configurationManager;
        public ObservableCollection<BackupPlanViewModel> BackupPlans { get; }

        public ReactiveCommand<BackupPlanViewModel, Unit> ExecutePlanCommand { get; }
        public Interaction<EditBackupPlanViewModel, bool> ShowPlanEditDialog { get; }
        public ReactiveCommand<BackupPlanViewModel, Unit> EditPlanCommand { get; }
        public ReactiveCommand<BackupPlanViewModel, Unit> RemovePlanCommand { get; }

        public MainWindowViewModel(Application application, ConfigurationManager configurationManager, IApplicationEvents applicationEvents)
        {
            this.application = application;
            this.configurationManager = configurationManager;

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
                foreach (var plan in configurationManager.ExecutablePlans)
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
            if (shouldSave)
            {
                var newExecutablePlan = configurationManager.ReplacePlan(backupPlanViewModel.ExecutablePlan, editPlanViewModel.ConvertToPlan());
                int existingIndex = BackupPlans.IndexOf(backupPlanViewModel);
                if (existingIndex >= 0)
                {
                    BackupPlans[existingIndex] = new BackupPlanViewModel(newExecutablePlan, ExecutePlanCommand, EditPlanCommand, RemovePlanCommand);
                    newExecutablePlan.Execution.NotifyInitialStates();
                }
                configurationManager.SaveConfiguration();
            }
        }

        private void RemovePlan(BackupPlanViewModel backupPlanViewModel)
        {
            BackupPlans.Remove(backupPlanViewModel);
            configurationManager.RemovePlan(backupPlanViewModel.ExecutablePlan);
            configurationManager.SaveConfiguration();
        }
    }
}
