using System;
using System.Collections.Generic;
using System.Linq;

namespace Taskington.Base.TinyBus
{
    public abstract class OneWayEvent<F, P>
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

        protected void Invoke(Action<F> invoker, Predicate<P> predicateInvoker)
        {
            foreach (var subscriberInfo in subscriptions)
            {
                if ((subscriberInfo.Predicate == null) || predicateInvoker(subscriberInfo.Predicate))
                {
                    invoker(subscriberInfo.Subscriber);
                }
            }
        }
    }
}
