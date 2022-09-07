using System;
using System.Collections.Generic;
using System.Linq;

namespace Taskington.Base.TinyBus;

public abstract record RequestMessageData<T, R> where T : RequestMessageData<T, R>
{
    private static readonly RequestMessage<T, R> messageEndPoint = new();

    public IEnumerable<R> Request() =>
        messageEndPoint.Request((T) this);

    public static void Subscribe(Func<T, R> subscriber) =>
        messageEndPoint.Subscribe(subscriber);

    public static void Subscribe(Func<T, R> subscriber, Func<T, bool> predicate) =>
        messageEndPoint.Subscribe(subscriber, predicate);

    protected static void Unsubscribe(Func<T, R> subscriber) =>
        messageEndPoint.Unsubscribe(subscriber);

    protected static void UnsubscribeAll() =>
        messageEndPoint.UnsubscribeAll();
}

public static class RequestMessageDataExtensions
{
    public static IEnumerable<R> RequestMany<T, R>(this RequestMessageData<T, IEnumerable<R>> message)
         where T : RequestMessageData<T, IEnumerable<R>> =>
       message.Request().SelectMany(r => r);
}