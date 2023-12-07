using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Taskington.Gui.ViewModels;
using System;

namespace Taskington.Gui
{
    class ViewLocator : IDataTemplate
    {
        public bool SupportsRecycling => false;

        public Control Build(object? data)
        {
            var name = data?.GetType().FullName!.Replace("ViewModel", "View");
            if (name is not null)
            {
                var type = Type.GetType(name);
                if (type is not null)
                {
                    return (Control) Activator.CreateInstance(type)!;
                }
            }

            return new TextBlock { Text = "Not Found: " + name };
        }

        public bool Match(object? data)
        {
            return data is ViewModelBase;
        }
    }
}
