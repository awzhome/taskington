using Taskington.Base.Extension;

[assembly: TaskingtonExtension(typeof(Taskington.Update.Windows.UpdateServices))]

namespace Taskington.Update.Windows
{
    public class UpdateServices : ITaskingtonExtension
    {
        public void Initialize(IHandlerStore handlerStore)
        {
        }
    }
}
