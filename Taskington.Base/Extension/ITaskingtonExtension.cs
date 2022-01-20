using Taskington.Base.TinyBus;

namespace Taskington.Base.Extension
{
    public interface ITaskingtonExtension
    {
        void Initialize(IHandlerStore handlerStore);
    }
}
