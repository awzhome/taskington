using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PPBackup.WinApp.ViewModel
{
    interface IAsyncCommand<P> : ICommand where P : class
    {
        Task ExecuteAsync(P? parameter);
    }

    public class AsyncRelayCommand<P> : IAsyncCommand<P> where P : class
    {
        private readonly Func<P?, bool>? canExecute;
        private readonly Func<P?, Task> execute;
        private bool isExecuting;

        public event EventHandler? CanExecuteChanged;

        public AsyncRelayCommand(Func<P?, Task> execute)
            : this(execute, null)
        {
        }

        public AsyncRelayCommand(Func<P?, Task> execute, Func<P?, bool>? canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object? parameter)
        {
            return !isExecuting && (canExecute?.Invoke(parameter as P) ?? true);
        }

        public async Task ExecuteAsync(P? parameter)
        {
            if (CanExecute(parameter))
            {
                try
                {
                    isExecuting = true;
                    await execute(parameter);
                }
                finally
                {
                    isExecuting = false;
                }
            }
        }

        public async void Execute(object? parameter)
        {
            try
            {
                await ExecuteAsync(parameter as P);
            }
            catch (Exception ex)
            {
                //handler?.HandleError(ex);
                int test = 0;
            }
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public class AsyncRelayCommand : AsyncRelayCommand<object>
    {
        public AsyncRelayCommand(Func<Task> execute)
            : base(async (_) => await execute(), null)
        {
        }

        public AsyncRelayCommand(Func<Task> execute, Func<bool>? canExecute)
            : base(async (_) => await execute(), (_) => canExecute?.Invoke() ?? false)
        {
        }
    }
}
