using System;
using System.Threading.Tasks;

namespace Taskington.Base.TinyBus.Endpoints;

public abstract class AsyncOneWayMessageEndPoint<F, P> : TwoWayMessageEndPoint<F, P, Task>
    where F : notnull
    where P : notnull
{
    protected async Task Invoke(Func<F, Task> invoker, Func<P, bool> predicateInvoker) =>
        await Task.WhenAll(InvokeAndCollect(invoker, predicateInvoker));
}
