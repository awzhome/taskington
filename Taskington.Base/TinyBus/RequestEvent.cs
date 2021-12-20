using System;
using System.Collections.Generic;

namespace Taskington.Base.TinyBus
{
    public class RequestEvent<R> : TwoWayEvent<Func<R>, R>
    {
        public IEnumerable<R> Request() =>
            InvokeAndCollect(subscriber => subscriber());
    }

    public class RequestEvent<T1, R> : TwoWayEvent<Func<T1, R>, R>
    {
        public IEnumerable<R> Request(T1 param1) =>
            InvokeAndCollect(subscriber => subscriber(param1));
    }

    public class RequestEvent<T1, T2, R> : TwoWayEvent<Func<T1, T2, R>, R>
    {
        public IEnumerable<R> Request(T1 param1, T2 param2) =>
            InvokeAndCollect(subscriber => subscriber(param1, param2));
    }

    public class RequestEvent<T1, T2, T3, R> : TwoWayEvent<Func<T1, T2, T3, R>, R>
    {
        public IEnumerable<R> Request(T1 param1, T2 param2, T3 param3) =>
            InvokeAndCollect(subscriber => subscriber(param1, param2, param3));
    }

    public class RequestEvent<T1, T2, T3, T4, R> : TwoWayEvent<Func<T1, T2, T3, T4, R>, R>
    {
        public IEnumerable<R> Request(T1 param1, T2 param2, T3 param3, T4 param4) =>
            InvokeAndCollect(subscriber => subscriber(param1, param2, param3, param4));
    }

    public class RequestEvent<T1, T2, T3, T4, T5, R> : TwoWayEvent<Func<T1, T2, T3, T4, T5, R>, R>
    {
        public IEnumerable<R> Request(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5) =>
            InvokeAndCollect(subscriber => subscriber(param1, param2, param3, param4, param5));
    }
}
