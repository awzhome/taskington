using System.Collections.Generic;
using System.Linq;

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
            storedHandlers.AddRange(handlers);
        }
    }
}
