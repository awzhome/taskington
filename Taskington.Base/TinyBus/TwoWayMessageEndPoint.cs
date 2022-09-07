using System;
using System.Collections.Generic;
using System.Linq;

namespace Taskington.Base.TinyBus;

public abstract class TwoWayMessage<F, P, R> : Subscribable<F, P>
    where F : notnull
    where P : notnull
{
    protected IEnumerable<R> InvokeAndCollect(Func<F, R> invoker, Func<P, bool> predicateInvoker) =>
        Subscriptions
            .Where(subscriberInfo => (subscriberInfo.Predicate == null) || predicateInvoker(subscriberInfo.Predicate))
            .Select(subscriberInfo => invoker(subscriberInfo.Subscriber));
}
