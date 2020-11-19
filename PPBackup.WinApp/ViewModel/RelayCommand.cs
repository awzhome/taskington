using System;
using System.Windows.Input;

namespace PPBackup.WinApp.ViewModel
{
    public class RelayCommand : ICommand
    {
        private readonly Func<bool>? canExecute;
        private readonly Action execute;

        public event EventHandler? CanExecuteChanged;

        public RelayCommand(Action execute)
            : this(execute, null)
        {
        }

        public RelayCommand(Action execute,
                       Func<bool>? canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) => CanExecute();

        public bool CanExecute() => canExecute?.Invoke() ?? true;

        public void Execute(object? parameter)
        {
            if (CanExecute())
            {
                execute();
            }
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
