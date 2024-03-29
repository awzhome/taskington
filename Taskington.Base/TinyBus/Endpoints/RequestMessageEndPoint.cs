using System;
using System.Collections.Generic;
using System.Linq;

namespace Taskington.Base.TinyBus.Endpoints;

public class RequestMessageEndPoint<R> : TwoWayMessageEndPoint<Func<R>, Func<bool>, R>
{
    public IEnumerable<R> Request() =>
        InvokeAndCollect(subscriber => subscriber(), predicate => true);
}

public class RequestMessageEndPoint<T1, R> : TwoWayMessageEndPoint<Func<T1, R>, Func<T1, bool>, R>
{
    public IEnumerable<R> Request(T1 param1) =>
        InvokeAndCollect(subscriber => subscriber(param1), predicate => predicate(param1));
}

public class RequestMessageEndPoint<T1, T2, R> : TwoWayMessageEndPoint<Func<T1, T2, R>, Func<T1, T2, bool>, R>
{
    public IEnumerable<R> Request(T1 param1, T2 param2) =>
        InvokeAndCollect(subscriber => subscriber(param1, param2), predicate => predicate(param1, param2));
}

public class RequestMessageEndPoint<T1, T2, T3, R> : TwoWayMessageEndPoint<Func<T1, T2, T3, R>, Func<T1, T2, T3, bool>, R>
{
    public IEnumerable<R> Request(T1 param1, T2 param2, T3 param3) =>
        InvokeAndCollect(subscriber => subscriber(param1, param2, param3), predicate => predicate(param1, param2, param3));
}

public class RequestMessageEndPoint<T1, T2, T3, T4, R> : TwoWayMessageEndPoint<Func<T1, T2, T3, T4, R>, Func<T1, T2, T3, T4, bool>, R>
{
    public IEnumerable<R> Request(T1 param1, T2 param2, T3 param3, T4 param4) =>
        InvokeAndCollect(subscriber => subscriber(param1, param2, param3, param4), predicate => predicate(param1, param2, param3, param4));
}

public class RequestMessageEndPoint<T1, T2, T3, T4, T5, R> : TwoWayMessageEndPoint<Func<T1, T2, T3, T4, T5, R>, Func<T1, T2, T3, T4, T5, bool>, R>
{
    public IEnumerable<R> Request(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5) =>
        InvokeAndCollect(subscriber => subscriber(param1, param2, param3, param4, param5), predicate => predicate(param1, param2, param3, param4, param5));
}

public static class RequestMessageEndPointExtensions
{
    public static IEnumerable<R> RequestMany<R>(this RequestMessageEndPoint<IEnumerable<R>> message) =>
        message.Request().SelectMany(r => r);

    public static IEnumerable<R> RequestMany<T1, R>(this RequestMessageEndPoint<T1, IEnumerable<R>> message, T1 param1) =>
       message.Request(param1).SelectMany(r => r);

    public static IEnumerable<R> RequestMany<T1, T2, R>(this RequestMessageEndPoint<T1, T2, IEnumerable<R>> message, T1 param1, T2 param2) =>
       message.Request(param1, param2).SelectMany(r => r);

    public static IEnumerable<R> RequestMany<T1, T2, T3, R>(this RequestMessageEndPoint<T1, T2, T3, IEnumerable<R>> message, T1 param1, T2 param2, T3 param3) =>
       message.Request(param1, param2, param3).SelectMany(r => r);

    public static IEnumerable<R> RequestMany<T1, T2, T3, T4, R>(this RequestMessageEndPoint<T1, T2, T3, T4, IEnumerable<R>> message, T1 param1, T2 param2, T3 param3, T4 param4) =>
       message.Request(param1, param2, param3, param4).SelectMany(r => r);

    public static IEnumerable<R> RequestMany<T1, T2, T3, T4, T5, R>(this RequestMessageEndPoint<T1, T2, T3, T4, T5, IEnumerable<R>> message, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5) =>
       message.Request(param1, param2, param3, param4, param5).SelectMany(r => r);

}
