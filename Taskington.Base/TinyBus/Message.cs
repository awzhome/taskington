using System;
using Taskington.Base.TinyBus.Endpoints;

namespace Taskington.Base.TinyBus;

public abstract record Message<T> where T : Message<T>
{
    private static MessageEndPoint<T> messageEndPoint = new();

    public void Publish() =>
        messageEndPoint.Push((T) this);

    public static void Subscribe(Action<T> subscriber) =>
        messageEndPoint.Subscribe(subscriber);

    public static void Subscribe(Action<T> subscriber, Func<T, bool> predicate) =>
        messageEndPoint.Subscribe(subscriber, predicate);

    protected static void Unsubscribe(Action<T> subscriber) =>
        messageEndPoint.Unsubscribe(subscriber);

    protected static void UnsubscribeAll() =>
        messageEndPoint.UnsubscribeAll();
}
