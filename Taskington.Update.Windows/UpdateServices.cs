using Taskington.Base;
using Taskington.Base.Extension;

[assembly: TaskingtonExtension(typeof(Taskington.Update.Windows.UpdateServices))]

namespace Taskington.Update.Windows;

public class UpdateServices : ITaskingtonExtension
{
    public object? InitializeEnvironment(IBaseEnvironment baseEnvironment)
    {
        return null;
    }
}
