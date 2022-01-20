using System;
using System.Collections.Generic;
using System.Linq;

namespace Taskington.Base.TinyBus
{
    public class RequestEvent<R> : TwoWayEvent<Func<R>, Func<bool>, R>
    {
        public IEnumerable<R> Request() =>
            InvokeAndCollect(subscriber => subscriber(), predicate => true);
    }

    public class RequestEvent<T1, R> : TwoWayEvent<Func<T1, R>, Func<T1, bool>, R>
    {
        public IEnumerable<R> Request(T1 param1) =>
            InvokeAndCollect(subscriber => subscriber(param1), predicate => predicate(param1));
    }

    public class RequestEvent<T1, T2, R> : TwoWayEvent<Func<T1, T2, R>, Func<T1, T2, bool>, R>
    {
        public IEnumerable<R> Request(T1 param1, T2 param2) =>
            InvokeAndCollect(subscriber => subscriber(param1, param2), predicate => predicate(param1, param2));
    }

    public class RequestEvent<T1, T2, T3, R> : TwoWayEvent<Func<T1, T2, T3, R>, Func<T1, T2, T3, bool>, R>
    {
        public IEnumerable<R> Request(T1 param1, T2 param2, T3 param3) =>
            InvokeAndCollect(subscriber => subscriber(param1, param2, param3), predicate => predicate(param1, param2, param3));
    }

    public class RequestEvent<T1, T2, T3, T4, R> : TwoWayEvent<Func<T1, T2, T3, T4, R>, Func<T1, T2, T3, T4, bool>, R>
    {
        public IEnumerable<R> Request(T1 param1, T2 param2, T3 param3, T4 param4) =>
            InvokeAndCollect(subscriber => subscriber(param1, param2, param3, param4), predicate => predicate(param1, param2, param3, param4));
    }

    public class RequestEvent<T1, T2, T3, T4, T5, R> : TwoWayEvent<Func<T1, T2, T3, T4, T5, R>, Func<T1, T2, T3, T4, T5, bool>, R>
    {
        public IEnumerable<R> Request(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5) =>
            InvokeAndCollect(subscriber => subscriber(param1, param2, param3, param4, param5), predicate => predicate(param1, param2, param3, param4, param5));
    }

    public static class RequestEventExtensions
    {
        public static IEnumerable<R> RequestMany<R>(this RequestEvent<IEnumerable<R>> @event) =>
            @event.Request().SelectMany(r => r);

        public static IEnumerable<R> RequestMany<T1, R>(this RequestEvent<T1, IEnumerable<R>> @event, T1 param1) =>
           @event.Request(param1).SelectMany(r => r); 
        
        public static IEnumerable<R> RequestMany<T1, T2, R>(this RequestEvent<T1, T2, IEnumerable<R>> @event, T1 param1, T2 param2) =>
           @event.Request(param1, param2).SelectMany(r => r);
        
        public static IEnumerable<R> RequestMany<T1, T2, T3, R>(this RequestEvent<T1, T2, T3, IEnumerable<R>> @event, T1 param1, T2 param2, T3 param3) =>
           @event.Request(param1, param2, param3).SelectMany(r => r);
        
        public static IEnumerable<R> RequestMany<T1, T2, T3, T4, R>(this RequestEvent<T1, T2, T3, T4, IEnumerable<R>> @event, T1 param1, T2 param2, T3 param3, T4 param4) =>
           @event.Request(param1, param2, param3, param4).SelectMany(r => r);
        
        public static IEnumerable<R> RequestMany<T1, T2, T3, T4, T5, R>(this RequestEvent<T1, T2, T3, T4, T5, IEnumerable<R>> @event, T1 param1, T2 param2, T3 param3, T4 param4, T5 param5) =>
           @event.Request(param1, param2, param3, param4, param5).SelectMany(r => r);

    }
}
