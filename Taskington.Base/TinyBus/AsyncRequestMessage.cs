using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Taskington.Base.TinyBus;

public abstract record AsyncRequestMessageData<T, R> where T : AsyncRequestMessageData<T, R>
{
    private static readonly AsyncRequestMessage<T, R> messageEndPoint = new();

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

public static class AsyncRequestMessageDataExtensions
{
    public static async Task<IEnumerable<R>> RequestMany<T, R>(this AsyncRequestMessage<T, IEnumerable<R>> message, T param1) =>
       (await message.Request(param1)).SelectMany(r => r);
}