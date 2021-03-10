using System;

namespace PPBackup.Base
{
    public interface IApplicationEvents
    {
        event EventHandler? ConfigurationReloaded;
        event EventHandler<ConfigurationReloadDelayEventArgs>? ConfigurationReloadDelayed;
    }

    public class ConfigurationReloadDelayEventArgs : EventArgs
    {
        public bool IsDelayed { get; set; }
    }

    public class ApplicationEvents : IApplicationEvents
    {
        public event EventHandler? ConfigurationChanged;
        public event EventHandler? ConfigurationReloaded;
        public event EventHandler<ConfigurationReloadDelayEventArgs>? ConfigurationReloadDelayed;

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

        public ApplicationEvents ConfigurationReloadDelay(bool isDelayed)
        {
            ConfigurationReloadDelayed?.Invoke(this, new ConfigurationReloadDelayEventArgs() { IsDelayed = isDelayed });
            return this;
        }
    }
}
