using System;

namespace PPBackup.Base
{
    public interface IApplicationEvents
    {
        event EventHandler? ConfigurationChanged;
    }

    public class ApplicationEvents : IApplicationEvents
    {
        public event EventHandler? ConfigurationChanged;

        public ApplicationEvents ConfigurationChange()
        {
            ConfigurationChanged?.Invoke(this, new EventArgs());
            return this;
        }
    }
}
