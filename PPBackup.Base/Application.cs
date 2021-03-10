using PPBackup.Base.Config;
using PPBackup.Base.Plans;
using System.Collections.Generic;
using System.Linq;

namespace PPBackup.Base
{
    public class Application
    {
        public ApplicationServices Services { get; }

        public Application()
        {
            Services = new ApplicationServices();
            BaseServices.Bind(Services);
            Services.With(this);
        }

        public void Start()
        {
            Services.Start();
        }

        public void NotifyInitialStates()
        {
            foreach (var execution in Services.Get<List<ExecutableBackupPlan>>().Select(executablePlan => executablePlan.Execution))
            {
                execution.NotifyInitialStates();
            }
        }
    }
}
