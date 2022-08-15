using System;
using System.Threading.Tasks;

namespace Taskington.Base.TinyBus
{
    public abstract class AsyncOneWayMessage<F, P> : TwoWayMessage<F, P, Task>
    {
        protected async Task Invoke(Func<F, Task> invoker, Func<P, bool> predicateInvoker) =>
            await Task.WhenAll(InvokeAndCollect(invoker, predicateInvoker));
    }
}
