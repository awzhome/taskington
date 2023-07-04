using System.Collections.Generic;
using Taskington.Base.TinyBus;

namespace Taskington.Base.Extension
{
    public interface IHandlerStore
    {
        void Add(params object?[] handlers);
    }

    internal class ExtensionHandlerStore : IHandlerStore
    {
        private readonly List<object?> storedHandlers = new();

        public void Add(params object?[] handlers)
        {
            foreach (var handler in handlers)
            {
                storedHandlers.Add(handler);
                if (handler is not null)
                {
                    DeclarativeSubscriptions.SubscribeAsDeclared(handler);
                }
            }
        }


    }
}
