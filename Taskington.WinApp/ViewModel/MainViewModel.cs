using Taskington.WinApp.ViewModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Taskington.Base;
using Taskington.Base.Config;
using Taskington.Base.Model;

namespace Taskington.WinApp.ViewModel
{
    class MainViewModel : NotifiableObject
    {
        private readonly ConfigurationManager configurationManager;

        public ObservableCollection<PlanViewModel> Plans { get; }

        public MainViewModel(Base.Application application, ConfigurationManager configurationManager, IApplicationEvents applicationEvents)
        {
            this.configurationManager = configurationManager;

            Plans = new ObservableCollection<PlanViewModel>();
            applicationEvents.ConfigurationReloaded += (sender, e) =>
            {
                UpdatePlanViewModels();
                application.NotifyInitialStates();
            };
            UpdatePlanViewModels();
        }

        private void UpdatePlanViewModels()
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                Plans.Clear();
                foreach (var plan in configurationManager.ExecutablePlans)
                {
                    Plans.Add(new PlanViewModel(plan));
                }
            });
        }
    }
}
