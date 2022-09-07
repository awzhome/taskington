using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Taskington.Base.TinyBus;

public abstract class AsyncTwoWayMessage<F, P, R> : TwoWayMessage<F, P, Task<R>>
    where F : notnull
    where P : notnull
{
    protected new async Task<IEnumerable<R>> InvokeAndCollect(Func<F, Task<R>> invoker, Func<P, bool> predicateInvoker) =>
        await Task.WhenAll(base.InvokeAndCollect(invoker, predicateInvoker));
}
