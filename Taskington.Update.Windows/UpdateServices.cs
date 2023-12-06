using Taskington.Base;
using Taskington.Base.Extension;

[assembly: TaskingtonExtension(typeof(Taskington.Update.Windows.UpdateServices))]

namespace Taskington.Update.Windows;

public class UpdateServices : ITaskingtonExtension<IBaseEnvironment>
{
    public object? InitializeEnvironment(IBaseEnvironment baseEnvironment)
    {
        return null;
    }
}
