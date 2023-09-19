using Taskington.Base.TinyBus;

namespace Taskington.Base.Extension
{
    public interface ITaskingtonExtension
    {
        object? InitializeEnvironment(IBaseEnvironment baseEnvironment);
    }
}
