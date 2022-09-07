using System;
using System.Collections.Generic;

namespace Taskington.Base.TinyBus.Endpoints;

public class Subscribable<F, P>
    where F : notnull
    where P : notnull
{
    private readonly List<SubscriberInfo<F, P>> subscriptions = new();

    protected List<SubscriberInfo<F, P>> Subscriptions => subscriptions;

    public void Subscribe(F subscriber)
    {
        subscriptions.Add(new SubscriberInfo<F, P>(subscriber));
    }

    public void Subscribe(F subscriber, P predicate)
    {
        subscriptions.Add(new SubscriberInfo<F, P>(subscriber, predicate));
    }

    public void Unsubscribe(F subscriber)
    {
        subscriptions.RemoveAll(subscriberInfo => subscriber.Equals(subscriberInfo.Subscriber));
    }

    public void UnsubscribeAll()
    {
        subscriptions.Clear();
    }
}

