using System;
using System.Collections.Generic;
using System.Linq;

namespace Taskington.Base.TinyBus
{
    public abstract class TwoWayMessage<F, P, R>
    {
        private readonly List<SubscriberInfo<F, P>> subscriptions = new();

        public void Subscribe(F subscriber)
        {
            subscriptions.Add(new SubscriberInfo<F, P>(subscriber));
        }

        public void Subscribe(F subscriber, P predicate)
        {
            subscriptions.Add(new SubscriberInfo<F, P>(subscriber, predicate));
        }

        protected IEnumerable<R> InvokeAndCollect(Func<F, R> invoker, Func<P, bool> predicateInvoker) =>
            subscriptions
                .Where(subscriberInfo => (subscriberInfo.Predicate == null) || predicateInvoker(subscriberInfo.Predicate))
                .Select(subscriberInfo => invoker(subscriberInfo.Subscriber));
    }
}
