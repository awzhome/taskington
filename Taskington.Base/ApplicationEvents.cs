using System;

namespace Taskington.Base
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

        public ApplicationEvents OnConfigurationChanged()
        {
            ConfigurationChanged?.Invoke(this, new EventArgs());
            return this;
        }

        public ApplicationEvents OnConfigurationReloaded()
        {
            ConfigurationReloaded?.Invoke(this, new EventArgs());
            return this;
        }

        public ApplicationEvents OnConfigurationReloadDelayed(bool isDelayed)
        {
            ConfigurationReloadDelayed?.Invoke(this, new ConfigurationReloadDelayEventArgs() { IsDelayed = isDelayed });
            return this;
        }
    }
}
