using MahApps.Metro.Controls.Dialogs;
using System;
using System.Threading.Tasks;

namespace PPBackup.WinApp.ViewModel
{
    public class DialogController
    {
        private readonly IDialogCoordinator dialogCoordinator;
        private readonly MainViewModel mainViewModel;

        public DialogController(MainViewModel mainViewModel)
        {
            dialogCoordinator = DialogCoordinator.Instance;
            this.mainViewModel = mainViewModel;
        }

        public AsyncRelayCommand OpenDialogCommand(Func<BaseMetroDialog> dialogCreatorFunc, Func<bool> canExecute)
        {
            return new(async () =>
                    await dialogCoordinator.ShowMetroDialogAsync(mainViewModel, dialogCreatorFunc()),
                canExecute);
        }

        public AsyncRelayCommand<BaseMetroDialog> HideDialogCommand()
        {
            return new(async (dialog) => await HideDialog(dialog));
        }

        public async Task HideDialog(BaseMetroDialog? dialog)
        {
            await dialogCoordinator.HideMetroDialogAsync(mainViewModel, dialog);
        }
    }
}
