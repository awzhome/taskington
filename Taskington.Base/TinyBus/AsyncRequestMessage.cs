using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taskington.Base.TinyBus.Endpoints;

namespace Taskington.Base.TinyBus;

public abstract record AsyncRequestMessage<T, R> where T : AsyncRequestMessage<T, R>
{
    private static readonly AsyncRequestMessageEndPoint<T, R> messageEndPoint = new();

    public async Task<IEnumerable<R>> Request() =>
        await messageEndPoint.Request((T) this);

    public static void Subscribe(Func<T, Task<R>> subscriber) =>
        messageEndPoint.Subscribe(subscriber);

    public static void Subscribe(Func<T, Task<R>> subscriber, Func<T, bool> predicate) =>
        messageEndPoint.Subscribe(subscriber, predicate);

    protected static void Unsubscribe(Func<T, Task<R>> subscriber) =>
        messageEndPoint.Unsubscribe(subscriber);

    protected static void UnsubscribeAll() =>
        messageEndPoint.UnsubscribeAll();
}

public static class AsyncRequestMessageExtensions
{
    public static async Task<IEnumerable<R>> RequestMany<T, R>(this AsyncRequestMessage<T, IEnumerable<R>> message)
            where T : AsyncRequestMessage<T, IEnumerable<R>> =>
        (await message.Request()).SelectMany(r => r);
}