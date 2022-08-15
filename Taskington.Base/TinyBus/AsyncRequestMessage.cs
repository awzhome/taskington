using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Taskington.Base.TinyBus;

public class AsyncRequestMessage<R> : AsyncTwoWayMessage<Func<Task<R>>, Func<bool>, R>
{
    public async Task<IEnumerable<R>> Request() =>
        await InvokeAndCollect(async subscriber => await subscriber(), predicate => true);
}

public class AsyncRequestMessage<T1, R> : AsyncTwoWayMessage<Func<T1, Task<R>>, Func<T1, bool>, R>
{
    public async Task<IEnumerable<R>> Request(T1 param1) =>
        await InvokeAndCollect(async subscriber => await subscriber(param1), predicate => predicate(param1));
}

public class AsyncRequestMessage<T1, T2, R> : AsyncTwoWayMessage<Func<T1, T2, Task<R>>, Func<T1, T2, bool>, R>
{
    public async Task<IEnumerable<R>> Request(T1 param1, T2 param2) =>
        await InvokeAndCollect(async subscriber => await subscriber(param1, param2), predicate => predicate(param1, param2));
}

public class AsyncRequestMessage<T1, T2, T3, R> : AsyncTwoWayMessage<Func<T1, T2, T3, Task<R>>, Func<T1, T2, T3, bool>, R>
{
    public async Task<IEnumerable<R>> Request(T1 param1, T2 param2, T3 param3) =>
        await InvokeAndCollect(async subscriber => await subscriber(param1, param2, param3), predicate => predicate(param1, param2, param3));
}

public class AsyncRequestMessage<T1, T2, T3, T4, R> : AsyncTwoWayMessage<Func<T1, T2, T3, T4, Task<R>>, Func<T1, T2, T3, T4, bool>, R>
{
    public async Task<IEnumerable<R>> Request(T1 param1, T2 param2, T3 param3, T4 param4) =>
        await InvokeAndCollect(async subscriber => await subscriber(param1, param2, param3, param4), predicate => predicate(param1, param2, param3, param4));
}

public class AsyncRequestMessage<T1, T2, T3, T4, T5, R> : AsyncTwoWayMessage<Func<T1, T2, T3, T4, T5, Task<R>>, Func<T1, T2, T3, T4, T5, bool>, R>
{
    public async Task<IEnumerable<R>> Request(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5) =>
        await InvokeAndCollect(async subscriber => await subscriber(param1, param2, param3, param4, param5), predicate => predicate(param1, param2, param3, param4, param5));
}

public static class AsyncRequestMessageExtensions
{
    public static async Task<IEnumerable<R>> RequestMany<R>(this AsyncRequestMessage<IEnumerable<R>> message) =>
        (await message.Request()).SelectMany(r => r);

    public static async Task<IEnumerable<R>> RequestMany<T1, R>(this AsyncRequestMessage<T1, IEnumerable<R>> message, T1 param1) =>
       (await message.Request(param1)).SelectMany(r => r);

    public static async Task<IEnumerable<R>> RequestMany<T1, T2, R>(this AsyncRequestMessage<T1, T2, IEnumerable<R>> message, T1 param1, T2 param2) =>
       (await message.Request(param1, param2)).SelectMany(r => r);

    public static async Task<IEnumerable<R>> RequestMany<T1, T2, T3, R>(this AsyncRequestMessage<T1, T2, T3, IEnumerable<R>> message, T1 param1, T2 param2, T3 param3) =>
       (await message.Request(param1, param2, param3)).SelectMany(r => r);

    public static async Task<IEnumerable<R>> RequestMany<T1, T2, T3, T4, R>(this AsyncRequestMessage<T1, T2, T3, T4, IEnumerable<R>> message, T1 param1, T2 param2, T3 param3, T4 param4) =>
       (await message.Request(param1, param2, param3, param4)).SelectMany(r => r);

    public static async Task<IEnumerable<R>> RequestMany<T1, T2, T3, T4, T5, R>(this AsyncRequestMessage<T1, T2, T3, T4, T5, IEnumerable<R>> message, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5) =>
       (await message.Request(param1, param2, param3, param4, param5)).SelectMany(r => r);

}
