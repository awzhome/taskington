using System;

namespace PPBackup.Base
{
    public interface IApplicationEvents
    {
        event EventHandler? ConfigurationReloaded;
    }

    public class ApplicationEvents : IApplicationEvents
    {
        public event EventHandler? ConfigurationChanged;
        public event EventHandler? ConfigurationReloaded;

        public ApplicationEvents ConfigurationChange()
        {
            ConfigurationChanged?.Invoke(this, new EventArgs());
            return this;
        }

        public ApplicationEvents ConfigurationReload()
        {
            ConfigurationReloaded?.Invoke(this, new EventArgs());
            return this;
        }
    }
}
