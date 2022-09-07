using System;

namespace Taskington.Base.TinyBus.Endpoints;

public class MessageEndPoint : OneWayMessageEndPoint<Action, Func<bool>>
{
    public void Push() => Invoke(subscriber => subscriber(), predicate => true);
}

public class MessageEndPoint<T1> : OneWayMessageEndPoint<Action<T1>, Func<T1, bool>>
{
    public void Push(T1 param) =>
        Invoke(subscriber => subscriber(param), subscriber => subscriber(param));
}

public class MessageEndPoint<T1, T2> : OneWayMessageEndPoint<Action<T1, T2>, Func<T1, T2, bool>>
{
    public void Push(T1 param1, T2 param2) =>
        Invoke(subscriber => subscriber(param1, param2), predicate => predicate(param1, param2));
}

public class MessageEndPoint<T1, T2, T3> : OneWayMessageEndPoint<Action<T1, T2, T3>, Func<T1, T2, T3, bool>>
{
    public void Push(T1 param1, T2 param2, T3 param3) =>
        Invoke(subscriber => subscriber(param1, param2, param3), predicate => predicate(param1, param2, param3));
}

public class MessageEndPoint<T1, T2, T3, T4> : OneWayMessageEndPoint<Action<T1, T2, T3, T4>, Func<T1, T2, T3, T4, bool>>
{
    public void Push(T1 param1, T2 param2, T3 param3, T4 param4) =>
        Invoke(subscriber => subscriber(param1, param2, param3, param4), predicate => predicate(param1, param2, param3, param4));
}

public class MessageEndPoint<T1, T2, T3, T4, T5> : OneWayMessageEndPoint<Action<T1, T2, T3, T4, T5>, Func<T1, T2, T3, T4, T5, bool>>
{
    public void Push(T1 param1, T2 param2, T3 param3, T4 param4, T5 param5) =>
        Invoke(subscriber => subscriber(param1, param2, param3, param4, param5), predicate => predicate(param1, param2, param3, param4, param5));
}
