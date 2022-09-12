using System;
using System.Collections.Generic;
using System.Linq;
using Taskington.Base.TinyBus.Endpoints;

namespace Taskington.Base.TinyBus;

public abstract record RequestMessage<R>
{
    private static readonly RequestMessageEndPoint<R> messageEndPoint = new();

    public IEnumerable<R> Request() =>
        messageEndPoint.Request();

    public static void Subscribe(Func<R> subscriber) =>
        messageEndPoint.Subscribe(subscriber);

    protected static void Unsubscribe(Func<R> subscriber) =>
        messageEndPoint.Unsubscribe(subscriber);

    protected static void UnsubscribeAll() =>
        messageEndPoint.UnsubscribeAll();
}

public abstract record RequestMessage<T, R> where T : RequestMessage<T, R>
{
    private static readonly RequestMessageEndPoint<T, R> messageEndPoint = new();

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

public static class RequestMessageExtensions
{
    public static IEnumerable<R> RequestMany<R>(this RequestMessage<IEnumerable<R>> message) =>
       message.Request().SelectMany(r => r);

    public static IEnumerable<R> RequestMany<T, R>(this RequestMessage<T, IEnumerable<R>> message)
         where T : RequestMessage<T, IEnumerable<R>> =>
       message.Request().SelectMany(r => r);
}