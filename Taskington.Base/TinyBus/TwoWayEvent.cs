using System;
using System.Collections.Generic;
using System.Linq;

namespace Taskington.Base.TinyBus
{
    public abstract class TwoWayEvent<T, R>
    {
        private readonly List<T> subscriptions = new();

        public void Subscribe(T subscriber)
        {
            subscriptions.Add(subscriber);
        }

        protected IEnumerable<R> InvokeAndCollect(Func<T, R> invoker) =>
            subscriptions.Select(subscriber => invoker(subscriber));
    }
}
